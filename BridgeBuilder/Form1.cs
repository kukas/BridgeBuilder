using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BridgeBuilder
{
    public partial class Form1 : Form
    {
        Point mouse;

        Thread rendererThread;
        Thread simulationThread;
        private bool rendering = true;

        Simulation simulation;
        FpsMeter fps = new FpsMeter();

        int width, height;

        DoubleBuffer db;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            width = pictureBox1.Width;
            height = pictureBox1.Height;
            // canvas = new Bitmap(width, height);
            // g = Graphics.FromImage(canvas);
            mouse = new Point();

            simulation = new Simulation(width, height);
            numericUpDown2.DataBindings.Add("Value", simulation, "Damping", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDown1.DataBindings.Add("Value", simulation, "Stiffness", true, DataSourceUpdateMode.OnPropertyChanged);

            db = new DoubleBuffer(width, height);

            rendererThread = new Thread(RenderLoop);
            simulationThread = new Thread(UpdateLoop);
            rendererThread.Start();
            simulationThread.Start();
        }

        private void RenderLoop()
        {
            while (rendering)
            {
                Bitmap canvas = db.CurrentBitmap;
                Graphics g = db.CurrentGraphics;

                g.Clear(Color.Black);
                simulation.Render(g);
                
                double currentFps = Math.Round(fps.Frame(), 1);
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 150, 30));
                g.DrawString($"fps: {currentFps}", new Font("Arial", 16), new SolidBrush(Color.White), 10, 10);

                if (pictureBox1.InvokeRequired)
                {
                    
                    IAsyncResult action = BeginInvoke(new Action(() =>
                    {
                        pictureBox1.Image = canvas;
                        pictureBox1.Invalidate();
                    }));

                    action.AsyncWaitHandle.WaitOne(20);
                    
                }
                else
                {
                    pictureBox1.Image = canvas;
                    pictureBox1.Invalidate();
                }

                db.Switch();
            }
            
        }

        private void UpdateLoop()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long last = 0;
            while (rendering)
            {
                long now = sw.ElapsedMilliseconds;
                simulation.Update((now - last)/1000f);
                last = now;
            }
            sw.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rendering = false;
            if(rendererThread != null)
                rendererThread.Join();

            Close();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            simulation.MouseDown(e);
            //simulation.AddVertex(e.X, e.Y);
            //e.Button
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // canvas.DrawLine(Pens.Black, new Point(10, 10), new Point(e.X, e.Y));
            simulation.MouseMove(e);
        }

        private void Form1_KeyPress(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("keyup");
            simulation.KeyPress(e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            simulation.Gravitation = checkBox1.Checked;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            simulation.MouseUp(e);
        }
    }
}
