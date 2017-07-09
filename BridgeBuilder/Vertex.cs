using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    [Serializable]
    class Vertex
    {
        private Simulation simulation;

        public PointF ConstrainedDelta = new PointF();
        public int ConstrainedCount = 0;
        public PointF Position;
        public PointF PrevPos;
        public PointF Force;

        public bool Fixed = false;
        public float Radius = 10f;
        public float Mass = 1f;

        private PointF target;
        private bool targetSet = false;

        public Vertex(Simulation simulation, float x, float y)
        {
            this.simulation = simulation;
            Position = PrevPos = new PointF(x, y);
        }

        public void SetSimulation(Simulation simulation)
        {
            this.simulation = simulation;
        }

        private PointF ComputeForces(float dt)
        {
            PointF force = Force;
            Force = new PointF();

            if (simulation.Gravitation)
                force.Y += simulation.GravitationStrength;

            PointF Velocity = Position.Sub(PrevPos).MultiplyScalar(1f / dt);

            var drag = Velocity.MultiplyScalar(-simulation.Damping);
            force = force.Add(drag);

            if (Position.Y + Radius > simulation.Height)
                force.Y -= (Position.Y + Radius - simulation.Height) * simulation.GroundStrength + Velocity.Y * simulation.GroundDamping;

            if (targetSet)
            {
                var draggingForce = target.Sub(Position).MultiplyScalar(simulation.DraggingStrength);
                var damping = Velocity.MultiplyScalar(-simulation.DraggingDamping);
                force = force.Add(draggingForce).Add(damping);
            }
            return force;
        }

        internal void ApplyForce(PointF force)
        {
            Force = Force.Add(force);
        }

        // https://en.wikipedia.org/wiki/Verlet_integration#Verlet_integration_.28without_velocities.29
        public void Update(float dt)
        {
            if (Fixed)
                return;

            PointF NextPos = Position.Add(Position).Sub(PrevPos).Add(ComputeForces(dt).MultiplyScalar(dt * dt));
            PrevPos = Position;
            Position = NextPos;
        }

        public void ResetConstrains()
        {
            ConstrainedDelta = new PointF();
            ConstrainedCount = 0;
        }

        internal void ApplyConstrains()
        {
            // asi hloupost:
            // float a = (float)simulation.Stiffness;
            // Position = Position.Add(ConstrainedDelta.MultiplyScalar(1f / (a*ConstrainedCount+1-a)));
            if (ConstrainedCount > 0)
                Position = Position.Add(ConstrainedDelta.MultiplyScalar(1f / ConstrainedCount));
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
            return $"now: {Position} prev: {PrevPos}";
        }
    }
}
