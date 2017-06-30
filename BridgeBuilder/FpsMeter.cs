using System;
using System.Diagnostics;

namespace BridgeBuilder
{
    internal class FpsMeter
    {
        Stopwatch sw = new Stopwatch();
        long last = 0;

        public float FPS { get; internal set; }

        public FpsMeter()
        {
            sw.Start();
        }

        public int Next()
        {
            long now = sw.ElapsedMilliseconds;
            long dt = now - last;
            last = now;
            FPS = 1000f / dt;

            long goal = 1000 / 30;
            int wait = Math.Max((int)(goal - dt), 0);
            // Debug.WriteLine(wait);
            return wait;
        }
    }
}