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
        public IEnumerable<Edge> SelectedEdges { get; private set; } = Enumerable.Empty<Edge>();
        public IEnumerable<Vertex> Dragging { get; private set; } = Enumerable.Empty<Vertex>();
        public bool SnapToGrid = false;
        public int GridSize = 10;

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
                            interaction.simulation.AddEdge(First,Second);
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
                {
                    var MousePosition = new PointF(e.X, e.Y);
                    if (SnapToGrid)
                        MousePosition = Snap(MousePosition);
                    simulation.AddVertex(MousePosition.X, MousePosition.Y);
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                Dragging = Selected.ToList(); // copy selected
                simulation.RemoveEdges(SelectedEdges);
            }
        }

        private PointF Snap(PointF position)
        {
            float x = (float)Math.Round((double)position.X / GridSize) * GridSize;
            float y = (float)Math.Round((double)position.Y / GridSize) * GridSize;
            return new PointF(x, y);
        }

        internal void MouseMove(MouseEventArgs e)
        {
            mouse = e;
            var MousePosition = new PointF(e.X, e.Y);
            Selected = simulation.Vertices.Where(v => { return MousePosition.Sub(v.Position).Mag() < v.Radius; });
            SelectedEdges = simulation.Edges.Where(edge => {
                // https://en.wikipedia.org/wiki/Vector_projection
                PointF delta = edge.U.Position.Sub(edge.V.Position);
                float length = delta.Mag();
                PointF unit = delta.MultiplyScalar(1f/length);
                PointF deltaMouse = MousePosition.Sub(edge.V.Position);
                float scalarProjection = unit.Dot(deltaMouse);
                PointF a1 = unit.MultiplyScalar(scalarProjection);
                PointF a2 = deltaMouse.Sub(a1);
                float mouseDistance = a2.Mag();
                float maxDistance = Math.Max(edge.U.Radius, edge.V.Radius);
                return scalarProjection > edge.U.Radius && scalarProjection < length - edge.V.Radius && mouseDistance < maxDistance;
            });

            foreach (var v in Dragging)
            {
                if (simulation.Pause)
                {
                    if (SnapToGrid)
                    {
                        v.Position = Snap(MousePosition);
                        v.PrevPos = Snap(MousePosition);
                    }
                    else
                    {
                        v.Position = MousePosition;
                        v.PrevPos = MousePosition;
                    }
                    IEnumerable<Edge> affectedEdges = simulation.GetEdges(v);
                    foreach (var edge in affectedEdges)
                        edge.ResetLength();
                }
                else
                {
                    v.SetTarget(MousePosition);
                }
            }
        }

        internal void MouseUp(MouseEventArgs e)
        {
            mouse = e;
            foreach (var v in Dragging) v.ResetTarget();
            Dragging = Enumerable.Empty<Vertex>();
        }
    }
}
