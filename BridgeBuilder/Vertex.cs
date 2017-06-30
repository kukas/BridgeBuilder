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

        public bool Fixed = false;
        public float Radius = 10;

        private PointF target;
        private bool targetSet = false;

        public Vertex(Simulation simulation, float x, float y)
        {
            this.simulation = simulation;
            Position = new PointF(x, y);
            Neighbours = new ConcurrentBag<Edge>();
        }

        public void Update(float dt)
        {
            if (Fixed)
                return;
            var dPosition = Velocity.Clone().MultiplyScalar(dt);
            Position = Position.Add(dPosition);

            PointF force = new PointF();
            if (simulation.Gravitation)
                force.Y = 0.1f;
            foreach (var edge in Neighbours)
            {
                var v = edge.GetOpposite(this);

                var toV = v.Position.Sub(Position);
                var distance = toV.Mag();
                var spring = toV.MultiplyScalar(edge.Length / distance);

                var springDelta = toV.Sub(spring).MultiplyScalar((float)simulation.Stiffness);

                force.X += springDelta.X;
                force.Y += springDelta.Y;
            }

            var drag = Velocity.MultiplyScalar(-(float)simulation.Damping);
            force = force.Add(drag);

            if (Position.Y > simulation.Height)
                force.Y -= Position.Y - simulation.Height;

            if (targetSet)
            {
                var draggingForce = target.Sub(Position).MultiplyScalar(0.005f);
                force = force.Add(draggingForce);
            }

            Velocity = Velocity.Add(force);
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
    }
}
