using System;
using System.Diagnostics;

namespace BridgeBuilder
{
    internal class FpsMeter
    {
        private readonly Stopwatch sw = new Stopwatch();
        private readonly double[] history;
        private const int historyLength = 50;
        private int pointer;
        private const long Goal = 1000 / 20;

        public double Fps { get; internal set; }

        public FpsMeter()
        {
            history = new double[historyLength];
            sw.Start();
        }

        public int Next()
        {
            double now = sw.Elapsed.TotalMilliseconds;

            history[pointer] = now;
            pointer = (pointer+1)%historyLength;

            double dt = (now - history[pointer])/historyLength;

            Fps = 1000 / dt;

            int wait = Math.Max((int)(Goal - dt), 0);
            return wait;
        }
    }
}