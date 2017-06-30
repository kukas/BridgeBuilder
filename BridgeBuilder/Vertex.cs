using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
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
            Position = new PointF(x, y);
            Neighbours = new ConcurrentBag<Edge>();
        }
        // http://gafferongames.com/game-physics/integration-basics/
        public void Update(float dt)
        {
            if (Fixed)
                return;

            PointF force = new PointF();
            if (simulation.Gravitation)
                force.Y = (float)simulation.GravitationStrength;
            foreach (var edge in Neighbours)
            {
                var v = edge.GetOpposite(this);

                var toV = v.Position.Sub(Position);
                var distance = toV.Mag();
                var spring = toV.MultiplyScalar(edge.Length / distance);
                var x = toV.Sub(spring);
                var f = x.MultiplyScalar((float)simulation.Stiffness / (2*dt*dt));

                var dv = v.Velocity.Sub(Velocity).MultiplyScalar((float)simulation.Damping / dt);
                // var dv = new PointF();

                force = force.Add(f).Add(dv);
                // force = force.Add(dv);
                /*
                var toV = v.Position.Sub(Position);
                var distance = toV.Mag();
                var spring = toV.MultiplyScalar(edge.Length / distance);

                var springDelta = toV.Sub(spring).MultiplyScalar((float)simulation.Stiffness);

                var damping = v.Velocity.Sub(Velocity).MultiplyScalar(-(float)simulation.Damping);
                force.X += springDelta.X - damping.X;
                force.Y += springDelta.Y - damping.Y;
                */
            }

            var drag = Velocity.MultiplyScalar(-1);
            force = force.Add(drag);

            if (Position.Y > simulation.Height)
                force.Y -= (Position.Y - simulation.Height)*10;

            if (targetSet)
            {
                //Position = target;
                
                var draggingForce = target.Sub(Position).MultiplyScalar((float)simulation.DraggingStrength);
                force = force.Add(draggingForce);
            }
            
            nVelocity = Velocity.Add(force.MultiplyScalar(dt));
            var dPosition = Velocity.MultiplyScalar(dt);
            nPosition = Position.Add(dPosition);
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
