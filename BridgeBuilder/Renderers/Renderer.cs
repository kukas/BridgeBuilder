using System;
using System.Drawing;

namespace BridgeBuilder
{
    internal abstract class Renderer
    {
        public abstract void Render(Graphics g);

        /// <summary>
        /// Pomocná třída pro kreslení bodu v jeho prostředku
        /// </summary>
        /// <param name="position">Prostřední bod vykreslovaného objektu</param>
        /// <param name="size">Průměr vykreslovaného objektu</param>
        /// <param name="render">Funkce pro vykreslování</param>
        protected void RenderVertex(PointF position, float size, Action<float, float, float> render)
        {
            float ex = position.X - size / 2f;
            float ey = position.Y - size / 2f;
            render(ex, ey, size);
        }
    }
}