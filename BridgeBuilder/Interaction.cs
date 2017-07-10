using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BridgeBuilder
{
    internal class Interaction : INotifyPropertyChanged
    {
        private readonly Simulation simulation;

        public VertexConnector Connector;

        // body, nad kterými je nyní myš
        public IEnumerable<Vertex> Hover { get; private set; } = Enumerable.Empty<Vertex>();
        // hrany, nad kterými je myš
        public IEnumerable<Edge> HoverEdges { get; private set; } = Enumerable.Empty<Edge>();
        // body, které jsou nyní přetahovány myší
        public IEnumerable<Vertex> Dragging { get; private set; } = Enumerable.Empty<Vertex>();

        public bool SnapToGrid { get; set; } = true;
        public int GridSize = 10;
        public PointF StickyMousePosition;
        public PointF MousePosition;

        private bool placingRoads;
        public bool PlacingRoads
        {
            get { return placingRoads; }
            set
            {
                placingRoads = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlacingRoads"));
            }
        }

        private bool fixingVertices;
        public bool FixingVertices
        {
            get { return fixingVertices; }
            set
            {
                fixingVertices = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FixingVertices"));
            }
        }

        private bool addingVertices;
        public bool AddingVertices
        {
            get { return addingVertices; }
            set
            {
                addingVertices = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddingVertices"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Interaction(Simulation simulation)
        {
            this.simulation = simulation;
            Connector = new VertexConnector(this);
        }

        public class VertexConnector
        {
            public Vertex First;
            private readonly Interaction interaction;
            public float MaxBoxDistance = 80;
            public float MaxDiagDistance = 120;

            public bool Selected => First != null;

            public VertexConnector(Interaction interaction)
            {
                this.interaction = interaction;
            }

            public void Click(IEnumerable<Vertex> clicked)
            {
                // ještě není vybraný žádný bod
                if (First == null)
                {
                    // vybere bod, na který uživatel kliknul
                    First = clicked.FirstOrDefault();
                }
                else // je vybraný první vertex, vybíráme druhý
                {
                    Vertex second = clicked.FirstOrDefault(v => FirstCanConnect(v.Position));
                    if (second != null && second != First)
                    {
                        interaction.AddEdge(First, second);
                        First = second;
                    }
                    else
                    {
                        First = clicked.FirstOrDefault();
                    }
                }
            }

            public void ConnectFirstWith(Vertex second)
            {
                if (First == null)
                    return;

                if (second != First && FirstCanConnect(second.Position))
                {
                    interaction.AddEdge(First, second);
                }
                First = second;
            }

            public void Cancel()
            {
                First = null;
            }

            public bool FirstCanConnect(PointF point)
            {
                if (First == null)
                    return false;
                return CanConnect(First.Position, point);
            }

            public bool CanConnect(PointF p1, PointF p2)
            {
                // rezerva, když se vrcholy maličko pohnou při simulaci
                float mbd = MaxBoxDistance + 2;
                float mdd = MaxDiagDistance + 2;

                // původně tu byl prostě check na vzdálenost dvou bodů,
                // což je i zřetelnější v kódu. Po vzoru Bridge Building Game
                // jsem ale maximální délku trámu udělal závislou na směru
                // nyní maximální délka neopisuje kružnici, ale osmiúhelník
                
                // čtverec
                PointF delta = p1.Sub(p2);
                delta.X = Math.Abs(delta.X);
                delta.Y = Math.Abs(delta.Y);
                if (delta.X > mbd || delta.Y > mbd)
                    return false;
                // kosočtverec
                float manhattanDist = delta.X + delta.Y;
                return manhattanDist <= mdd;
            }

            public PointF GetCandidate(PointF point)
            {
                if (!Selected) return point;
                PointF delta = point.Sub(First.Position);
                PointF cand = delta;

                float mbd = MaxBoxDistance;
                float mdd = MaxDiagDistance;

                // rezervy pro jednotlivé módy. Je to humus, ale funguje to pak uživatelsky přívětivěji
                if (interaction.SnapToGrid)
                {
                    mbd += interaction.GridSize / 2 - 1;
                    mdd += interaction.GridSize / 2 - 1;
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
                    cand = new PointF(Math.Sign(cand.X) * mbd, Utils.Clamp(mbd / Math.Abs(cand.X) * cand.Y, -mbd, mbd));
                if (Math.Abs(cand.Y) > mbd)
                    cand = new PointF(Utils.Clamp(mbd / Math.Abs(cand.Y) * cand.X, -mbd, mbd), Math.Sign(cand.Y) * mbd);

                if (interaction.SnapToGrid)
                {

                    // přichytávání k mřížce
                    float fractX = First.Position.X - interaction.FloorSnap(First.Position.X);
                    float fractY = First.Position.Y - interaction.FloorSnap(First.Position.Y);
                    cand.X = interaction.TruncateSnap(cand.X + fractX) - fractX;
                    cand.Y = interaction.TruncateSnap(cand.Y + fractY) - fractY;

                    // versus přichytávání relativně k označenému bodu
                    // cand.X = interaction.TruncateSnap(cand.X);
                    // cand.Y = interaction.TruncateSnap(cand.Y);
                }

                return First.Position.Add(cand);
            }
        }


        private void AddEdge(Vertex first, Vertex second)
        {
            Edge edge = simulation.AddEdge(first, second);
            edge.IsRoad = PlacingRoads;
        }

        private float RoundSnap(float x)
        {
            return (float)Math.Round(x / GridSize) * GridSize;
        }

        private float FloorSnap(float x)
        {
            return (float)Math.Floor(x / GridSize) * GridSize;
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

        internal void KeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
                FixingVertices = !FixingVertices;
            if (e.KeyCode == Keys.A)
                AddingVertices = !AddingVertices;

            if (Control.ModifierKeys != Keys.Shift)
                PlacingRoads = false;
        }

        internal void KeyDown(KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
                PlacingRoads = true;
        }

        internal void MouseDown(MouseEventArgs e)
        {
            // LEVÉ TLAČÍTKO
            if (e.Button == MouseButtons.Left)
            {
                // pokud uživatel fixuje body, neděláme nic jiného
                if (FixingVertices && Hover.Any())
                {
                    foreach (Vertex v in Hover) v.Fixed = !v.Fixed;
                    return;
                }

                // podobně pokud přidáváme body do prostoru, neděláme nic jiného
                if (AddingVertices)
                {
                    simulation.AddVertex(StickyMousePosition.X, StickyMousePosition.Y);
                    return;
                }

                // levé tlačítko myši přesouvá body
                if (!Connector.Selected)
                    Dragging = Hover.ToList(); // copy selected

                // také vytváří nové spoje (pokud neběží simulace)
                if (simulation.Pause)
                {
                    // pokud máme označený bod a nad žádným dalším bodem nedržíme myš
                    if (!Hover.Any() && Connector.Selected)
                    {
                        // získáme vhodnou pozici pro nový bod (nejblíže k myši od označeného bodu)
                        PointF candidatePosition = Connector.GetCandidate(MousePosition);
                        // PointF candidatePosition = StickyMousePosition;
                        // pokud existuje nějaký bod poblíž vhodné pozice
                        IEnumerable<Vertex> nearCandidate = simulation.Vertices.Where(v => candidatePosition.Sub(v.Position).Mag() < v.Radius);
                        if (nearCandidate.Any())
                        {
                            // tak spojíme tento blízký bod a označený bod
                            Connector.ConnectFirstWith(nearCandidate.First());
                        }
                        else
                        {
                            // jinak vytvoříme bod nový
                            Vertex vertex = simulation.AddVertex(candidatePosition.X, candidatePosition.Y);
                            Connector.ConnectFirstWith(vertex);
                        }
                    }
                    // pokud držíme nad nějakým bodem myš
                    if (Hover.Any())
                    {
                        Connector.Click(Hover);
                    }
                }
            }
            // PRAVÉ TLAČÍTKO
            if (e.Button == MouseButtons.Right)
            {
                // jen pokud není už selektlý bod. Bylo to otravné
                if (!Connector.Selected)
                {
                    // smaže hrany poblíž myši
                    if (HoverEdges.Any())
                        simulation.RemoveEdges(HoverEdges);
                    // smaže body poblíž myši
                    if (Hover.Any())
                        simulation.RemoveVertices(Hover);
                }

                // zruší výběr
                Connector.Cancel();
            }
        }

        internal void MouseMove(MouseEventArgs e)
        {
            MousePosition = new PointF(e.X, e.Y);
            StickyMousePosition = Snap(MousePosition);

            Hover = simulation.Vertices.Where(v => MousePosition.Sub(v.Position).Mag() < v.Radius);
            HoverEdges = simulation.Edges.Where(edge =>
            {
                // hledání úseček v dané (či menší) vzdálenosti od bodu
                // https://en.wikipedia.org/wiki/Vector_projection
                // vektor od V do U
                PointF delta = edge.U.Position.Sub(edge.V.Position);
                float length = delta.Mag();
                PointF unit = delta.MultiplyScalar(1f / length);
                // vektor od V do pozice myši
                PointF deltaMouse = MousePosition.Sub(edge.V.Position);
                // skalární projekce - musí být > 0 && < délka úsečky UV
                // jinak je projekce bodu myši mimo tuto úsečku a tedy
                // myš není u úsečky ale někde před ní nebo za ní
                float scalarProjection = unit.Dot(deltaMouse);
                if (scalarProjection < edge.U.Radius || scalarProjection > length - edge.V.Radius)
                    return false;

                // vektorová projekce
                PointF a1 = unit.MultiplyScalar(scalarProjection);
                // rozdíl, tedy kolmý vektor na UV ukazující od úsečky k bodu myši
                PointF a2 = deltaMouse.Sub(a1);
                // "kolmá" vzdálenost úsečky a myši
                float mouseDistance = a2.Mag();
                float maxDistance = Math.Max(edge.U.Radius, edge.V.Radius);
                return mouseDistance < maxDistance;
            });

            foreach (Vertex v in Dragging)
            {
                if (simulation.Pause)
                {
                    // přesouvání bodu probíhá při vypnuté simulaci
                    // přímým nastavením pozice bodu
                    v.Position = StickyMousePosition;
                    v.PrevPos = StickyMousePosition;
                    IEnumerable<Edge> affectedEdges = simulation.GetEdges(v);
                    foreach (Edge edge in affectedEdges)
                        edge.ResetLength();
                }
                else
                {
                    // přesouvání bodu probíhá při běžící simulaci
                    // pomocí aplikací síly
                    v.SetTarget(MousePosition);
                }
            }
        }

        internal void MouseUp(MouseEventArgs e)
        {
            foreach (Vertex v in Dragging) v.ResetTarget();
            Dragging = Enumerable.Empty<Vertex>();
        }
    }
}
