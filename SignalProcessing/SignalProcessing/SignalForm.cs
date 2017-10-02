using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
                    fileNameLabel.Text = Path.GetFileName(fileName);
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
            (_settings, _signal) = LoadDataFromFile();
            samplesCount.Text = _signal.Length.ToString();
            if (_signal == null) return;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisX.Title = "t";
            chart1.ChartAreas[0].AxisY.Title = _settings.YLabel;
            Tuple<double, double>[] result = PrepareSignalPreviewData(_signal);
            result.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
        }

        private Tuple<double, double>[] PrepareSignalPreviewData(double[] signal)=>
            signal.Select((s, i) => new Tuple<double, double>(i / _settings.XFrequency, s)).ToArray();

        private Tuple<double, double>[] PrepareSignalPreviewData(Complex[] signal) =>
            signal.Select((s, i) => new Tuple<double, double>(i / _settings.XFrequency, s.Real)).ToArray();



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
            ApplyTransform(DFT, signalCut, Direction.Forward);
        }

        private void ApplyTransform(Func<Complex[], Direction, Complex[]> transformFunction,double[] signal,Direction direction)
        {
            var settings = _settings;
            if (signal == null)
                return;
            var chartXLabel = "Гц";
            var result = transformFunction(GetComplexData(signal), direction);
            (double[] amp, double[] phase) = CalculateMetrics(result);
            var magnitudePoints = amp.Select(
                (a, i) => new Tuple<double, double>(1.0 * i * settings.XFrequency / amp.Length, amp[i])
                );
            ShowChart(magnitudePoints, chartXLabel, _settings.YLabel, $"Magnitude {transformFunction.Method.Name}");
            var phasePoints = phase.Select((s, i) => new Tuple<double, double>(i * settings.XFrequency / phase.Length, s));
            ShowChart(phasePoints, chartXLabel, _settings.YLabel, $"Phase {transformFunction.Method.Name}");            
        }

        private static Complex[] GetComplexData(double[] signal)
        {
            return signal.Select((s) => new Complex(s, 0)).ToArray();
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

        private Complex[] FastDFTN(Complex[] signal, Direction direction)
        {
            int N = signal.Length;
            int maxPower = 10;// 
               // FindMaxPower(N);
            int L = 1 << maxPower;
            int M = N / L;
            N = M * L;
            //int L = N / M;
            //N =L* M, L=2P.
            Complex[] calculationSignal = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                calculationSignal[i] = signal[i];
            }
            for (int i = 0; i < M; i++)
            {
                FastDFTStep(direction,i, M, L,ref calculationSignal); // вызов БПФ для отсчетов шагом M
            }
            var result = new Complex[N];
            for (int s = 0; s < M; s++)
            {
                for (int r = 0; r < L; r++)
                {
                    for (int m = 0; m < M; m++)
                    {
                        result[r + s * L] += calculationSignal[m + r * M] * Complex.FromPolarCoordinates(1,(int)direction * 2 * Math.PI * m * (r + s * L) / N);                        
                    }
                }
            }            
            if (direction == Direction.Forward)
            {
                for (int k = 0; k < N; k++)
                {
                    result[k] = result[k] / N;
                }
            }
            return result;
        }

        private int ReverseIndex(int index, int maxPower)
        {
            int R;
            int Shift;
            int k;
            R = 0;
            Shift = maxPower - 1;
            int Low, High;
            Low = 1;
            High = 1 << Shift;
            while (Shift >= 0)
            {
                k = ((index & Low) << Shift) | ((index & High) >> Shift);
                R = R | k;
                Shift -= 2;
                Low = Low << 1;
                High = High >> 1;
            }
            return R;

        }


        private void FastDFTStep(Direction direction, int Z, int H, int N,ref Complex[] signal)
        { // БПФ с шагом H от отсчета Z
            int M = (int)Math.Log(N,2);
            for (int i = 0; i < N; i++)
            {
                int k = ReverseIndex(i, M);
                int p = Z + i * H;
                int q = Z + k * H;
                if (p < q)
                {
                    Complex temp = signal[p];
                    signal[p] = signal[q];
                    signal[q] = temp;                    
                }
            }
            for (int s = 1; s <= M; s++)
            {
                int m = 1<<s;                
                var Wm = Complex.FromPolarCoordinates(1, 2 * Math.PI * (int)direction / m);
                for (int k = 0; k < N; k += m)
                {
                    Complex W = new Complex(1,0);
                    for (int j = 0; j <= m / 2 - 1; j++)
                    {
                        int p = k + j + m / 2;
                        int q = k + j;
                        p = Z + p * H;
                        q = Z + q * H;
                        Complex t = signal[p] * W;
                        Complex u = signal[q];
                        signal[q] = t + u;
                        signal[p] = u - t;
                        //W = W * Wm;
                        W = W*Wm;                        
                    }
                }
            }
        }


        private void fdftClick(object sender, EventArgs e)
        {
            var inputSignal = _signal;
            int maxPower = FindMaxPower(inputSignal.Length);
            var signalCut = inputSignal.Take(1 << maxPower).ToArray();
            Stopwatch timeChecker = new Stopwatch();
            timeChecker.Start();
            ApplyTransform(DFT,signalCut,Direction.Forward);
            timeChecker.Stop();
            long time1 = timeChecker.ElapsedMilliseconds;
            timeChecker.Restart();
            ApplyTransform(FastDFT,signalCut,Direction.Forward);
            timeChecker.Stop();
            var time2 = timeChecker.ElapsedMilliseconds;
            MessageBox.Show($"DFT time {time1},{Environment.NewLine}FastDFT time {time2}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch timeChecker = new Stopwatch();
            timeChecker.Start();

            ApplyTransform(FastDFTN, _signal,Direction.Forward);
            timeChecker.Stop();
            long time1 = timeChecker.ElapsedMilliseconds;
            timeChecker.Restart();
            ApplyTransform(DFT, _signal, Direction.Forward);
            timeChecker.Stop();
            var time2 = timeChecker.ElapsedMilliseconds;
            MessageBox.Show($"DFT with step time {time1},{Environment.NewLine}DFT time {time2}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var transformedSignal = DFT(GetComplexData(_signal), Direction.Forward);
            var filteredGarmonics = new Complex[transformedSignal.Length];// (Complex[])transformedSignal.Clone();
            for (int i = 0; i < 6; i++)
            {
                filteredGarmonics[i] = transformedSignal[i];
                filteredGarmonics[transformedSignal.Length - 1 - i] = transformedSignal[transformedSignal.Length - 1 - i];
            }            
            var backSignal = DFT(filteredGarmonics, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "5 Гармоник");


            filteredGarmonics = new Complex[transformedSignal.Length];
            for (int i = 0; i < 30; i++)
            {
                filteredGarmonics[i] = transformedSignal[i];
                filteredGarmonics[transformedSignal.Length - 1 - i] = transformedSignal[transformedSignal.Length - 1 - i];
            }
            backSignal = DFT(filteredGarmonics, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "30 Гармоник");

        }

        private Complex[] LowFrequenciesFilter(Complex[] signal, int lowThresholdIndex)
        {
            for (int i = lowThresholdIndex+1; i<signal.Length-lowThresholdIndex; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private Complex[] HighFrequenciesFilter(Complex[] signal, int highThresholdIndex)
        {
            for (int i = 0; i < highThresholdIndex; i++)
            {
                signal[i] = 0;
            }
            for (int i = signal.Length - highThresholdIndex ; i < signal.Length; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private Complex[] RejectorFilter(Complex[] signal,int low, int high)
        {
            for (int i = low; i < high; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private Complex[] StripeFilter(Complex[] signal, int low, int high)
        {
            for (int i = 0; i < low; i++)
            {
                signal[i] = 0;
            }
            for (int i = high; i < signal.Length; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lowRadio.Checked)
            {
                var transformedSignal = DFT(GetComplexData(_signal), Direction.Forward);                
                var backSignal = DFT(LowFrequenciesFilter(transformedSignal,(int)numericUpDown1.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "Low frequencies filter");
            }
            if (highRadio.Checked)
            {
                var transformedSignal = DFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = DFT(HighFrequenciesFilter(transformedSignal, (int)numericUpDown2.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "High frequencies filter");
            }
            if (stripeRadio.Checked)
            {
                var transformedSignal = DFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = DFT(StripeFilter(transformedSignal, (int)numericUpDown1.Value,(int)numericUpDown2.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "Stripe frequencies filter");
            }
            if (rejectorRadio.Checked)
            {
                var transformedSignal = DFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = DFT(RejectorFilter(transformedSignal, (int)numericUpDown1.Value, (int)numericUpDown2.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal), "", _settings.YLabel, "Rejector frequencies filter");
            }
        }
    }
}
