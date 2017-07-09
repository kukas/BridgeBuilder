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
                g.DrawEllipse(p, testingStress.Position.X, testingStress.Position.Y, 10, 10);
            }
        }
    }
}