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
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.loadDataButton = new System.Windows.Forms.Button();
            this.fDftButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dftButton = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // loadDataButton
            // 
            this.loadDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadDataButton.Location = new System.Drawing.Point(882, 12);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(74, 23);
            this.loadDataButton.TabIndex = 1;
            this.loadDataButton.Text = "LoadData";
            this.loadDataButton.UseVisualStyleBackColor = true;
            this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
            // 
            // fDftButton
            // 
            this.fDftButton.Location = new System.Drawing.Point(883, 123);
            this.fDftButton.Name = "fDftButton";
            this.fDftButton.Size = new System.Drawing.Size(75, 23);
            this.fDftButton.TabIndex = 3;
            this.fDftButton.Text = "FastDFT";
            this.fDftButton.UseVisualStyleBackColor = true;
            this.fDftButton.Click += new System.EventHandler(this.fdftClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(883, 152);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "FDFTStep";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dftButton
            // 
            this.dftButton.Location = new System.Drawing.Point(882, 94);
            this.dftButton.Name = "dftButton";
            this.dftButton.Size = new System.Drawing.Size(75, 23);
            this.dftButton.TabIndex = 3;
            this.dftButton.Text = "DFT";
            this.dftButton.UseVisualStyleBackColor = true;
            this.dftButton.Click += new System.EventHandler(this.dftClick);
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Src";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(863, 575);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(882, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 38);
            this.button1.TabIndex = 5;
            this.button1.Text = "Del Garmonics";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(879, 334);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 22);
            this.button3.TabIndex = 6;
            this.button3.Text = "Filter";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rejectorRadio);
            this.groupBox1.Controls.Add(this.stripeRadio);
            this.groupBox1.Controls.Add(this.highRadio);
            this.groupBox1.Controls.Add(this.lowRadio);
            this.groupBox1.Location = new System.Drawing.Point(880, 225);
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
            this.numericUpDown1.Location = new System.Drawing.Point(883, 381);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(73, 20);
            this.numericUpDown1.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(880, 362);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "low";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(883, 428);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(73, 20);
            this.numericUpDown2.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(881, 453);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "n";
            // 
            // samplesCount
            // 
            this.samplesCount.AutoSize = true;
            this.samplesCount.Location = new System.Drawing.Point(882, 42);
            this.samplesCount.Name = "samplesCount";
            this.samplesCount.Size = new System.Drawing.Size(31, 13);
            this.samplesCount.TabIndex = 10;
            this.samplesCount.Text = "Total";
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(882, 66);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(51, 13);
            this.fileNameLabel.TabIndex = 10;
            this.fileNameLabel.Text = "FileName";
            // 
            // nNumeric
            // 
            this.nNumeric.Location = new System.Drawing.Point(884, 474);
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
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(881, 412);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "high";
            // 
            // SignalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 599);
            this.Controls.Add(this.fileNameLabel);
            this.Controls.Add(this.samplesCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nNumeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dftButton);
            this.Controls.Add(this.fDftButton);
            this.Controls.Add(this.loadDataButton);
            this.Name = "SignalForm";
            this.Text = "SignalForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button loadDataButton;
        private System.Windows.Forms.Button fDftButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button dftButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button button1;
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
    }
}