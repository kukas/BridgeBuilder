using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public VertexConnector Connector;
        public IEnumerable<Vertex> Selected { get; private set; } = Enumerable.Empty<Vertex>();
        public IEnumerable<Edge> SelectedEdges { get; private set; } = Enumerable.Empty<Edge>();
        public IEnumerable<Vertex> Dragging { get; private set; } = Enumerable.Empty<Vertex>();
        public bool SnapToGrid = false;
        public int GridSize = 10;
        public PointF StickyMousePosition = new PointF();
        public PointF MousePosition = new PointF();

        public Interaction(Simulation simulation)
        {
            this.simulation = simulation;
            Connector = new VertexConnector(this);
        }

        public class VertexConnector
        {
            public Vertex First;
            private Interaction interaction;
            public float MaxDistance = 59;

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
                        Vertex Second = interaction.Selected.FirstOrDefault(v => CanConnect(v.Position));
                        if (Second != null && Second != First)
                        {
                            interaction.simulation.AddEdge(First,Second);
                            First = Second;
                        }
                        else
                        {
                            First = interaction.Selected.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    // pokud uživatel kliknul vedle, výběr se zruší
                    First = null;
                }
            }

            public void Connect(Vertex Second)
            {
                if (First == null)
                    return;

                if (Second != First)
                {
                    interaction.simulation.AddEdge(First, Second);
                }
                First = Second;
            }

            public bool CanConnect(PointF point)
            {
                if (First == null)
                    return false;
                PointF delta = First.Position.Sub(point);
                return delta.MagSq() <= MaxDistance * MaxDistance;
            }

            public void Cancel()
            {
                First = null;
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

            if (e.Button == MouseButtons.Left)
            {
                Dragging = Selected.ToList(); // copy selected

                if(Connector.CanConnect(StickyMousePosition) && !Selected.Any())
                {
                    Debug.WriteLine("can connect");
                    Vertex vertex = simulation.AddVertex(StickyMousePosition.X, StickyMousePosition.Y);
                    Connector.Connect(vertex);
                }
                else
                {
                    Connector.Connect();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                simulation.RemoveEdges(SelectedEdges);
                Connector.Cancel();
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
            MousePosition = new PointF(e.X, e.Y);
            if (SnapToGrid)
                StickyMousePosition = Snap(MousePosition);
            else
                StickyMousePosition = MousePosition;

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
                    v.Position = StickyMousePosition;
                    v.PrevPos = StickyMousePosition;
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
