using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BridgeBuilder
{
    [Serializable]
    class Simulation
    {
        public ConcurrentBag<Vertex> Vertices { get; private set; }
        public ConcurrentBag<Edge> Edges { get; private set; }

        private ConcurrentBag<Vertex> newVertices;
        private ConcurrentBag<Edge> newEdges;


        public decimal Damping { get; set; } = 0.01M;
        public decimal Stiffness { get; set; } = 1M;
        public decimal GravitationStrength { get; set; } = 1000M;
        public decimal DraggingStrength { get; set; } = 500M;
        public decimal DraggingDamping { get; set; } = 50M;
        public decimal GroundStrength { get; set; } = 5000M;
        public decimal GroundDamping { get; set; } = 5M;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Pause = true;
        public bool Gravitation = true;
        public float MaxStrain = 0.01f;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Vertices = new ConcurrentBag<Vertex>();
            Edges = new ConcurrentBag<Edge>();

            var v = AddVertex(100, 110);
            //v.PrevPos.X -= 5;
            v.Position.X += 0.016f;
            AddVertex(200, 110);
            AddVertex(150, 110);

            var board = new Vertex[1, 1];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    board[x, y] = new Vertex(this, x * 20 + 100, y * 20 + 100);
                    Vertices.Add(board[x, y]);
                }
            }
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    for (int dx = Math.Max(x - 1, 0); dx <= Math.Min(x + 1, board.GetLength(0) - 1); dx++)
                    {
                        for (int dy = Math.Max(y - 1, 0); dy <= Math.Min(y + 1, board.GetLength(1) - 1); dy++)
                        {
                            if (dx > x || dy > y)
                                AddEdge(board[x, y], board[dx, dy]);
                        }
                    }
                }
            }
        }

        public void Update(float dt)
        {
            if (newEdges != null)
            {
                Edges = newEdges;
                newEdges = null;
            }
            if (newVertices != null)
            {
                Vertices = newVertices;
                newVertices = null;
            }
            if (Pause) return;
            foreach (var v in Vertices) v.Update(dt);
            for (int i = 0; i < 10; i++)
            {
                foreach (var v in Vertices) v.ResetConstrains();
                foreach (var e in Edges) e.Relax();
                foreach (var v in Vertices) v.ApplyConstrains();
            }
            var tooStrained = Edges.Where(e => Math.Abs(1 - e.Length / e.CurrentLength) > MaxStrain);
            if (tooStrained.Any())
                RemoveEdges(tooStrained);
        }

        public Vertex AddVertex(float x, float y)
        {
            Vertex vertex = new Vertex(this, x, y);
            Vertices.Add(vertex);
            return vertex;
        }

        public Edge AddEdge(Vertex vertex1, Vertex vertex2)
        {
            Edge edge = new Edge(vertex1, vertex2);
            Edges.Add(edge);
            return edge;
        }

        internal void RemoveEdges(IEnumerable<Edge> edgesToRemove)
        {
            List<Edge> EdgesList = Edges.ToList();
            newEdges = new ConcurrentBag<Edge>(EdgesList.Except(edgesToRemove));
        }

        internal void RemoveVertices(IEnumerable<Vertex> verticesToRemove)
        {
            List<Vertex> VerticesList = Vertices.ToList();
            newVertices = new ConcurrentBag<Vertex>(VerticesList.Except(verticesToRemove));
            RemoveEdges(Edges.Where(e => verticesToRemove.Contains(e.U) || verticesToRemove.Contains(e.V)));
        }

        internal IEnumerable<Edge> GetEdges(Vertex v)
        {
            return Edges.Where(edge => edge.U == v || edge.V == v);
        }

        internal void LoadSimulation(Simulation loadedSimulation)
        {
            Vertices = loadedSimulation.Vertices;
            Edges = loadedSimulation.Edges;
            foreach (var v in loadedSimulation.Vertices) v.SetSimulation(this);
        }

        internal void Clear()
        {
            Vertices = new ConcurrentBag<Vertex>();
            Edges = new ConcurrentBag<Edge>();
        }
    }
}