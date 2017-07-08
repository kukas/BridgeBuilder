﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    class SimulationRenderer
    {
        private Interaction interaction;
        private Simulation simulation;

        private Pallete straincolor;

        public bool RenderStrain = false;

        public SimulationRenderer(Simulation simulation, Interaction interaction)
        {
            this.simulation = simulation;
            this.interaction = interaction;

            straincolor = new Pallete();
        }

        class Pallete {
            // http://stackoverflow.com/questions/15868234/map-a-value-0-0-1-0-to-color-gain
            // paleta z: http://4.bp.blogspot.com/-d96rd-cACn0/TdUINqcBxuI/AAAAAAAAA9I/nGDXL7ksxAc/s1600/01-Deep_Rumba-A_Calm_in_the_Fire_of_Dances_2496-Cubana.flac.png
            double linear(double x, double start, double end)
            {
                if (x < start)
                    return 0;
                else if (x > end)
                    return 1;
                else
                    return (x - start) / (end - start);
            }

            double getR(double value)
            {
                return linear(value, 25.0 / 200.0, 140.0 / 200.0);
            }
            double getG(double value)
            {
                return linear(value, 120.0 / 200.0, 180.0 / 200.0);
            }
            double getB(double value)
            {
                return linear(value, 0.75, 1.0) + (linear(value, 0, 57.0 / 200.0) - linear(value, 63.0 / 200.0, 120.0 / 200.0)) * 0.5;
                // return 1.0 - linear(value, 0, 0.5);
            }

            public Color GetColor(double v)
            {
                int r = (int)(getR(v)*255);
                int g = (int)(getG(v)*255);
                int b = (int)(getB(v)*255);
                return Color.FromArgb(r, g, b);
            }
        }

        public void Render(Graphics g)
        {
            foreach (var v in simulation.Vertices)
            {
                RenderVertex(v, v.Radius, (x, y, s) =>
                {
                    if (interaction.Selected.Contains(v))
                        g.FillEllipse(Brushes.White, x, y, s, s);
                    else
                        g.DrawEllipse(Pens.White, x, y, s, s);
                });
                if (v.Fixed)
                {
                    RenderVertex(v, 14, (x, y, s) =>
                    {
                        g.DrawRectangle(Pens.White, x, y, s, s);
                    });
                }
                foreach (var edge in v.Neighbours)
                {
                    var u = edge.GetOpposite(v);
                    Pen p = Pens.White;
                    if (RenderStrain)
                    {
                        float maxStrain = 8;
                        var strain = Math.Min(Math.Abs(edge.Length - u.Position.Sub(v.Position).Mag()), maxStrain);
                        p = new Pen(straincolor.GetColor(strain / maxStrain), 2);
                    }
                    g.DrawLine(p, v.Position, u.Position);
                }
            }

            if (interaction.Connector.First != null)
            {
                RenderVertex(interaction.Connector.First, 14, (x, y, s) =>
                {
                    g.DrawEllipse(Pens.White, x, y, s, s);
                });
            }
        }

        private void RenderVertex(Vertex v, float size, Action<float, float, float> render)
        {
            float ex = v.Position.X - size / 2f;
            float ey = v.Position.Y - size / 2f;
            render(ex, ey, size);
        }
    }
}