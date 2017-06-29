using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BridgeBuilder
{
    internal class Simulation
    {
        MouseEventArgs mouse;
        PointF MousePosition = new PointF();

        class Edge
        {
            public Vertex V;
            public Vertex U;
            public float Length;
            public Edge(Vertex v, Vertex u)
            {
                this.V = v;
                this.U = u;
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

        class Vertex
        {
            public ConcurrentBag<Edge> Neighbours;
            public PointF Position;
            public PointF Velocity;

            public Vertex(Simulation simulation, float x, float y)
            {
                this.simulation = simulation;
                Position = new PointF(x, y);
                Neighbours = new ConcurrentBag<Edge>();
            }

            public bool Selected = false;

            public bool Fixed = false;
            private Simulation simulation;

            public bool Dragging = false;

            public void Update(float dt)
            {
                if (Fixed)
                    return;
                var dPosition = Velocity.Clone().MultiplyScalar(dt);
                Position = Position.Add(dPosition);

                PointF force = new PointF();
                if (simulation.Gravitation)
                    force.Y = 0.1f;
                foreach(var edge in Neighbours)
                {
                    var v = edge.GetOpposite(this);

                    var toV = v.Position.Sub(Position);
                    var distance = toV.Mag();
                    var spring = toV.MultiplyScalar(edge.Length / distance);

                    var springDelta = toV.Sub(spring).MultiplyScalar((float)simulation.Stiffness);
                    
                    force.X += springDelta.X;
                    force.Y += springDelta.Y;
                }

                var drag = Velocity.MultiplyScalar(-(float)simulation.Damping);
                force = force.Add(drag);

                if (Position.Y > simulation.Height)
                    force.Y -= Position.Y - simulation.Height;

                if (Dragging)
                {
                    var toMouse = simulation.MousePosition.Sub(Position).MultiplyScalar(0.005f);
                    force = force.Add(toMouse);
                }

                Velocity = Velocity.Add(force);
            }

            internal void AddEdge(Vertex selected2)
            {
                Edge edge = new Edge(this, selected2);
                this.Neighbours.Add(edge);
                selected2.Neighbours.Add(edge);
            }
        }

        ConcurrentBag<Vertex> vertices;

        // public decimal Damping ;
        // public decimal Stiffness = 0.01M;
        public decimal Damping { get; set; } = 0.0001M;
        public decimal Stiffness { get; set; } = 0.01M;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            connector = new Connector(this);
            vertices = new ConcurrentBag<Vertex>();
            /*
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                float x = (float)rnd.NextDouble() * width;
                float y = (float)rnd.NextDouble() * height;
                var v = new Vertex(x, y);
                v.Velocity.X = (float)rnd.NextDouble() * 2 - 1;
                v.Velocity.Y = (float)rnd.NextDouble() * 2 - 1;
                vertices.Add(v);
            }

            foreach(var v in vertices)
            {
                int neighbourCount = rnd.Next(10);
                while(v.Neighbours.Count < neighbourCount)
                {
                    var candidate = vertices[rnd.Next(vertices.Count)];
                    if(candidate != v && v.Neighbours.IndexOf(candidate) == -1)
                    {
                        v.Neighbours.Add(candidate);
                    }
                }
            }*/
        }

        public void Update(float dt)
        {
            if(mouse != null)
            {
                MousePosition.X = mouse.X;
                MousePosition.Y = mouse.Y;
            }
            foreach(var v in vertices)
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
                Vertex selected = vertices.FirstOrDefault(v => v.Selected);
                if(selected != null)
                {
                    selected.Fixed = !selected.Fixed;
                }
            }
            else if(e.KeyCode == Keys.D)
            {
                Vertex selected = vertices.FirstOrDefault(v => v.Selected);
                if (selected != null)
                {
                    // selected.Neighbours.
                }
            }
        }

        public void Render(Graphics g)
        {
            foreach (var v in vertices)
            {
                RenderVertex(v, 10, (x,y,s)=> {
                    if(v.Selected)
                        g.FillEllipse(Brushes.White, x, y, s, s);
                    else
                        g.DrawEllipse(Pens.White, x, y, s, s);
                });
                if (v.Fixed)
                {
                    RenderVertex(v, 14, (x, y, s) => {
                        g.DrawRectangle(Pens.White, x, y, s, s);
                    });
                }
                foreach (var edge in v.Neighbours)
                {
                    var u = edge.GetOpposite(v);
                    g.DrawLine(Pens.White, v.Position, u.Position);
                }
            }
            if(connector.Selected != null)
                RenderVertex(connector.Selected, 14, (x, y, s) => {
                    g.DrawEllipse(Pens.White, x, y, s, s);
                });
        }

        private void RenderVertex(Vertex v, float size, Action<float, float, float> render)
        {
            float ex = v.Position.X - size / 2f;
            float ey = v.Position.Y - size / 2f;
            render(ex, ey, size);
            /*
            if (fill)
                g.FillEllipse(Brushes.White, ex, ey, size, size);
            else
                g.DrawEllipse(Pens.White, ex, ey, size, size);
                */
        }

        internal void MouseDown(MouseEventArgs e)
        {
            mouse = e;
            if(e.Button == MouseButtons.Right)
            {
                if(vertices.FirstOrDefault(v => v.Selected) != null || connector.Selected != null)
                    connector.Connect();
                else
                    AddVertex(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left)
            {
                var selected = vertices.FirstOrDefault(v => v.Selected);
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
                    Selected = simulation.vertices.FirstOrDefault(v => v.Selected);
                }
                else
                {
                    Vertex selected2 = simulation.vertices.FirstOrDefault(v => v.Selected);
                    if(selected2 != null && selected2 != Selected)
                    {
                        Selected.AddEdge(selected2);
                        
                    }
                    Selected = null;
                }
            }
        }
        Connector connector;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Gravitation = false;

        internal void MouseMove(MouseEventArgs e)
        {
            mouse = e;
        }

        internal void MouseUp(MouseEventArgs e)
        {
            mouse = e;
            foreach (var v in vertices)
                v.Dragging = false;
        }

        public void AddVertex(float x, float y)
        {
            vertices.Add(new Vertex(this, x, y));
        }


    }
}