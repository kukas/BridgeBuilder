using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BridgeBuilder
{
    class Interaction
    {
        private Simulation simulation;
        private MouseEventArgs mouse;
        // private PointF mousePosition = new PointF();

        public VertexConnector Connector;
        public IEnumerable<Vertex> Selected { get; private set; } = Enumerable.Empty<Vertex>();
        public IEnumerable<Vertex> Dragging { get; private set; } = Enumerable.Empty<Vertex>();

        public Interaction(Simulation simulation)
        {
            this.simulation = simulation;
            Connector = new VertexConnector(this);
        }

        public class VertexConnector
        {
            public Vertex First;
            private Interaction interaction;

            public VertexConnector(Interaction interaction)
            {
                this.interaction = interaction;
            }

            public void Connect()
            {
                if (interaction.Selected.Any())
                {
                    // nevybraný žádný vertex
                    if (First == null)
                    {
                        First = interaction.Selected.First();
                    }
                    else // je vybraný první vertex, vybíráme druhý
                    {
                        Vertex Second = interaction.Selected.First();
                        if (Second != null && Second != First)
                        {
                            First.AddEdge(Second);
                        }
                        First = null;
                    }
                }
                else
                {
                    // pokud uživatel kliknul vedle, výběr se zruší
                    First = null;
                }
                
            }
        }

        internal void KeyPress(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {
                foreach (var v in Selected) v.Fixed = !v.Fixed;
            }
        }

        internal void MouseDown(MouseEventArgs e)
        {
            mouse = e;
            if (e.Button == MouseButtons.Right)
            {
                if (Selected.Any() || Connector.First != null)
                    Connector.Connect();
                else
                    simulation.AddVertex(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left)
            {
                Dragging = Selected.ToList(); // copy selected
            }
        }

        internal void MouseMove(MouseEventArgs e)
        {
            mouse = e;
            var MousePosition = new PointF(e.X, e.Y);
            Selected = simulation.Vertices.Where(v => { return MousePosition.Sub(v.Position).Mag() < v.Radius; });

            foreach (var v in Dragging) v.SetTarget(MousePosition);
        }

        internal void MouseUp(MouseEventArgs e)
        {
            mouse = e;
            foreach (var v in Dragging) v.ResetTarget();
            Dragging = Enumerable.Empty<Vertex>();
        }
    }
}
