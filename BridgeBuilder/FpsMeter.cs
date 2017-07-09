using System;
using System.Diagnostics;

namespace BridgeBuilder
{
    internal class FpsMeter
    {
        Stopwatch sw = new Stopwatch();
        double[] history;
        int len = 50;
        int p = 0;
        double last = 0;

        public double FPS { get; internal set; }

        public FpsMeter()
        {
            history = new double[len];
            sw.Start();
        }

        public int Next()
        {
            double now = sw.Elapsed.TotalMilliseconds;
            history[p] = now;
            p = (p+1)%len;
            double dt = (now - history[p])/len;
            last = now;
            FPS = 1000 / dt;

            long goal = 1000 / 20;
            int wait = Math.Max((int)(goal - dt), 0);
            return wait;
        }
    }
}