using System;
using System.Drawing;

namespace BridgeBuilder
{
    abstract class Renderer
    {
        public abstract void Render(Graphics g);

        protected void RenderVertex(PointF position, float size, Action<float, float, float> render)
        {
            float ex = position.X - size / 2f;
            float ey = position.Y - size / 2f;
            render(ex, ey, size);
        }
    }
}