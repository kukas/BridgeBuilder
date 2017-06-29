using System.Drawing;

namespace BridgeBuilder
{
    internal class DoubleBuffer
    {
        private int height;
        private int width;

        Bitmap[] bitmaps;
        Graphics[] graphics;

        int current = 0;

        public Bitmap CurrentBitmap {
            get {
                return bitmaps[current];
            }
        }
        public Graphics CurrentGraphics
        {
            get
            {
                return graphics[current];
            }
        }

        public DoubleBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;

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