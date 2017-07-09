using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace BridgeBuilder
{
    internal class TestingStress
    {
        private Simulation simulation;

        private Edge currentRoad;
        private float currentPosition = 0;

        public decimal Speed { get; set; } = 50M;
        public decimal Weight { get; set; } = 5000M;
        
        public bool Started { get { return currentRoad != null; } }
        public PointF Position = new PointF();

        public TestingStress(Simulation simulation)
        {
            this.simulation = simulation;
        }

        public bool StartTest()
        {
            var roads = simulation.Edges.Where(e => e.IsRoad);

            if (!roads.Any())
                return false;

            Edge leftmost = roads.First();

            foreach (var road in roads)
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

            currentPosition += (float)Speed*dt;
            float ratio = currentPosition / currentRoad.Length;
            PointF uPos = currentRoad.U.Position, vPos = currentRoad.V.Position;
            Position = uPos.Add(vPos.Sub(uPos).MultiplyScalar(ratio));
            // Debug.WriteLine(Position);
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
            var roads = simulation.Edges.Where(e => e.IsRoad && e.U == currentRoad.V);
            if (roads.Any())
                currentRoad = roads.First();
            else
                currentRoad = null;
            currentPosition = 0;
        }
    }
}