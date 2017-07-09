﻿using System;
using System.Drawing;
using System.Linq;

namespace BridgeBuilder
{
    class InteractionRenderer : Renderer
    {
        private Interaction interaction;

        private Pen dashedPen;

        public InteractionRenderer(Interaction interaction)
        {
            this.interaction = interaction;

            float[] dashValues = { 2, 4 };
            dashedPen = new Pen(Color.White, 1);
            dashedPen.DashPattern = dashValues;
        }

        public override void Render(Graphics g)
        {
            Vertex First = interaction.Connector.First;
            if (First != null)
            {
                RenderVertex(First.Position, 14, (x, y, s) =>
                {
                    g.DrawEllipse(Pens.White, x, y, s, s);
                });

                if (interaction.Connector.Selected)
                {
                    PointF candidate;
                    if (interaction.Hover.Any())
                    {
                        candidate = interaction.Hover.First().Position;
                    }
                    else
                    {
                        candidate = interaction.Connector.GetCandidate(interaction.MousePosition);
                    }

                    if (interaction.Connector.CanConnect(candidate))
                        g.DrawLine(dashedPen, candidate, First.Position);
                    RenderVertex(candidate, 10, (x, y, s) =>
                    {
                        g.DrawEllipse(dashedPen, x, y, s, s);
                    });
                }
            }

            if (interaction.AddingVertices)
                RenderVertex(interaction.StickyMousePosition, 10, (x, y, s) =>
                {
                    g.DrawEllipse(dashedPen, x, y, s, s);
                });
        }
    }
}