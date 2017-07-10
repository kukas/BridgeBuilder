using System;
using System.Drawing;

namespace BridgeBuilder
{
    [Serializable]
    internal class Vertex
    {
        private Simulation simulation;

        public PointF ConstrainedDelta;
        public int ConstrainedCount;

        public PointF Position;
        public PointF PrevPos;
        public PointF Force;

        public bool Fixed = false;
        public float Radius = 10f;

        private PointF target;
        private bool targetSet;

        public Vertex(Simulation simulation, float x, float y)
        {
            this.simulation = simulation;
            Position = PrevPos = new PointF(x, y);
        }

        public void SetSimulation(Simulation s)
        {
            simulation = s;
        }

        private PointF ComputeForces(float dt)
        {
            // Výsledná síla se skládá ze:
            // - zátěže při testování
            PointF force = Force;
            Force = new PointF();

            // - působení gravitace
            if (simulation.Gravitation)
                force.Y += simulation.GravitationStrength;

            PointF velocity = Position.Sub(PrevPos).MultiplyScalar(1f / dt);

            // - "odporu vzduchu" = přirozené ztrácení celkové energie systému
            PointF drag = velocity.MultiplyScalar(-simulation.Damping);
            force = force.Add(drag);

            // - působení země při kolizi s ní
            if (Position.Y + Radius > simulation.Height)
                force.Y -= (Position.Y + Radius - simulation.Height) * simulation.GroundStrength + velocity.Y * simulation.GroundDamping;

            // - působení myši při přesunu v módu simulace
            if (targetSet)
            {
                PointF draggingForce = target.Sub(Position).MultiplyScalar(simulation.DraggingStrength);
                PointF damping = velocity.MultiplyScalar(-simulation.DraggingDamping);
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

            PointF nextPos = Position.Add(Position).Sub(PrevPos).Add(ComputeForces(dt).MultiplyScalar(dt * dt));
            PrevPos = Position;
            Position = nextPos;
        }

        public void ResetConstraints()
        {
            ConstrainedDelta = new PointF();
            ConstrainedCount = 0;
        }

        internal void ApplyConstraints()
        {
            // asi hloupost:
            // float a = (float)simulation.Stiffness;
            // Position = Position.Add(ConstrainedDelta.MultiplyScalar(1f / (a*ConstrainedCount+1-a)));

            if (ConstrainedCount > 0)
                Position = Position.Add(ConstrainedDelta.MultiplyScalar(1f / ConstrainedCount));
        }

        public void SetTarget(PointF t)
        {
            target = t;
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
