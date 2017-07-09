using System.Drawing;

namespace BridgeBuilder
{
    class TestingStressRenderer : Renderer
    {
        private TestingStress testingStress;

        public TestingStressRenderer(TestingStress testingStress)
        {
            this.testingStress = testingStress;
        }

        public override void Render(Graphics g)
        {

            if (testingStress.Started)
            {
                Pen p = Pens.White;
                float size = (float)testingStress.Weight / 1000f;
                g.DrawEllipse(p, testingStress.Position.X - size / 2, testingStress.Position.Y - size, size, size);
            }
        }
    }
}