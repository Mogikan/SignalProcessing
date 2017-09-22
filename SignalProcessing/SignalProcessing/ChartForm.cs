using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SignalProcessing
{
    public partial class ChartForm : Form
    {
        public ChartForm(IEnumerable<Tuple<double,double>> values,string xTitle,string yTitle,string title)
        {
            InitializeComponent();
            this.Text = title;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisX.Title = xTitle;
            chart1.ChartAreas[0].AxisY.Title = yTitle;
            values.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
        }
    }
}
