using System.Drawing;
using System.Linq;

namespace BridgeBuilder
{
    internal class InteractionRenderer : Renderer
    {
        private readonly Interaction interaction;

        private readonly Pen dashedPen;

        public InteractionRenderer(Interaction interaction)
        {
            this.interaction = interaction;

            float[] dashValues = { 2, 4 };
            dashedPen = new Pen(Color.White, 1) {DashPattern = dashValues};
        }

        public override void Render(Graphics g)
        {
            Vertex first = interaction.Connector.First;
            // pokud je vybraný nějaký bod
            if (interaction.Connector.Selected)
            {
                // nakreslí kolem něj kolečko
                RenderVertex(first.Position, 14, (x, y, s) =>
                {
                    g.DrawEllipse(Pens.White, x, y, s, s);
                });

                // nalezne vhodný bod pro spojení s vybraným
                // a vykreslí ho čárkovaně
                PointF candidate;
                if (interaction.Hover.Any()) // pokud myš ukazuje na existující bod, vybere ten
                    candidate = interaction.Hover.First().Position;
                else // pokud ne, vybere nový
                    candidate = interaction.Connector.GetCandidate(interaction.MousePosition);

                // linku zobrazí jen, pokud body lze spojit
                if (interaction.Connector.FirstCanConnect(candidate))
                    g.DrawLine(dashedPen, candidate, first.Position);

                RenderVertex(candidate, 10, (x, y, s) =>
                {
                    g.DrawEllipse(dashedPen, x, y, s, s);
                });
            }

            // při přidávání nového bodu vykreslí čárkovaně bod na aktuální pozici
            if (interaction.AddingVertices)
                RenderVertex(interaction.StickyMousePosition, 10, (x, y, s) =>
                {
                    g.DrawEllipse(dashedPen, x, y, s, s);
                });
        }
    }
}