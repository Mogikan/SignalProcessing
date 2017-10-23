namespace SignalProcessing
{
    partial class SignalForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rejectorRadio = new System.Windows.Forms.RadioButton();
            this.stripeRadio = new System.Windows.Forms.RadioButton();
            this.highRadio = new System.Windows.Forms.RadioButton();
            this.lowRadio = new System.Windows.Forms.RadioButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.samplesCount = new System.Windows.Forms.Label();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.nNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButterworth = new System.Windows.Forms.RadioButton();
            this.radioChebyshev2 = new System.Windows.Forms.RadioButton();
            this.radioChebyshev1 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.polesNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumeric)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.polesNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Src";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1025, 632);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(1043, 191);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 22);
            this.button3.TabIndex = 6;
            this.button3.Text = "Filter";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rejectorRadio);
            this.groupBox1.Controls.Add(this.stripeRadio);
            this.groupBox1.Controls.Add(this.highRadio);
            this.groupBox1.Controls.Add(this.lowRadio);
            this.groupBox1.Location = new System.Drawing.Point(1046, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(76, 109);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "filter";
            // 
            // rejectorRadio
            // 
            this.rejectorRadio.AutoSize = true;
            this.rejectorRadio.Location = new System.Drawing.Point(2, 86);
            this.rejectorRadio.Name = "rejectorRadio";
            this.rejectorRadio.Size = new System.Drawing.Size(60, 17);
            this.rejectorRadio.TabIndex = 0;
            this.rejectorRadio.TabStop = true;
            this.rejectorRadio.Text = "rejector";
            this.rejectorRadio.UseVisualStyleBackColor = true;
            this.rejectorRadio.Visible = false;
            // 
            // stripeRadio
            // 
            this.stripeRadio.AutoSize = true;
            this.stripeRadio.Location = new System.Drawing.Point(2, 66);
            this.stripeRadio.Name = "stripeRadio";
            this.stripeRadio.Size = new System.Drawing.Size(50, 17);
            this.stripeRadio.TabIndex = 0;
            this.stripeRadio.TabStop = true;
            this.stripeRadio.Text = "stripe";
            this.stripeRadio.UseVisualStyleBackColor = true;
            this.stripeRadio.Visible = false;
            // 
            // highRadio
            // 
            this.highRadio.AutoSize = true;
            this.highRadio.Location = new System.Drawing.Point(2, 43);
            this.highRadio.Name = "highRadio";
            this.highRadio.Size = new System.Drawing.Size(45, 17);
            this.highRadio.TabIndex = 0;
            this.highRadio.TabStop = true;
            this.highRadio.Text = "high";
            this.highRadio.UseVisualStyleBackColor = true;
            // 
            // lowRadio
            // 
            this.lowRadio.AutoSize = true;
            this.lowRadio.Location = new System.Drawing.Point(2, 20);
            this.lowRadio.Name = "lowRadio";
            this.lowRadio.Size = new System.Drawing.Size(41, 17);
            this.lowRadio.TabIndex = 0;
            this.lowRadio.TabStop = true;
            this.lowRadio.Text = "low";
            this.lowRadio.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Location = new System.Drawing.Point(1040, 232);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(73, 20);
            this.numericUpDown1.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1043, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "low";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown2.Location = new System.Drawing.Point(1040, 271);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(73, 20);
            this.numericUpDown2.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1043, 294);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Poles";
            // 
            // samplesCount
            // 
            this.samplesCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplesCount.AutoSize = true;
            this.samplesCount.Location = new System.Drawing.Point(1043, 60);
            this.samplesCount.Name = "samplesCount";
            this.samplesCount.Size = new System.Drawing.Size(31, 13);
            this.samplesCount.TabIndex = 10;
            this.samplesCount.Text = "Total";
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(1043, 38);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(51, 13);
            this.fileNameLabel.TabIndex = 10;
            this.fileNameLabel.Text = "FileName";
            // 
            // nNumeric
            // 
            this.nNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nNumeric.Location = new System.Drawing.Point(1042, 358);
            this.nNumeric.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nNumeric.Name = "nNumeric";
            this.nNumeric.Size = new System.Drawing.Size(73, 20);
            this.nNumeric.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1043, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "high";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(1042, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Load wav";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1037, 490);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "AFC";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButterworth);
            this.groupBox2.Controls.Add(this.radioChebyshev2);
            this.groupBox2.Controls.Add(this.radioChebyshev1);
            this.groupBox2.Location = new System.Drawing.Point(1036, 384);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(89, 100);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter";
            // 
            // radioButterworth
            // 
            this.radioButterworth.AutoSize = true;
            this.radioButterworth.Location = new System.Drawing.Point(6, 19);
            this.radioButterworth.Name = "radioButterworth";
            this.radioButterworth.Size = new System.Drawing.Size(79, 17);
            this.radioButterworth.TabIndex = 0;
            this.radioButterworth.TabStop = true;
            this.radioButterworth.Text = "Butterworth";
            this.radioButterworth.UseVisualStyleBackColor = true;
            // 
            // radioChebyshev2
            // 
            this.radioChebyshev2.AutoSize = true;
            this.radioChebyshev2.Location = new System.Drawing.Point(6, 65);
            this.radioChebyshev2.Name = "radioChebyshev2";
            this.radioChebyshev2.Size = new System.Drawing.Size(84, 17);
            this.radioChebyshev2.TabIndex = 0;
            this.radioChebyshev2.TabStop = true;
            this.radioChebyshev2.Text = "Chebyshev2";
            this.radioChebyshev2.UseVisualStyleBackColor = true;
            // 
            // radioChebyshev1
            // 
            this.radioChebyshev1.AutoSize = true;
            this.radioChebyshev1.Location = new System.Drawing.Point(6, 42);
            this.radioChebyshev1.Name = "radioChebyshev1";
            this.radioChebyshev1.Size = new System.Drawing.Size(84, 17);
            this.radioChebyshev1.TabIndex = 0;
            this.radioChebyshev1.TabStop = true;
            this.radioChebyshev1.Text = "Chebyshev1";
            this.radioChebyshev1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1037, 519);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "K(w) FC";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1036, 548);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(89, 23);
            this.button5.TabIndex = 15;
            this.button5.Text = "Chebyshev1";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1040, 621);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(83, 23);
            this.button6.TabIndex = 16;
            this.button6.Text = "Compare";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1043, 342);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "N";
            // 
            // polesNumeric
            // 
            this.polesNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.polesNumeric.Location = new System.Drawing.Point(1042, 310);
            this.polesNumeric.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.polesNumeric.Name = "polesNumeric";
            this.polesNumeric.Size = new System.Drawing.Size(73, 20);
            this.polesNumeric.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Input";
            // 
            // SignalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1129, 648);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.fileNameLabel);
            this.Controls.Add(this.samplesCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.polesNumeric);
            this.Controls.Add(this.nNumeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chart1);
            this.Name = "SignalForm";
            this.Text = "SignalForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumeric)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.polesNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rejectorRadio;
        private System.Windows.Forms.RadioButton stripeRadio;
        private System.Windows.Forms.RadioButton highRadio;
        private System.Windows.Forms.RadioButton lowRadio;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label samplesCount;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.NumericUpDown nNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButterworth;
        private System.Windows.Forms.RadioButton radioChebyshev2;
        private System.Windows.Forms.RadioButton radioChebyshev1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown polesNumeric;
        private System.Windows.Forms.Label label5;
    }
}