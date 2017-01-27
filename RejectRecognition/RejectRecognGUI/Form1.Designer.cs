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
            this.cameraFeed1 = new Emgu.CV.UI.ImageBox();
            this.cameraFeed2 = new Emgu.CV.UI.ImageBox();
            this.snapshot1 = new System.Windows.Forms.Button();
            this.snapshot2 = new System.Windows.Forms.Button();
            this.buttonDetect = new System.Windows.Forms.Button();
            this.colourBox = new System.Windows.Forms.PictureBox();
            this.onoffText = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colourBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraFeed1
            // 
            this.cameraFeed1.Location = new System.Drawing.Point(12, 12);
            this.cameraFeed1.Name = "cameraFeed1";
            this.cameraFeed1.Size = new System.Drawing.Size(425, 260);
            this.cameraFeed1.TabIndex = 0;
            this.cameraFeed1.TabStop = false;
            // 
            // cameraFeed2
            // 
            this.cameraFeed2.Location = new System.Drawing.Point(447, 12);
            this.cameraFeed2.Name = "cameraFeed2";
            this.cameraFeed2.Size = new System.Drawing.Size(425, 260);
            this.cameraFeed2.TabIndex = 1;
            this.cameraFeed2.TabStop = false;
            // 
            // snapshot1
            // 
            this.snapshot1.Location = new System.Drawing.Point(12, 278);
            this.snapshot1.Name = "snapshot1";
            this.snapshot1.Size = new System.Drawing.Size(75, 23);
            this.snapshot1.TabIndex = 2;
            this.snapshot1.Text = "Snapshot";
            this.snapshot1.UseVisualStyleBackColor = true;
            this.snapshot1.Click += new System.EventHandler(this.snapshot1_Click);
            // 
            // snapshot2
            // 
            this.snapshot2.Location = new System.Drawing.Point(447, 278);
            this.snapshot2.Name = "snapshot2";
            this.snapshot2.Size = new System.Drawing.Size(75, 23);
            this.snapshot2.TabIndex = 3;
            this.snapshot2.Text = "Snapshot";
            this.snapshot2.UseVisualStyleBackColor = true;
            this.snapshot2.Click += new System.EventHandler(this.snapshot2_Click);
            // 
            // buttonDetect
            // 
            this.buttonDetect.Location = new System.Drawing.Point(12, 329);
            this.buttonDetect.Name = "buttonDetect";
            this.buttonDetect.Size = new System.Drawing.Size(75, 23);
            this.buttonDetect.TabIndex = 4;
            this.buttonDetect.Text = "Различия";
            this.buttonDetect.UseVisualStyleBackColor = true;
            this.buttonDetect.Click += new System.EventHandler(this.buttonDetect_Click);
            // 
            // colourBox
            // 
            this.colourBox.BackColor = System.Drawing.Color.Yellow;
            this.colourBox.Location = new System.Drawing.Point(107, 329);
            this.colourBox.Name = "colourBox";
            this.colourBox.Size = new System.Drawing.Size(23, 22);
            this.colourBox.TabIndex = 5;
            this.colourBox.TabStop = false;
            // 
            // onoffText
            // 
            this.onoffText.AutoSize = true;
            this.onoffText.Location = new System.Drawing.Point(137, 335);
            this.onoffText.Name = "onoffText";
            this.onoffText.Size = new System.Drawing.Size(37, 13);
            this.onoffText.TabIndex = 7;
            this.onoffText.Text = "Выкл.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(402, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "0%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(851, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "0%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 390);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.onoffText);
            this.Controls.Add(this.colourBox);
            this.Controls.Add(this.buttonDetect);
            this.Controls.Add(this.snapshot2);
            this.Controls.Add(this.snapshot1);
            this.Controls.Add(this.cameraFeed2);
            this.Controls.Add(this.cameraFeed1);
            this.Name = "Form1";
            this.Text = "ХЛЪБ";
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraFeed2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colourBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox cameraFeed1;
        private Emgu.CV.UI.ImageBox cameraFeed2;
        private System.Windows.Forms.Button snapshot1;
        private System.Windows.Forms.Button snapshot2;
        private System.Windows.Forms.Button buttonDetect;
        private System.Windows.Forms.PictureBox colourBox;
        private System.Windows.Forms.Label onoffText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

