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
            public Settings(double baseValue, double step, double xFrequency, string yLabel)
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
            {"wav",new Settings(0,1,44100,"")},
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
                    result[k] += inputSignal[i] * Complex.FromPolarCoordinates(1, 2.0 * Math.PI * k * (int)direction * i / N);
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
        private (double[] amplitude, double[] phase) CalculateMetrics(Complex[] signal)
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
            return (amplitude, phase);
        }



        private (Settings settings, double[] signal) LoadDataFromFile()
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
                        var signal = stringValues.Select((stringValue) => (Convert.ToDouble(stringValue) - selectedSettings.BaseValue) / selectedSettings.Step).ToArray();
                        return (selectedSettings, signal);
                    }
                }
            }
            return (null, null);
        }


        private double[] _signal;
        private byte[] _header;
        private string _fileName;
        private Settings _settings;

        private void loadDataButton_Click(object sender, EventArgs e)
        {
            (_settings, _signal) = LoadDataFromFile();
            DisplayData(_settings, _signal);
        }

        private void DisplayData(Settings settings, double[] signal, int n = 0)
        {
            if (signal == null) return;
            samplesCount.Text = signal.Length.ToString();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisX.Title = "t";
            chart1.ChartAreas[0].AxisY.Title = settings.YLabel;
            Tuple<double, double>[] result = PrepareSignalPreviewData(signal, settings);
            if (n != 0)
            {
                result.Take(n).ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
            }
            else
            {
                result.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
            }
        }

        private Tuple<double, double>[] PrepareSignalPreviewData(double[] signal, Settings settings) =>
            signal.Select((s, i) => new Tuple<double, double>(i / settings.XFrequency, s)).ToArray();

        private Tuple<double, double>[] PrepareSignalPreviewData(Complex[] signal, Settings settings) =>
            signal.Select((s, i) => new Tuple<double, double>(i / settings.XFrequency, s.Real)).ToArray();



        private void ShowChart(IEnumerable<Tuple<double, double>> values, string xTitle, string yTitle, string title)
        {
            ChartForm chart = new ChartForm(values, xTitle, yTitle, title);
            chart.Show();
        }

        private void dftClick(object sender, EventArgs e)
        {
            var inputSignal = _signal;
            int maxPower = FindMaxPower(inputSignal.Length);
            var signalCut = inputSignal.Take(1 << maxPower).ToArray();
            ApplyTransform(DFT, signalCut, Direction.Forward);
        }

        private void ApplyTransform(Func<Complex[], Direction, Complex[]> transformFunction, double[] signal, Direction direction)
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
            var N = inputSignal.Length;
            var C = FastDFTInternal(inputSignal, direction);
            if (direction == Direction.Inverse)
            {
                return C.Select((c) => c / N).ToArray();
            }
            return C;
        }

        private Complex[] FastDFTInternal(Complex[] inputSignal, Direction direction)
        {
            int N = inputSignal.Length;
            if (N == 1)
                return inputSignal;
            Complex W = 1;
            var C0 = FastDFTInternal(inputSignal.Where((s, i) => i % 2 == 0).ToArray(), direction);    // F0 = [f0,f2,...,fn-2]  // рекурсия
            var C1 = FastDFTInternal(inputSignal.Where((s, i) => i % 2 == 1).ToArray(), direction);    //  F1 = [f1,f3,...,fn-1]
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
            return C;
        }

        private Complex[] FastDFTN(Complex[] signal, Direction direction)
        {
            int N = signal.Length;
            int maxPower = FindMaxPower(N);
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
                FastDFTStep(direction, i, M, L, ref calculationSignal); // вызов БПФ для отсчетов шагом M
            }
            var result = new Complex[N];
            for (int s = 0; s < M; s++)
            {
                for (int r = 0; r < L; r++)
                {
                    for (int m = 0; m < M; m++)
                    {
                        result[r + s * L] += calculationSignal[m + r * M] * Complex.FromPolarCoordinates(1, (int)direction * 2 * Math.PI * m * (r + s * L) / N);
                    }
                }
            }
            if (direction == Direction.Inverse)
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


        private void FastDFTStep(Direction direction, int Z, int H, int N, ref Complex[] signal)
        { // БПФ с шагом H от отсчета Z
            int M = (int)Math.Log(N, 2);
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
                int m = 1 << s;
                var Wm = Complex.FromPolarCoordinates(1, 2 * Math.PI * (int)direction / m);
                for (int k = 0; k < N; k += m)
                {
                    Complex W = new Complex(1, 0);
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
                        W = W * Wm;
                    }
                }
            }
        }


        private void fdftClick(object sender, EventArgs e)
        {
            var inputSignal = _signal;
            int maxPower = FindMaxPower(inputSignal.Length);
            var signalCut = inputSignal.Take(1 << maxPower).ToArray();
            //Stopwatch timeChecker = new Stopwatch();
            //timeChecker.Start();
            //ApplyTransform(DFT,signalCut,Direction.Forward);
            //timeChecker.Stop();
            //long time1 = timeChecker.ElapsedMilliseconds;
            //timeChecker.Restart();
            ApplyTransform(FastDFT, signalCut, Direction.Forward);
            //timeChecker.Stop();
            //var time2 = timeChecker.ElapsedMilliseconds;
            //MessageBox.Show($"DFT time {time1},{Environment.NewLine}FastDFT time {time2}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch timeChecker = new Stopwatch();
            timeChecker.Start();

            ApplyTransform(FastDFTN, _signal, Direction.Forward);
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
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "5 Гармоник");


            filteredGarmonics = new Complex[transformedSignal.Length];
            for (int i = 0; i < 30; i++)
            {
                filteredGarmonics[i] = transformedSignal[i];
                filteredGarmonics[transformedSignal.Length - 1 - i] = transformedSignal[transformedSignal.Length - 1 - i];
            }
            backSignal = DFT(filteredGarmonics, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "30 Гармоник");

        }

        private Complex[] LowFrequenciesFilter(Complex[] signal, double lowThreshold, Settings settings)
        {

            int lowThresholdIndex = (int)Math.Floor(1.0 * lowThreshold * signal.Length / settings.XFrequency);
            for (int i = lowThresholdIndex + 1; i < signal.Length - lowThresholdIndex; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private Complex[] HighFrequenciesFilter(Complex[] signal, double highThreshold, Settings settings)
        {
            int highThresholdIndex = (int)Math.Floor(1.0 * highThreshold * signal.Length / settings.XFrequency);
            for (int i = 0; i < highThresholdIndex; i++)
            {
                signal[i] = 0;
            }
            for (int i = signal.Length - highThresholdIndex; i < signal.Length; i++)
            {
                signal[i] = 0;
            }
            return signal;
        }

        private Complex[] RejectorFilter(Complex[] signal, int low, int high)
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
                var transformedSignal = FastDFTN(GetComplexData(_signal), Direction.Forward);
                var backSignal = FastDFTN(LowFrequenciesFilter(transformedSignal, (double)numericUpDown1.Value, _settings), Direction.Inverse);
                SaveWav(backSignal.Select((x)=>x.Real).ToArray(), _header, _settings, _fileName, $"low{(int)numericUpDown1.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Low frequencies filter");
            }
            if (highRadio.Checked)
            {
                var transformedSignal = FastDFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = FastDFT(HighFrequenciesFilter(transformedSignal, (double)numericUpDown2.Value, _settings), Direction.Inverse);
                SaveWav(backSignal.Select((x) => x.Real).ToArray(), _header, _settings, _fileName, $"high{(int)numericUpDown2.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "High frequencies filter");
            }
            if (stripeRadio.Checked)
            {
                var transformedSignal = FastDFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = FastDFT(StripeFilter(transformedSignal, (int)numericUpDown1.Value, (int)numericUpDown2.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Stripe frequencies filter");
            }
            if (rejectorRadio.Checked)
            {
                var transformedSignal = FastDFT(GetComplexData(_signal), Direction.Forward);
                var backSignal = FastDFT(RejectorFilter(transformedSignal, (int)numericUpDown1.Value, (int)numericUpDown2.Value), Direction.Inverse);
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Rejector frequencies filter");
            }
        }

        private void SaveWav(double[] backSignal, byte[] header, Settings settings, string fileName, string fileSuffix)
        {
            var fileNameWOExtention = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var resultFileName = $"{Path.GetDirectoryName(fileName)}\\{fileNameWOExtention}{fileSuffix}{ext}";
            using (BinaryWriter writer = new BinaryWriter(new FileStream(resultFileName, FileMode.Create)))
            {
                writer.Write(header);
                for (int i = 0; i < backSignal.Length; i++)
                {
                    short signalValue = (short)backSignal[i];
                    writer.Write(signalValue);
                    //byte byte1 = (byte)(signalValue & 0xFF);
                    //byte byte2 = (byte)((signalValue>>8) & 0xFF);
                    //writer.Write(byte1);
                    //writer.Write(byte2);
                }
            }
        }

        private (Settings settings, double[] signal, byte[] header, string fileName) LoadWav()
        {
            using (var fileDialog = new OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileName = fileDialog.FileName;
                    fileNameLabel.Text = Path.GetFileName(fileName);
                    using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
                    {
                        byte[] header = new byte[44];
                        reader.Read(header, 0, 44);
                        int length =
                            (((int)header[43]) << 24) +
                            (((int)header[42]) << 16) +
                            (((int)header[41]) << 8) +
                            (int)header[40];
                        length = length >> 1;
                        chart1.Series[0].Points.Clear();
                        var signal = new double[length];
                        var selectedSettings = settings["wav"];
                        for (int i = 0; i < length; i++)
                        {
                            byte byteOne = reader.ReadByte();
                            byte byteTwo = reader.ReadByte();
                            int signalOne = (int)(short)(byteOne | byteTwo << 8);
                            //int signalValue = reader.ReadInt16();
                            signal[i] = signalOne - selectedSettings.BaseValue;
                        }

                        return (selectedSettings, signal, header, fileName);
                    }
                }
            }
            return (null, null, null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            (_settings, _signal, _header, _fileName) = LoadWav();
            DisplayData(_settings, _signal, (int)nNumeric.Value);
        }

        enum WeightType
        {
            Rectangular,
            Hamming,
            Bartlet,
            Hanning,
            BlackMan
        }

        Dictionary<WeightType, Func<double,int, double>> WeightFunctions = new Dictionary<WeightType, Func<double,int, double>>()
        {
            { WeightType.Rectangular, (i,N)=>1 },
            { WeightType.Hamming, (i,N)=> 0.54-0.46*Math.Cos(2*Math.PI*i/(N-1))},
            { WeightType.Bartlet, (i,N)=>1-(2*(i-(N-1)/2.0))/(N-1)},
            { WeightType.Hanning, (i,N)=>0.5-0.5*Math.Cos(2*Math.PI*i/(N-1)) },
            { WeightType.BlackMan, (i,N)=>0.42-0.5*Math.Cos(2*Math.PI*i/(N-1))+0.08*Math.Cos(4*Math.PI*i/(N-1)) }
        };

        double[] GenerateWeights(WeightType weightType,int N)
        {
            return Enumerable.Range(0, N).Select((x) => WeightFunctions[weightType](x,N)).ToArray();
        }

        enum FilterType
        {
            High,
            Low
        }

        double[] GenerateH(int N,double cutFrequency,FilterType filterType)
        {
            int M = N - 1;
            int sign = filterType == FilterType.Low ? 1 : -1;
            var centerH = filterType == FilterType.Low ? 2* cutFrequency : 1-2* cutFrequency;
            return Enumerable.Range(0, N).Select((i) =>
            (i == M / 2)? centerH :
            sign*Math.Sin(2 * Math.PI * cutFrequency * (i - M>>1)) 
            / (Math.PI*(i - M>>1)))
            .ToArray();            
        }

        double[] ApplyFilter(double[] signal, double cutFrequency, int N,WeightType weightType,FilterType filterType)
        {
            var result = new double[signal.Length];
            var w = GenerateWeights(weightType, N);
            var h = GenerateH(N,cutFrequency,filterType);
            for (int k = 0; k < signal.Length; k++)
            {
                int windowN = N;
                if (k + 1 < N)
                {
                    windowN = k + 1;
                }
                for (int m = 0; m < windowN; m++)
                {
                    result[k] += w[m] * signal[k-m] * h[m];
                }                
            }
            return result;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            (_settings,_signal) = LoadDataFromFile();
            var fdft = FastDFT(GetComplexData(_signal), Direction.Forward);
            var backSignal = FastDFT(fdft, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "test");
        }

        double CalculateFc(double x, Settings settings) => x / settings.XFrequency / 2;
        

        private void buttonWFilter_Click_1(object sender, EventArgs e)
        {
            if (lowRadio.Checked)
            {

                var backSignal =
                    ApplyFilter(
                        _signal,
                        CalculateFc((double)numericUpDown1.Value, _settings),
                        (int)nNumeric.Value,
                        GetWeightType(),
                        FilterType.Low);                    
                SaveWav(backSignal, _header, _settings, _fileName, $"low{(int)numericUpDown1.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Low frequencies filter");
            }
            if (highRadio.Checked)
            {
                var backSignal =
                    ApplyFilter(
                        _signal,
                        CalculateFc((double)numericUpDown2.Value, _settings),
                        (int)nNumeric.Value,
                        GetWeightType(),
                        FilterType.High);                    
                SaveWav(backSignal, _header, _settings, _fileName, $"high{(int)numericUpDown2.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "High frequencies filter");
            }
        }

        private WeightType GetWeightType()
        {
            if (blackmanRadio.Checked)
                return WeightType.BlackMan;
            if (hammingRadio.Checked)
                return WeightType.Hamming;
            if (hanningRadio.Checked)
                return WeightType.Hanning;
            if (bartletRadio.Checked)
                return WeightType.Bartlet;
            return WeightType.Rectangular;
        }
    }
}
