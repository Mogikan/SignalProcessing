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
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // loadDataButton
            // 
            this.loadDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadDataButton.Location = new System.Drawing.Point(880, 12);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(75, 23);
            this.loadDataButton.TabIndex = 1;
            this.loadDataButton.Text = "LoadData";
            this.loadDataButton.UseVisualStyleBackColor = true;
            this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
            // 
            // fDftButton
            // 
            this.fDftButton.Location = new System.Drawing.Point(881, 68);
            this.fDftButton.Name = "fDftButton";
            this.fDftButton.Size = new System.Drawing.Size(75, 23);
            this.fDftButton.TabIndex = 3;
            this.fDftButton.Text = "FastDFT";
            this.fDftButton.UseVisualStyleBackColor = true;
            this.fDftButton.Click += new System.EventHandler(this.fdftClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(881, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dftButton
            // 
            this.dftButton.Location = new System.Drawing.Point(881, 41);
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
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(863, 575);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // SignalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 599);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dftButton);
            this.Controls.Add(this.fDftButton);
            this.Controls.Add(this.loadDataButton);
            this.Name = "SignalForm";
            this.Text = "SignalForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button loadDataButton;
        private System.Windows.Forms.Button fDftButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button dftButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}