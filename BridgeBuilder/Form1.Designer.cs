namespace BridgeBuilder
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.gravitationToggle = new System.Windows.Forms.CheckBox();
            this.stressToggle = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.snapToggle = new System.Windows.Forms.CheckBox();
            this.pauseToggle = new System.Windows.Forms.CheckBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.weightUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.speedUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.roadToggle = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1002, 446);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(940, 543);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // gravitationToggle
            // 
            this.gravitationToggle.AutoSize = true;
            this.gravitationToggle.Location = new System.Drawing.Point(6, 65);
            this.gravitationToggle.Name = "gravitationToggle";
            this.gravitationToggle.Size = new System.Drawing.Size(75, 17);
            this.gravitationToggle.TabIndex = 2;
            this.gravitationToggle.Text = "gravitation";
            this.gravitationToggle.UseVisualStyleBackColor = true;
            this.gravitationToggle.CheckedChanged += new System.EventHandler(this.gravitationToggle_CheckedChanged);
            // 
            // stressToggle
            // 
            this.stressToggle.AutoSize = true;
            this.stressToggle.Location = new System.Drawing.Point(6, 19);
            this.stressToggle.Name = "stressToggle";
            this.stressToggle.Size = new System.Drawing.Size(81, 17);
            this.stressToggle.TabIndex = 7;
            this.stressToggle.Text = "show stress";
            this.stressToggle.UseVisualStyleBackColor = true;
            this.stressToggle.CheckedChanged += new System.EventHandler(this.stressToggle_CheckedChanged);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(87, 19);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(6, 19);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 9;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // snapToggle
            // 
            this.snapToggle.AutoSize = true;
            this.snapToggle.Location = new System.Drawing.Point(114, 19);
            this.snapToggle.Name = "snapToggle";
            this.snapToggle.Size = new System.Drawing.Size(80, 17);
            this.snapToggle.TabIndex = 10;
            this.snapToggle.Text = "align to grid";
            this.snapToggle.UseVisualStyleBackColor = true;
            this.snapToggle.CheckedChanged += new System.EventHandler(this.snapToggle_CheckedChanged);
            // 
            // pauseToggle
            // 
            this.pauseToggle.AutoSize = true;
            this.pauseToggle.Location = new System.Drawing.Point(6, 42);
            this.pauseToggle.Name = "pauseToggle";
            this.pauseToggle.Size = new System.Drawing.Size(104, 17);
            this.pauseToggle.TabIndex = 11;
            this.pauseToggle.Text = "pause simulation";
            this.pauseToggle.UseVisualStyleBackColor = true;
            this.pauseToggle.CheckedChanged += new System.EventHandler(this.pauseToggle_CheckedChanged);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(6, 71);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 12;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(6, 71);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 13;
            this.testButton.Text = "Run test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // weightUpDown
            // 
            this.weightUpDown.Location = new System.Drawing.Point(74, 45);
            this.weightUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.weightUpDown.Name = "weightUpDown";
            this.weightUpDown.Size = new System.Drawing.Size(120, 20);
            this.weightUpDown.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "weight";
            // 
            // speedUpDown
            // 
            this.speedUpDown.Location = new System.Drawing.Point(74, 17);
            this.speedUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.speedUpDown.Name = "speedUpDown";
            this.speedUpDown.Size = new System.Drawing.Size(120, 20);
            this.speedUpDown.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "speed";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stressToggle);
            this.groupBox1.Controls.Add(this.pauseToggle);
            this.groupBox1.Controls.Add(this.gravitationToggle);
            this.groupBox1.Controls.Add(this.snapToggle);
            this.groupBox1.Location = new System.Drawing.Point(13, 465);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.testButton);
            this.groupBox2.Controls.Add(this.speedUpDown);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.weightUpDown);
            this.groupBox2.Location = new System.Drawing.Point(220, 466);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Testing";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.loadButton);
            this.groupBox3.Controls.Add(this.saveButton);
            this.groupBox3.Controls.Add(this.clearButton);
            this.groupBox3.Location = new System.Drawing.Point(427, 466);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 100);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scene";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.roadToggle);
            this.groupBox4.Location = new System.Drawing.Point(634, 466);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 100);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Interaction";
            // 
            // roadToggle
            // 
            this.roadToggle.AutoSize = true;
            this.roadToggle.Location = new System.Drawing.Point(7, 18);
            this.roadToggle.Name = "roadToggle";
            this.roadToggle.Size = new System.Drawing.Size(81, 17);
            this.roadToggle.TabIndex = 0;
            this.roadToggle.Text = "place roads";
            this.roadToggle.UseVisualStyleBackColor = true;
            this.roadToggle.CheckedChanged += new System.EventHandler(this.roadToggle_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 577);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.CheckBox gravitationToggle;
        private System.Windows.Forms.CheckBox stressToggle;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.CheckBox snapToggle;
        private System.Windows.Forms.CheckBox pauseToggle;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.NumericUpDown weightUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown speedUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox roadToggle;
    }
}

