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

        public SimulationRenderer(Simulation simulation, Interaction interaction)
        {
            this.simulation = simulation;
            this.interaction = interaction;
        }

        public void Render(Graphics g)
        {
            foreach (var v in simulation.Vertices)
            {
                RenderVertex(v, 10, (x, y, s) => {
                    if (v.Selected)
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
            /*
            if (connector.Selected != null)
                RenderVertex(connector.Selected, 14, (x, y, s) => {
                    g.DrawEllipse(Pens.White, x, y, s, s);
                });
                */
        }

        private void RenderVertex(Vertex v, float size, Action<float, float, float> render)
        {
            float ex = v.Position.X - size / 2f;
            float ey = v.Position.Y - size / 2f;
            render(ex, ey, size);
        }
    }
}
