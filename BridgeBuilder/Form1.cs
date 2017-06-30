using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BridgeBuilder
{
    public partial class Form1 : Form
    {
        private Thread rendererThread;
        private Thread simulationThread;
        private bool running = true;

        private Simulation simulation;
        private FpsMeter fps = new FpsMeter();
        private DoubleBuffer db;
        private SimulationRenderer simulationRenderer;
        private Interaction interaction;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true; // needed for keyPress event to work

            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            simulation = new Simulation(width, height);
            interaction = new Interaction(simulation);
            simulationRenderer = new SimulationRenderer(simulation, interaction);
            

            // databinding for faster and easier parameter tuning
            numericUpDown2.DataBindings.Add("Value", simulation, "Damping", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDown1.DataBindings.Add("Value", simulation, "Stiffness", true, DataSourceUpdateMode.OnPropertyChanged);

            db = new DoubleBuffer(width, height);

            rendererThread = new Thread(RenderLoop);
            rendererThread.Start();

            simulationThread = new Thread(UpdateLoop);
            simulationThread.Start();
        }

        private void RenderLoop()
        {
            while (running)
            {
                Bitmap canvas = db.CurrentBitmap;
                Graphics g = db.CurrentGraphics;

                g.Clear(Color.Black);
                simulationRenderer.Render(g);
                
                // draw debugging FPS info
                double currentFps = Math.Round(fps.FPS, 1);
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 150, 30));
                g.DrawString($"fps: {currentFps}", new Font("Arial", 16), new SolidBrush(Color.White), 10, 10);

                // invokeRequired check needed for changing GUI from different thread
                if (pictureBox1.InvokeRequired)
                {
                    IAsyncResult switchingAction = BeginInvoke(new Action(() =>
                    {
                        pictureBox1.Image = canvas;
                        pictureBox1.Invalidate();
                    }));

                    switchingAction.AsyncWaitHandle.WaitOne(20); // 20ms hard limit for waiting for switchingAction to complete
                }
                else
                {
                    pictureBox1.Image = canvas;
                    pictureBox1.Invalidate();
                }

                // switch canvases (double buffering)
                db.Switch();

                // sleep to prevent unnecessary rendering
                Thread.Sleep(fps.Next());
            }
        }

        private void UpdateLoop()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double last = 0;
            int precision = 1;
            while (running)
            {
                double now = sw.Elapsed.TotalMilliseconds;
                double dt = (now - last);
                for (int i = 0; i < precision; i++)
                {
                    simulation.Update(0.001f);
                }
                last = now;
                Thread.Sleep(1);
            }
            sw.Stop();
        }

        // mouse events
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            interaction.MouseDown(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            interaction.MouseMove(e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            interaction.MouseUp(e);
        }

        // keyboard events
        private void Form1_KeyPress(object sender, KeyEventArgs e)
        {
            interaction.KeyPress(e);
        }

        // GUI interaction events
        private void button1_Click(object sender, EventArgs e)
        {
            running = false;
            if(rendererThread != null)
                rendererThread.Join();

            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            simulation.Gravitation = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            simulationRenderer.RenderStrain = checkBox2.Checked;
        }
    }
}
