using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace BridgeBuilder
{
    public partial class BridgeBuilder : Form
    {
        private Thread rendererThread;
        private Thread simulationThread;
        private bool running = true;
        private double simulationTime = 0;

        private Simulation simulation;
        private FpsMeter fps = new FpsMeter();
        private DoubleBuffer db;

        private Interaction interaction;
        private TestingStress testingStress;

        private SimulationRenderer simulationRenderer;
        private InteractionRenderer interactionRenderer;
        private TestingStressRenderer testingStressRenderer;

        public BridgeBuilder()
        {
            InitializeComponent();
            KeyPreview = true; // needed for keyPress event to work

            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            simulation = new Simulation(width, height);
            interaction = new Interaction(simulation);
            testingStress = new TestingStress(simulation);
            simulationRenderer = new SimulationRenderer(simulation, interaction);
            interactionRenderer = new InteractionRenderer(interaction);
            testingStressRenderer = new TestingStressRenderer(testingStress);

            // one-way databinding (hodnoty nezmění nic jiného než GUI)
            speedUpDown.DataBindings.Add("Value", testingStress, "Speed", true, DataSourceUpdateMode.OnPropertyChanged);
            weightUpDown.DataBindings.Add("Value", testingStress, "Weight", true, DataSourceUpdateMode.OnPropertyChanged);

            pauseToggle.DataBindings.Add("Checked", simulation, "Pause", true, DataSourceUpdateMode.OnPropertyChanged);
            gravitationToggle.DataBindings.Add("Checked", simulation, "Gravitation", true, DataSourceUpdateMode.OnPropertyChanged);
            stressToggle.DataBindings.Add("Checked", simulationRenderer, "RenderStrain", true, DataSourceUpdateMode.OnPropertyChanged);
            snapToggle.DataBindings.Add("Checked", interaction, "SnapToGrid", true, DataSourceUpdateMode.OnPropertyChanged);

            Binding bind = new Binding("Enabled", simulation, "Pause");
            bind.Format += (sender, e) => { e.Value = !((bool)e.Value); };
            testButton.DataBindings.Add(bind);

            // two-way databinding
            roadToggle.DataBindings.Add("Checked", interaction, "PlacingRoads", true, DataSourceUpdateMode.OnPropertyChanged);
            fixingToggle.DataBindings.Add("Checked", interaction, "FixingVertices", true, DataSourceUpdateMode.OnPropertyChanged);
            addVerticesToggle.DataBindings.Add("Checked", interaction, "AddingVertices", true, DataSourceUpdateMode.OnPropertyChanged);

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
                interactionRenderer.Render(g);
                testingStressRenderer.Render(g);

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
            int precision = 1;
            int maxCycles = 10;
            float dt = 0.004f;
            float dtSimulation = dt / precision;
            while (running)
            {
                Simulation s = simulation;
                double now = sw.Elapsed.TotalMilliseconds / 1000f;

                int cycle = 0;
                while (simulationTime < now && ++cycle < maxCycles)
                {
                    for (int i = 0; i < precision; i++)
                    {
                        s.Update(dtSimulation);
                        testingStress.Update(dtSimulation);
                    }
                    simulationTime += dt;
                }
                // Debug.WriteLine(cycle);
                Thread.Sleep(4);
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
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            interaction.KeyUp(e);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            interaction.KeyDown(e);
        }

        // GUI interaction events
        private void exitButton_Click(object sender, EventArgs e)
        {
            running = false;
            if (rendererThread != null)
                rendererThread.Join();

            Close();
        }

        // scene actions
        private void saveButton_Click(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("bridge.bin",
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, simulation);
            stream.Close();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream("bridge.bin",
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
                Simulation loadedSimulation = (Simulation)formatter.Deserialize(stream);
                simulation.LoadSimulation(loadedSimulation);
                stream.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Bridge file error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            simulation.Clear();
        }
        // bridge testing
        private void testButton_Click(object sender, EventArgs e)
        {
            bool success = testingStress.StartTest();
            if(!success)
                MessageBox.Show("There are no roads on the scene. Testing cancelled.", "Bridge Testing problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
