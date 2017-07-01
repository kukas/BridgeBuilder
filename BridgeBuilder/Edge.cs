using System;
using System.Collections.Generic;
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
        public float Length;

        public Edge(Vertex v, Vertex u)
        {
            V = v;
            U = u;
            Length = V.Position.Sub(U.Position).Mag();
        }

        public Vertex GetOpposite(Vertex v)
        {
            if (V == v)
                return U;
            else
                return V;
        }
    }
}
