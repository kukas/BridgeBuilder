using System;
using System.Drawing;
using System.Linq;

namespace BridgeBuilder
{
    internal class SimulationRenderer : Renderer
    {
        private readonly Interaction interaction;
        private readonly Simulation simulation;

        private readonly Pallete straincolor;

        public bool RenderStrain { get; set; } = false;

        public SimulationRenderer(Simulation simulation, Interaction interaction)
        {
            this.simulation = simulation;
            this.interaction = interaction;

            straincolor = new Pallete();
        }

        private class Pallete
        {
            // http://stackoverflow.com/questions/15868234/map-a-value-0-0-1-0-to-color-gain
            // paleta z: http://4.bp.blogspot.com/-d96rd-cACn0/TdUINqcBxuI/AAAAAAAAA9I/nGDXL7ksxAc/s1600/01-Deep_Rumba-A_Calm_in_the_Fire_of_Dances_2496-Cubana.flac.png
            private static double Linear(double x, double start, double end)
            {
                if (x < start)
                    return 0;
                else if (x > end)
                    return 1;
                else
                    return (x - start) / (end - start);
            }

            private static double GetR(double value)
            {
                return Linear(value, 25.0 / 200.0, 140.0 / 200.0);
            }

            private static double GetG(double value)
            {
                return Linear(value, 120.0 / 200.0, 180.0 / 200.0);
            }

            private static double GetB(double value)
            {
                return Linear(value, 0.75, 1.0) + (Linear(value, -0.2, 57.0 / 200.0) - Linear(value, 63.0 / 200.0, 120.0 / 200.0)) * 0.5;
            }

            public Color GetColor(double v)
            {
                int r = (int)(GetR(v) * 255);
                int g = (int)(GetG(v) * 255);
                int b = (int)(GetB(v) * 255);
                return Color.FromArgb(r, g, b);
            }
        }

        public override void Render(Graphics g)
        {
            if (interaction.SnapToGrid)
            {
                Brush b = Brushes.DimGray;
                for (int x = 0; x < simulation.Width; x += interaction.GridSize)
                    for (int y = 0; y < simulation.Height; y += interaction.GridSize)
                        g.FillRectangle(b, x, y, 1, 1);
            }

            foreach (Vertex v in simulation.Vertices)
            {
                RenderVertex(v.Position, v.Radius, (x, y, s) =>
                {
                    if (interaction.Hover.Contains(v))
                        g.FillEllipse(Brushes.White, x, y, s, s);
                    else
                        g.DrawEllipse(Pens.White, x, y, s, s);
                });
                if (v.Fixed)
                {
                    RenderVertex(v.Position, 14, (x, y, s) =>
                    {
                        g.DrawRectangle(Pens.White, x, y, s, s);
                    });
                }
            }
            foreach (Edge e in simulation.Edges)
            {
                Vertex u = e.U;
                Vertex v = e.V;
                Pen p = Pens.White;

                if (RenderStrain)
                {
                    float maxStrain = simulation.MaxStrain;
                    float delta = 1 - e.CurrentLength / e.Length;
                    float strain = Math.Min(Math.Abs(delta), maxStrain);
                    p = new Pen(straincolor.GetColor(strain / maxStrain), 2);

                    // Show strain as text
                    // var pos = u.Position.Add(v.Position).MultiplyScalar(0.5f);
                    // g.DrawString(String.Format("{0:0.00}", strain / maxStrain), new Font("Arial", 8), new SolidBrush(Color.White), pos.X, pos.Y);
                }
                if (interaction.HoverEdges.Contains(e))
                    p = new Pen(Color.White, 2);
                g.DrawLine(p, v.Position, u.Position);

                if (e.IsRoad)
                {
                    PointF delta = new PointF(0, -3);
                    g.DrawLine(p, v.Position.Add(delta), u.Position.Add(delta));
                }
            }
        }
    }
}
