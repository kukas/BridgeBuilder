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
        public IEnumerable<Vertex> Hover { get; private set; } = Enumerable.Empty<Vertex>();
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
            public float MaxBoxDistance = 80;
            public float MaxDiagDistance = 120;

            public bool Selected { get { return First != null; } }

            public VertexConnector(Interaction interaction)
            {
                this.interaction = interaction;
            }

            public void Connect()
            {
                if (interaction.Hover.Any())
                {
                    // nevybraný žádný vertex
                    if (First == null)
                    {
                        First = interaction.Hover.First();
                    }
                    else // je vybraný první vertex, vybíráme druhý
                    {
                        Vertex Second = interaction.Hover.FirstOrDefault(v => CanConnect(v.Position));
                        if (Second != null && Second != First)
                        {
                            interaction.simulation.AddEdge(First,Second);
                            First = Second;
                        }
                        else
                        {
                            First = interaction.Hover.FirstOrDefault();
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

                if (Second != First && CanConnect(Second.Position))
                {
                    interaction.simulation.AddEdge(First, Second);
                }
                First = Second;
            }

            public void Cancel()
            {
                First = null;
            }

            public bool CanConnect(PointF point)
            {
                if (First == null)
                    return false;
                return CanConnect(First.Position, point);
            }

            public bool CanConnect(PointF p1, PointF p2)
            {
                PointF delta = p1.Sub(p2);
                delta.X = Math.Abs(delta.X);
                delta.Y = Math.Abs(delta.Y);
                float manhattanDist = delta.X + delta.Y;
                return manhattanDist <= MaxDiagDistance && delta.X <= MaxBoxDistance && delta.Y <= MaxBoxDistance;
            }

            public PointF GetCandidate(PointF point)
            {
                if (!Selected) return point;
                PointF delta = point.Sub(First.Position);
                PointF cand = delta;

                float mbd = MaxBoxDistance;
                float mdd = MaxDiagDistance;

                if (interaction.SnapToGrid)
                {
                    mbd += interaction.GridSize/2-1;
                    mdd += interaction.GridSize/2-1;
                }
                else
                {
                    mbd -= 0.1f;
                    mdd -= 0.1f;
                }

                // Kosočtverec
                float t = 1;
                if (cand.X + cand.Y > mdd && cand.X > 0 && cand.Y > 0)
                    t = mdd / (delta.Y + delta.X);
                if (-cand.X - cand.Y > mdd && cand.X < 0 && cand.Y < 0)
                    t = -mdd / (delta.Y + delta.X);
                if (-cand.X + cand.Y > mdd && cand.X < 0 && cand.Y > 0)
                    t = mdd / (delta.Y - delta.X);
                if (cand.X - cand.Y > mdd && cand.X > 0 && cand.Y < 0)
                    t = -mdd / (delta.Y - delta.X);
                cand.X *= t;
                cand.Y *= t;

                // Čtverec
                if (Math.Abs(cand.X) > mbd)
                    cand = new PointF(Math.Sign(cand.X)* mbd, Utils.Clamp(mbd / Math.Abs(cand.X) * cand.Y, -mbd, mbd));
                if (Math.Abs(cand.Y) > mbd)
                    cand = new PointF(Utils.Clamp(mbd / Math.Abs(cand.Y) * cand.X, -mbd, mbd), Math.Sign(cand.Y) * mbd);

                if (interaction.SnapToGrid)
                {
                    
                    // přichytávání k mřížce
                    float fractX = First.Position.X - interaction.FloorSnap(First.Position.X);
                    float fractY = First.Position.Y - interaction.FloorSnap(First.Position.Y);
                    cand.X = interaction.TruncateSnap(cand.X + fractX)-fractX;
                    cand.Y = interaction.TruncateSnap(cand.Y + fractY)-fractY;
                    
                    // versus přichytávání relativně k označenému bodu
                    // cand.X = interaction.TruncateSnap(cand.X);
                    // cand.Y = interaction.TruncateSnap(cand.Y);
                }

                return First.Position.Add(cand);
            }
        }

        private float RoundSnap(float x)
        {
            return (float)Math.Round(x / GridSize) * GridSize;
        }

        private float FloorSnap(float x)
        {
            return (float)Math.Floor(x / GridSize) * GridSize;
        }

        private float CeilSnap(float x)
        {
            return (float)Math.Ceiling(x / GridSize) * GridSize;
        }


        private float TruncateSnap(float x)
        {
            return (float)Math.Truncate(x / GridSize) * GridSize;
        }

        private PointF Snap(PointF position)
        {
            if (!SnapToGrid)
                return position;
            float x = RoundSnap(position.X);
            float y = RoundSnap(position.Y);
            return new PointF(x, y);
        }

        internal void KeyPress(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {
                foreach (var v in Hover) v.Fixed = !v.Fixed;
            }
        }

        internal void MouseDown(MouseEventArgs e)
        {
            mouse = e;

            // LEVÉ TLAČÍTKO
            if (e.Button == MouseButtons.Left)
            {
                // levé tlačítko myši přesouvá body
                Dragging = Hover.ToList(); // copy selected

                // také vytváří nové spoje
                if(Connector.Selected && !Hover.Any()) // pokud máme označený bod a nad žádným dalším bodem nedržíme myš
                {
                    // získáme vhodnou pozici pro nový bod (nejblíže k myši od označeného bodu)
                    PointF candidatePosition = Connector.GetCandidate(MousePosition);
                    // PointF candidatePosition = StickyMousePosition;
                    // pokud existuje nějaký bod poblíž vhodné pozice
                    var nearCandidate = simulation.Vertices.Where(v => { return candidatePosition.Sub(v.Position).Mag() < v.Radius; });
                    if (nearCandidate.Any())
                    {
                        // tak spojíme tento blízký bod a označený bod
                        Connector.Connect(nearCandidate.First());
                    }
                    else
                    {
                        // jinak vytvoříme bod nový
                        Vertex vertex = simulation.AddVertex(candidatePosition.X, candidatePosition.Y);
                        Connector.Connect(vertex);
                    }
                    
                }
                else // pokud nemáme označený bod nebo držíme nad nějakým myš
                {
                    // zkusíme označit
                    Connector.Connect();
                }
            }
            // PRAVÉ TLAČÍTKO
            if (e.Button == MouseButtons.Right)
            {
                // smaže vybrané hrany
                if(SelectedEdges.Any())
                    simulation.RemoveEdges(SelectedEdges);
                // zruší výběr
                Connector.Cancel();
            }
        }

        internal void MouseMove(MouseEventArgs e)
        {
            mouse = e;
            MousePosition = new PointF(e.X, e.Y);
            StickyMousePosition = Snap(MousePosition);

            Hover = simulation.Vertices.Where(v => { return MousePosition.Sub(v.Position).Mag() < v.Radius; });
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
