using System;
using System.Diagnostics;

namespace BridgeBuilder
{
    internal class FpsMeter
    {
        Stopwatch sw = new Stopwatch();
        double last = 0;

        public double FPS { get; internal set; }

        public FpsMeter()
        {
            sw.Start();
        }

        public int Next()
        {
            double now = sw.Elapsed.TotalMilliseconds;
            double dt = now - last;
            last = now;
            FPS = 1000 / dt;

            long goal = 1000 / 30;
            int wait = Math.Max((int)(goal - dt), 0);
            return wait;
        }
    }
}