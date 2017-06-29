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
        MouseEventArgs mouse;
        PointF MousePosition = new PointF();

        public ConcurrentBag<Vertex> Vertices { get; private set; }

        public decimal Damping { get; set; } = 0.0002M;
        public decimal Stiffness { get; set; } = 0.2M;

        Connector connector;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Gravitation = false;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            connector = new Connector(this);
            Vertices = new ConcurrentBag<Vertex>();
        }

        public void Update(float dt)
        {
            if(mouse != null)
            {
                MousePosition.X = mouse.X;
                MousePosition.Y = mouse.Y;
            }
            foreach(var v in Vertices)
            {
                v.Update(dt);
                var dMouse = MousePosition.Sub(v.Position);
                if (dMouse.Mag() < 10)
                {
                    v.Selected = true;
                }
                else
                {
                    v.Selected = false;
                }
            }
        }

        internal void KeyPress(KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyCode == Keys.F);
            if(e.KeyCode == Keys.F)
            {
                Vertex selected = Vertices.FirstOrDefault(v => v.Selected);
                if(selected != null)
                {
                    selected.Fixed = !selected.Fixed;
                }
            }
            else if(e.KeyCode == Keys.D)
            {
                Vertex selected = Vertices.FirstOrDefault(v => v.Selected);
                if (selected != null)
                {
                    // selected.Neighbours.
                }
            }
        }

        internal void MouseDown(MouseEventArgs e)
        {
            mouse = e;
            if(e.Button == MouseButtons.Right)
            {
                if(Vertices.FirstOrDefault(v => v.Selected) != null || connector.Selected != null)
                    connector.Connect();
                else
                    AddVertex(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left)
            {
                var selected = Vertices.FirstOrDefault(v => v.Selected);
                if (selected != null)
                    selected.Dragging = true;
            }
        }

        class Connector
        {
            public Vertex Selected;
            private Simulation simulation;

            public Connector(Simulation simulation)
            {
                this.simulation = simulation;
            }

            public void Connect()
            {
                if(Selected == null)
                {
                    Selected = simulation.Vertices.FirstOrDefault(v => v.Selected);
                }
                else
                {
                    Vertex selected2 = simulation.Vertices.FirstOrDefault(v => v.Selected);
                    if(selected2 != null && selected2 != Selected)
                    {
                        Selected.AddEdge(selected2);
                        
                    }
                    Selected = null;
                }
            }
        }

        internal void MouseMove(MouseEventArgs e)
        {
            mouse = e;
        }

        internal void MouseUp(MouseEventArgs e)
        {
            mouse = e;
            foreach (var v in Vertices)
                v.Dragging = false;
        }

        public void AddVertex(float x, float y)
        {
            Vertices.Add(new Vertex(this, x, y));
        }
    }
}