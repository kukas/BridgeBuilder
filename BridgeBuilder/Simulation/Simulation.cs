using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BridgeBuilder
{
    [Serializable]
    internal class Simulation
    {
        public ConcurrentBag<Vertex> Vertices { get; private set; }
        public ConcurrentBag<Edge> Edges { get; private set; }

        private ConcurrentBag<Vertex> newVertices;
        private ConcurrentBag<Edge> newEdges;

        public float Damping = 0.01f;
        public float GravitationStrength = 5000f;
        public float DraggingStrength = 500f;
        public float DraggingDamping = 50f;
        public float GroundStrength = 5000f;
        public float GroundDamping = 5f;

        public int Width { get; }
        public int Height { get; }

        public bool Pause { get; set; } = true;
        public bool Gravitation { get; set; } = true;

        public float MaxStrain = 0.01f;
        private const int relaxationSteps = 10;

        public Simulation(int width, int height)
        {
            Width = width;
            Height = height;
            Vertices = new ConcurrentBag<Vertex>();
            Edges = new ConcurrentBag<Edge>();
        }

        public void Update(float dt)
        {
            // záměna seznamu hran (s odebranými trámy)
            if (newEdges != null)
            {
                Edges = newEdges;
                newEdges = null;
            }
            // záměna seznamu vrcholů (s odebranými body)
            if (newVertices != null)
            {
                Vertices = newVertices;
                newVertices = null;
            }
            if (Pause) return;

            // update bodů
            foreach (Vertex v in Vertices) v.Update(dt);
            // update trámů = "relaxace"
            for (int i = 0; i < relaxationSteps; i++)
            {
                foreach (Vertex v in Vertices) v.ResetConstraints();
                foreach (Edge e in Edges) e.Relax();
                foreach (Vertex v in Vertices) v.ApplyConstraints();
            }
            
            // odebere příliš namáhané trámy
            IEnumerable<Edge> tooStrained = Edges.Where(e => Math.Abs(1 - e.Length / e.CurrentLength) > MaxStrain);
            if (tooStrained.Any())
                RemoveEdges(tooStrained);

            // odebere body mimo obraz
            // tolerance na Y ose - kvůli kolizím se zemí (míček se při kolizi občas na chvíli protuneluje mimo obraz)
            const float d = 50;
            IEnumerable<Vertex> tooFar = Vertices.Where(v => v.Position.X < 0 || v.Position.X > Width || v.Position.Y < -d || v.Position.Y > Height+d);
            if (tooFar.Any())
                RemoveVertices(tooFar);
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
            List<Edge> edgesList = Edges.ToList();
            newEdges = new ConcurrentBag<Edge>(edgesList.Except(edgesToRemove));
        }

        internal void RemoveVertices(IEnumerable<Vertex> verticesToRemove)
        {
            List<Vertex> verticesList = Vertices.ToList();
            newVertices = new ConcurrentBag<Vertex>(verticesList.Except(verticesToRemove));
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
            foreach (Vertex v in loadedSimulation.Vertices) v.SetSimulation(this);
        }

        internal void Clear()
        {
            Vertices = new ConcurrentBag<Vertex>();
            Edges = new ConcurrentBag<Edge>();
        }
    }
}