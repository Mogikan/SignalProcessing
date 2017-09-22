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
            var multiplier = (1-(N + 1) / 2.0) * (int)direction + ((N+1)/2.0);
            var result = new Complex[inputSignal.Length];            
            for (int k = 0; k < N; k++) // итоговые коэффициенты как сумма
            {
                for (int i = 0; i < N; i++) // внутренний цикл суммирования
                {                    
                    result[k] += inputSignal[i] * Complex.FromPolarCoordinates(1,2.0 * Math.PI * k *(int)direction * i / N)/multiplier;                    
                }
            }
            return result;
        }

        private int FindMaxPower(int input)
        {
            int power = 0x1 << 31;
            int maxPower = 31;
            while ((power&input) == 0)
            {
                maxPower--;
                power = power >> 1;
            }
            return maxPower;
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

        private void ShowChart(IEnumerable<Tuple<double, double>> values, string xTitle, string yTitle)
        {
            ChartForm chart = new ChartForm(values, xTitle, yTitle);
            chart.Show();            
        }

        private void dftClick(object sender, EventArgs e)
        {
            ApplyTransform(DFT);
        }

        private void ApplyTransform(Func<Complex[], Direction, Complex[]> transformFunction)
        {
            var signal = _signal;
            var settings = _settings;
            if (signal == null) return;
            var result = signal.Select((s, i) => new Tuple<double, double>(i / settings.XFrequency, signal[i]));
            var chartXLabel = "Гц";
            int maxPower = FindMaxPower(result.Count());
            int takeOnly = (int)Math.Pow(2, maxPower);
            (double[] amp, double[] spectre) = CalculateMetrics(transformFunction(result.Select((p) => new Complex(p.Item2, 0)).ToArray(), Direction.Forward));
            result = amp.Select((a, i) => new Tuple<double, double>(i * settings.XFrequency / amp.Length, amp[i]));
            ShowChart(result, chartXLabel, _settings.YLabel);
            result = spectre.Select((s, i) => new Tuple<double, double>(i * settings.XFrequency / spectre.Length, spectre[i]));
            ShowChart(result, chartXLabel, _settings.YLabel);
        }

        private Complex[] FastDFT(Complex[] inputSignal, Direction direction)
        {
            var signalCut = inputSignal.Take(1<<FindMaxPower(inputSignal.Length)).ToArray();
            int N = signalCut.Length;
            if (N == 1)
                return signalCut;
            var Wn = Complex.FromPolarCoordinates(1, 2 * Math.PI / N);
            //Wn = exp(2 * Pi * j / n);  // j – мнимая единица
            Complex W = 1;
            var C0 = FastDFT(signalCut.Where((s,i)=>i%2==0).ToArray(),direction);    // F0 = [f0,f2,...,fn-2]  // рекурсия
            var C1 = FastDFT(signalCut.Where((s,i)=>i%2==1).ToArray(),direction);    //  F1 = [f1,f3,...,fn-1]
            var C = new Complex[N];
            for (var k = 0; k < N / 2; k++)
            {
                C[k] = C0[k] + W * C1[k];  // фрмула (3) первая часть преобразования
                C[k + N / 2] = C0[k] - W * C1[k];   // Wk=-Wn/2+k   // свойство комплексной экспоненты
                W += Wn;
            }
            return C;

        }

        private void fdftClick(object sender, EventArgs e)
        {
            ApplyTransform(FastDFT);
        }
    }
}
