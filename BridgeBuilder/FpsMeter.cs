using System.Diagnostics;

namespace BridgeBuilder
{
    internal class FpsMeter
    {
        Stopwatch sw = new Stopwatch();
        long last = 0;

        public FpsMeter()
        {
            sw.Start();
        }

        public float Frame()
        {
            long now = sw.ElapsedMilliseconds;
            long dt = now-last;
            last = now;
            return 1000f / dt;
        }
    }
}