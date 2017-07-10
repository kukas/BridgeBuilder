using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BridgeBuilder
{
    internal class TestingStress
    {
        private readonly Simulation simulation;

        private Edge currentRoad;
        private float currentPosition;

        public decimal Speed { get; set; } = 150M;
        public decimal Weight { get; set; } = 30000M;

        public bool Started => currentRoad != null;
        public PointF Position;

        public TestingStress(Simulation simulation)
        {
            this.simulation = simulation;
        }

        public bool StartTest()
        {
            IEnumerable<Edge> roads = simulation.Edges.Where(e => e.IsRoad);

            if (!roads.Any())
                return false;

            Edge leftmost = roads.First();

            foreach (Edge road in roads)
                if (road.U.Position.X < leftmost.U.Position.X)
                    leftmost = road;

            currentRoad = leftmost;
            currentPosition = 0;
            return true;
        }

        public void Update(float dt)
        {
            if (currentRoad == null)
                return;

            currentPosition += (float)Speed * dt;
            float ratio = currentPosition / currentRoad.Length;
            PointF uPos = currentRoad.U.Position, vPos = currentRoad.V.Position;
            Position = uPos.Add(vPos.Sub(uPos).MultiplyScalar(ratio));
            if (ratio > 1)
            {
                NextRoad();
            }
            else
            {
                float maxLoad = (float)Weight;
                float uForce = 1 - ratio;
                currentRoad.U.ApplyForce(new PointF(0, maxLoad * uForce));
                float vForce = ratio;
                currentRoad.V.ApplyForce(new PointF(0, maxLoad * vForce));
            }
        }

        private void NextRoad()
        {
            IEnumerable<Edge> roads = simulation.Edges.Where(e => e.IsRoad && e.U == currentRoad.V);
            currentRoad = roads.FirstOrDefault();
            currentPosition = 0;
        }
    }
}