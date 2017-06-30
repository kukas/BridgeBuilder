using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BridgeBuilder
{
    class Simulation
    {
        public ConcurrentBag<Vertex> Vertices { get; private set; }

        public decimal Damping { get; set; } = 0.0002M;
        public decimal Stiffness { get; set; } = 0.2M;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Gravitation = false;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Vertices = new ConcurrentBag<Vertex>();
        }

        public void Update(float dt)
        {
            foreach(var v in Vertices)
            {
                v.Update(dt);
            }
        }

        public void AddVertex(float x, float y)
        {
            Vertices.Add(new Vertex(this, x, y));
        }
    }
}