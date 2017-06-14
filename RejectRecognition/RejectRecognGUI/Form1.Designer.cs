namespace RejectRecognGUI
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cameraFeed1 = new Emgu.CV.UI.ImageBox();
            this.snapshot1 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.LabelBrightness = new System.Windows.Forms.Label();
            this.LabelFPS = new System.Windows.Forms.Label();
            this.LabelContrast = new System.Windows.Forms.Label();
            this.numericContrast = new System.Windows.Forms.NumericUpDown();
            this.numericBrightness = new System.Windows.Forms.NumericUpDown();
            this.numericFPS = new System.Windows.Forms.NumericUpDown();
            this.comboCameras = new System.Windows.Forms.ComboBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelPostionXY = new System.Windows.Forms.Label();
            this.polzun = new System.Windows.Forms.TrackBar();
            this.polzunLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.polzun)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraFeed1
            // 
            this.cameraFeed1.Location = new System.Drawing.Point(12, 12);
            this.cameraFeed1.Name = "cameraFeed1";
            this.cameraFeed1.Size = new System.Drawing.Size(640, 480);
            this.cameraFeed1.TabIndex = 0;
            this.cameraFeed1.TabStop = false;
            this.cameraFeed1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cameraFeed1_MouseDown);
            this.cameraFeed1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cameraFeed1_MouseMove);
            this.cameraFeed1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cameraFeed1_MouseUp);
            // 
            // snapshot1
            // 
            this.snapshot1.Location = new System.Drawing.Point(12, 498);
            this.snapshot1.Name = "snapshot1";
            this.snapshot1.Size = new System.Drawing.Size(75, 23);
            this.snapshot1.TabIndex = 2;
            this.snapshot1.Text = "Снимок";
            this.snapshot1.UseVisualStyleBackColor = true;
            this.snapshot1.Click += new System.EventHandler(this.snapshot1_Click);
            // 
            // LabelBrightness
            // 
            this.LabelBrightness.AutoSize = true;
            this.LabelBrightness.Location = new System.Drawing.Point(246, 501);
            this.LabelBrightness.Name = "LabelBrightness";
            this.LabelBrightness.Size = new System.Drawing.Size(50, 13);
            this.LabelBrightness.TabIndex = 4;
            this.LabelBrightness.Text = "Яркость";
            // 
            // LabelFPS
            // 
            this.LabelFPS.AutoSize = true;
            this.LabelFPS.Location = new System.Drawing.Point(259, 527);
            this.LabelFPS.Name = "LabelFPS";
            this.LabelFPS.Size = new System.Drawing.Size(37, 13);
            this.LabelFPS.TabIndex = 4;
            this.LabelFPS.Text = "Кдр/с";
            // 
            // LabelContrast
            // 
            this.LabelContrast.AutoSize = true;
            this.LabelContrast.Location = new System.Drawing.Point(221, 555);
            this.LabelContrast.Name = "LabelContrast";
            this.LabelContrast.Size = new System.Drawing.Size(83, 13);
            this.LabelContrast.TabIndex = 4;
            this.LabelContrast.Text = "Контрастность";
            // 
            // numericContrast
            // 
            this.numericContrast.Location = new System.Drawing.Point(302, 553);
            this.numericContrast.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericContrast.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericContrast.Name = "numericContrast";
            this.numericContrast.Size = new System.Drawing.Size(55, 20);
            this.numericContrast.TabIndex = 5;
            this.numericContrast.ValueChanged += new System.EventHandler(this.numericContrast_ValueChanged);
            // 
            // numericBrightness
            // 
            this.numericBrightness.Location = new System.Drawing.Point(302, 501);
            this.numericBrightness.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericBrightness.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericBrightness.Name = "numericBrightness";
            this.numericBrightness.Size = new System.Drawing.Size(55, 20);
            this.numericBrightness.TabIndex = 5;
            this.numericBrightness.ValueChanged += new System.EventHandler(this.numericBrightness_ValueChanged);
            // 
            // numericFPS
            // 
            this.numericFPS.Location = new System.Drawing.Point(302, 527);
            this.numericFPS.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericFPS.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericFPS.Name = "numericFPS";
            this.numericFPS.Size = new System.Drawing.Size(55, 20);
            this.numericFPS.TabIndex = 5;
            this.numericFPS.ValueChanged += new System.EventHandler(this.numericFPS_ValueChanged);
            // 
            // comboCameras
            // 
            this.comboCameras.FormattingEnabled = true;
            this.comboCameras.Location = new System.Drawing.Point(527, 498);
            this.comboCameras.Name = "comboCameras";
            this.comboCameras.Size = new System.Drawing.Size(121, 21);
            this.comboCameras.TabIndex = 6;
            this.comboCameras.SelectedIndexChanged += new System.EventHandler(this.comboCameras_SelectedIndexChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 527);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(98, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelPostionXY
            // 
            this.labelPostionXY.AutoSize = true;
            this.labelPostionXY.Location = new System.Drawing.Point(130, 522);
            this.labelPostionXY.Name = "labelPostionXY";
            this.labelPostionXY.Size = new System.Drawing.Size(0, 13);
            this.labelPostionXY.TabIndex = 8;
            this.labelPostionXY.Visible = false;
            // 
            // polzun
            // 
            this.polzun.Location = new System.Drawing.Point(400, 534);
            this.polzun.Maximum = 255;
            this.polzun.Minimum = 1;
            this.polzun.Name = "polzun";
            this.polzun.Size = new System.Drawing.Size(252, 45);
            this.polzun.TabIndex = 9;
            this.polzun.TabStop = false;
            this.polzun.Value = 1;
            this.polzun.ValueChanged += new System.EventHandler(this.polzun_ValueChanged);
            // 
            // polzunLabel
            // 
            this.polzunLabel.AutoSize = true;
            this.polzunLabel.Enabled = false;
            this.polzunLabel.Location = new System.Drawing.Point(400, 507);
            this.polzunLabel.Name = "polzunLabel";
            this.polzunLabel.Size = new System.Drawing.Size(24, 13);
            this.polzunLabel.TabIndex = 10;
            this.polzunLabel.Text = "test";
            this.polzunLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 591);
            this.Controls.Add(this.polzunLabel);
            this.Controls.Add(this.polzun);
            this.Controls.Add(this.labelPostionXY);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.comboCameras);
            this.Controls.Add(this.numericBrightness);
            this.Controls.Add(this.numericFPS);
            this.Controls.Add(this.numericContrast);
            this.Controls.Add(this.LabelContrast);
            this.Controls.Add(this.LabelFPS);
            this.Controls.Add(this.LabelBrightness);
            this.Controls.Add(this.snapshot1);
            this.Controls.Add(this.cameraFeed1);
            this.Name = "Form1";
            this.Text = "ReconGUI";
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.polzun)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox cameraFeed1;
        private System.Windows.Forms.Button snapshot1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label LabelBrightness;
        private System.Windows.Forms.Label LabelFPS;
        private System.Windows.Forms.Label LabelContrast;
        private System.Windows.Forms.NumericUpDown numericContrast;
        private System.Windows.Forms.NumericUpDown numericBrightness;
        private System.Windows.Forms.NumericUpDown numericFPS;
        private System.Windows.Forms.ComboBox comboCameras;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelPostionXY;
        private System.Windows.Forms.TrackBar polzun;
        private System.Windows.Forms.Label polzunLabel;
    }
}

