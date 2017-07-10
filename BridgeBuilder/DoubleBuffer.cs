using System.Drawing;

namespace BridgeBuilder
{
    internal class DoubleBuffer
    {
        private readonly Bitmap[] bitmaps;
        private readonly Graphics[] graphics;

        private int current;

        public Bitmap CurrentBitmap => bitmaps[current];

        public Graphics CurrentGraphics => graphics[current];

        public DoubleBuffer(int width, int height)
        {
            bitmaps = new Bitmap[2];
            graphics = new Graphics[2];

            for (int i = 0; i < 2; i++)
            {
                bitmaps[i] = new Bitmap(width, height);
                graphics[i] = Graphics.FromImage(bitmaps[i]);
            }
        }

        public void Switch()
        {
            current = 1 - current;
        }
    }
}