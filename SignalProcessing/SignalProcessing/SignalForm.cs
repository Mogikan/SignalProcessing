using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SignalProcessing
{
    public partial class SignalForm : Form
    {
        public SignalForm()
        {
            InitializeComponent();
        }
       
        private class Settings
        {
            public Settings(double baseValue,double step,double xFrequency,string yLabel)
            {
                BaseValue = baseValue;
                Step = step;
                XFrequency = xFrequency;
                YLabel = yLabel;
            }
            public double BaseValue { get; set; }
            public double Step { get; set; }
            public double XFrequency { get; set; }
            public string YLabel { get; set; }
        }

        private const bool ShowHalfSpectre = true;

        private Dictionary<string, Settings> settings = new Dictionary<string, Settings>()
        {
            {"cardio",new Settings(127,60,360,"мВ")},
            {"reo",new Settings(0,50,360,"мОм") },
            {"velo",new Settings(512,120,360 ,"мВ")},
            {"spiro",new Settings(512,100 ,360,"Л")},
            {"sawtooth", new Settings(0,1,360,"") },
            {"triangle", new Settings(0,1,360,"") },
            {"square", new Settings(0,1,360,"") },
        };

        enum Direction
        {
            Forward = -1,
            Inverse = 1,
        }

        private Complex[] DFT(Complex[] inputSignal, Direction direction)
        {
            int N = inputSignal.Length;
            var result = new Complex[inputSignal.Length];            
            for (int k = 0; k < N; k++) // итоговые коэффициенты как сумма
            {
                for (int i = 0; i < N; i++) // внутренний цикл суммирования
                {                    
                    result[k] += inputSignal[i] * Complex.FromPolarCoordinates(1,2.0 * Math.PI * k *(int)direction * i / N);                    
                }
            }
            if (direction == Direction.Forward)
            {
                return result.Select((c) => c / N).ToArray();
            }
            return result;
        }

        private int FindMaxPower(int input)
        {
            return (int)Math.Floor(Math.Log(input) / Math.Log(2));
        }
        private (double[] amplitude,double[] phase) CalculateMetrics(Complex[] signal)
        {
            int N = signal.Length;
            if (ShowHalfSpectre)
            {
                N = N / 2;
            }
            double[] phase = new double[N];
            double[] amplitude = new double[N];
            //double Hypot(double a, double b) => Math.Sqrt(a*a + b*b);
            for (int i = 0; i < N; i++)
            {
                amplitude[i] = signal[i].Magnitude;
                phase[i] = signal[i].Phase;
            }
            return (amplitude,phase);
        }



        private (Settings settings,double[] signal) LoadDataFromFile()
        {
            using (var fileDialog = new OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileName = fileDialog.FileName;
                    using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
                    {
                        chart1.Series[0].Points.Clear();                     
                        var stringValues = reader.ReadToEnd()
                            .Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);                        
                        var fileNameWOExtension = Path.GetFileNameWithoutExtension(fileName);
                        var settingsSelector = Path.GetFileNameWithoutExtension(fileNameWOExtension).Remove(fileNameWOExtension.Length - 2);
                        var selectedSettings = settings[settingsSelector];
                        var signal = stringValues.Select((stringValue)=> (Convert.ToDouble(stringValue) - selectedSettings.BaseValue) / selectedSettings.Step).ToArray();
                        return (selectedSettings, signal);                        
                    }
                }
            }
            return (null, null);
        }


        private double[] _signal;
        private Settings _settings;

        private void loadDataButton_Click(object sender, EventArgs e)
        {
            (_settings, _signal) =LoadDataFromFile();
            if (_signal == null) return;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisX.Title = "t";
            chart1.ChartAreas[0].AxisY.Title = _settings.YLabel;
            var result = new List<Tuple<double, double>>();
            for (int i = 0; i < _signal.Length; i++)
            {
                result.Add(new Tuple<double, double>(i / _settings.XFrequency, _signal[i]));
            }
            result.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
        }

        private void ShowChart(IEnumerable<Tuple<double, double>> values, string xTitle, string yTitle,string title)
        {
            ChartForm chart = new ChartForm(values, xTitle, yTitle,title);
            chart.Show();            
        }

        private void dftClick(object sender, EventArgs e)
        {
            var inputSignal = _signal;
            int maxPower = FindMaxPower(inputSignal.Length);
            var signalCut = inputSignal.Take(1 << maxPower).ToArray();
            ApplyTransform(DFT,signalCut);
        }

        private void ApplyTransform(Func<Complex[], Direction, Complex[]> transformFunction,double[] signal)
        {            
            var settings = _settings;
            if (signal == null) return;
            var chartXLabel = "Гц";
            (double[] amp, double[] phase) = CalculateMetrics(
                transformFunction(signal.Select((s)=>new Complex(s,0)).ToArray(), Direction.Forward));
            var magnitudePoints = amp.Select(
                (a, i) => new Tuple<double, double>(i * settings.XFrequency / amp.Length, amp[i])
                );
            ShowChart(magnitudePoints, chartXLabel, _settings.YLabel,$"Magnitude {transformFunction.Method.Name}");
            var phasePoints = phase.Select((s, i) => new Tuple<double, double>(i * settings.XFrequency / phase.Length, s));
            ShowChart(phasePoints, chartXLabel, _settings.YLabel,$"Phase {transformFunction.Method.Name}");
        }

        private Complex[] FastDFT(Complex[] inputSignal, Direction direction)
        {            
            int N = inputSignal.Length;
            if (N == 1)
                return inputSignal;
            Complex W = 1;
            var C0 = FastDFT(inputSignal.Where((s,i)=>i%2==0).ToArray(),direction);    // F0 = [f0,f2,...,fn-2]  // рекурсия
            var C1 = FastDFT(inputSignal.Where((s,i)=>i%2==1).ToArray(),direction);    //  F1 = [f1,f3,...,fn-1]
            var C = new Complex[N];
            for (var k = 0; k < N / 2; k++)
            {
                var Wn = Complex.FromPolarCoordinates(1, (int)direction * 2 * Math.PI * k / N);
                C1[k] *= Wn;
            }
            for (int k = 0; k < N / 2; k++)
            {
                C[k] = C0[k] + C1[k];
                C[k + N / 2] = C0[k] - C1[k];
            }
            if (direction == Direction.Forward)
            {
                return C.Select((c) => c / N).ToArray();
            }
            return C;

        }

        private void fdftClick(object sender, EventArgs e)
        {
            var inputSignal = _signal;
            int maxPower = FindMaxPower(inputSignal.Length);
            var signalCut = inputSignal.Take(1 << maxPower).ToArray();
            ApplyTransform(DFT,signalCut);
            ApplyTransform(FastDFT,signalCut);
        }
    }
}
