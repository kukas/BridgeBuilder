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


        public float Damping = 0.01f;
        public float GravitationStrength = 5000f;
        public float DraggingStrength = 500f;
        public float DraggingDamping = 50f;
        public float GroundStrength = 5000f;
        public float GroundDamping = 5f;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Pause { get; set; } = true;
        public bool Gravitation { get; set; } = true;

        public float MaxStrain = 0.01f;
        private int RelaxationSteps = 10;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Vertices = new ConcurrentBag<Vertex>();
            Edges = new ConcurrentBag<Edge>();
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
            for (int i = 0; i < RelaxationSteps; i++)
            {
                foreach (var v in Vertices) v.ResetConstrains();
                foreach (var e in Edges) e.Relax();
                foreach (var v in Vertices) v.ApplyConstrains();
            }

            var tooStrained = Edges.Where(e => Math.Abs(1 - e.Length / e.CurrentLength) > MaxStrain);
            if (tooStrained.Any())
                RemoveEdges(tooStrained);

            // tolerance v Y ose - kvůli kolizím se zemí (míček se při kolizi občas na chvíli protuneluje mimo obraz)
            float d = 50;
            var tooFar = Vertices.Where(v => v.Position.X < 0 || v.Position.X > Width || v.Position.Y < -d || v.Position.Y > Height+d);
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