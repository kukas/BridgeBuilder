﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    [Serializable]
    class Edge
    {
        public Vertex V { get; }
        public Vertex U { get; }
        public float CurrentLength { get { return U.Position.Sub(V.Position).Mag(); } }

        public float Length;

        public Edge(Vertex v, Vertex u)
        {
            V = v;
            U = u;
            ResetLength();
        }

        public Vertex GetOpposite(Vertex v)
        {
            if (V == v)
                return U;
            else
                return V;
        }
        // https://en.wikipedia.org/wiki/Verlet_integration#Constraints
        // Trik: průměruji všechny přírustky, aby nezáleželo na pořadí aplikování constraints
        // nevýhoda je ta, že jsou constraints mnohem pružnější
        // furt je to ale nejstabilnější simulace, které jsem dosáhl
        // případně se můžu vykašlat na pořadí a mít opravdu nepružné traverzy. Uvidím později.
        public void Relax()
        {
            PointF delta = U.Position.Sub(V.Position);
            float distance = delta.Mag();
            float ratio = (distance - Length) / distance;
            
            if (!V.Fixed)
            {
                V.ConstrainedDelta = V.ConstrainedDelta.Add(delta.MultiplyScalar(ratio * 0.5f));
                V.ConstrainedCount++;
            }
            if (!U.Fixed)
            {
                U.ConstrainedDelta = U.ConstrainedDelta.Add(delta.MultiplyScalar(-ratio * 0.5f));
                U.ConstrainedCount++;
            }
            
            /*
            if (!V.Fixed)
                V.Position = V.Position.Add(delta.MultiplyScalar(ratio * 0.5f));
            if (!U.Fixed)
                U.Position = U.Position.Add(delta.MultiplyScalar(-ratio * 0.5f));
            */
        }

        internal void ResetLength()
        {
            Length = CurrentLength;
        }
    }
}
