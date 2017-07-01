using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    class State
    {
        public PointF x = new PointF();      // position
        public PointF v = new PointF();      // velocity
    };

    class Derivative
    {
        public PointF dx = new PointF();      // dx/dt = velocity
        public PointF dv = new PointF();      // dv/dt = acceleration
    };

    [Serializable]
    class Vertex
    {
        private Simulation simulation;

        public ConcurrentBag<Edge> Neighbours;
        public PointF Position;
        public PointF Velocity = new PointF();

        public PointF nPosition;
        public PointF nVelocity = new PointF();

        public bool Fixed = false;
        public float Radius = 10f;
        public float Mass = 1f;

        private PointF target;
        private bool targetSet = false;

        public Vertex(Simulation simulation, float x, float y)
        {
            this.simulation = simulation;
            Position = nPosition = new PointF(x, y);
            Neighbours = new ConcurrentBag<Edge>();
        }

        // http://gafferongames.com/game-physics/integration-basics/
        public Derivative Evaluate(State initial, float t, float dt, Derivative d)
        {
            State state = new State();
            state.x = initial.x.Add(d.dx.MultiplyScalar(dt));
            state.v = initial.v.Add(d.dv.MultiplyScalar(dt));

            Derivative output = new Derivative();
            output.dx = state.v;
            output.dv = ComputeForces(state, t + dt);
            return output;
        }

        void Integrate(State state, float t, float dt)
        {
            Derivative a, b, c, d;

            a = Evaluate(state, t, 0.0f, new Derivative());
            b = Evaluate(state, t, dt * 0.5f, a);
            c = Evaluate(state, t, dt * 0.5f, b);
            d = Evaluate(state, t, dt, c);

            //float dxdt = 1.0f / 6.0f *
            //    (a.dx + 2.0f * (b.dx + c.dx) + d.dx);
            PointF dxdt = a.dx.Add(b.dx.Add(c.dx).MultiplyScalar(2)).Add(d.dx).MultiplyScalar(1.0f / 6.0f);

            //float dvdt = 1.0f / 6.0f *
            //    (a.dv + 2.0f * (b.dv + c.dv) + d.dv);
            PointF dvdt = a.dv.Add(b.dv.Add(c.dv).MultiplyScalar(2)).Add(d.dv).MultiplyScalar(1.0f / 6.0f);

            state.x = state.x.Add(dxdt.MultiplyScalar(dt));
            state.v = state.v.Add(dvdt.MultiplyScalar(dt));
        }

        private PointF ComputeForces(State state, float t)
        {
            PointF force = new PointF();
            if (simulation.Gravitation)
                force.Y = (float)simulation.GravitationStrength;
            foreach (var edge in Neighbours)
            {
                var v = edge.GetOpposite(this);

                var toV = v.Position.Sub(state.x);
                var distance = toV.Mag();
                var spring = toV.MultiplyScalar(edge.Length / distance);
                var x = toV.Sub(spring);
                var f = x.MultiplyScalar((float)simulation.Stiffness * 1E6f / 2f);

                var dv = v.Velocity.Sub(state.v).MultiplyScalar((float)simulation.Damping * 1E3f);
                // var dv = 

                force = force.Add(f).Add(dv);
            }
            /*
            var drag = state.v.MultiplyScalar(-1);
            force = force.Add(drag);
            */
            if (state.x.Y + Radius > simulation.Height)
                force.Y -= (state.x.Y + Radius - simulation.Height) * (float)simulation.GroundStrength + Velocity.Y* (float)simulation.GroundDamping;

            if (targetSet)
            {
                var draggingForce = target.Sub(state.x).MultiplyScalar((float)simulation.DraggingStrength);
                var damping = state.v.MultiplyScalar(-(float)simulation.DraggingDamping);
                force = force.Add(draggingForce).Add(damping);
            }
            return force;
        }

        public void Update(float dt)
        {
            if (Fixed)
                return;

            State now = new State();
            now.x = Position;
            now.v = Velocity;
            Integrate(now, 0, dt);

            // euler
            //nVelocity = Velocity.Add(force.MultiplyScalar(dt));
            //var dPosition = Velocity.MultiplyScalar(dt);
            //nPosition = Position.Add(dPosition);
            nVelocity = now.v;
            nPosition = now.x;
        }

        internal void Step()
        {
            Position = nPosition;
            Velocity = nVelocity;
        }

        internal void AddEdge(Vertex neighbour)
        {
            Edge edge = new Edge(this, neighbour);
            Neighbours.Add(edge);
            neighbour.Neighbours.Add(edge);
        }

        public void SetTarget(PointF target)
        {
            this.target = target;
            targetSet = true;
        }
        public void ResetTarget()
        {
            targetSet = false;
        }
        public override string ToString()
        {
            return $"p: {Position} v: {Velocity}";
        }
    }
}
