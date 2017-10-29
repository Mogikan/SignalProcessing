﻿using System;
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
using static System.Math;

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

        private static int FindMaxPower(int input)
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
            result.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
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
            ApplyTransform(FastDFT, signalCut, Direction.Forward);
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
                SaveWav(backSignal.Select((x) => x.Real).ToArray(), _header, _settings, _fileName, $"low{(int)numericUpDown1.Value}");
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

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    (_settings, _signal, _header, _fileName) = LoadWav();
        //    DisplayData(_settings, _signal, (int)nNumeric.Value);
        //}      
          
        enum WeightType
        {
            Rectangular,
            Hamming,
            Bartlet,
            Hanning,
            BlackMan
        }

        Dictionary<WeightType, Func<double, int, double>> WeightFunctions = new Dictionary<WeightType, Func<double, int, double>>()
        {
            { WeightType.Rectangular, (i,N)=>1 },
            { WeightType.Hamming, (i,N)=> 0.54-0.46*Math.Cos(2*Math.PI*i/(N-1))},
            { WeightType.Bartlet, (i,N)=>1-(2*Math.Abs(i-(N-1)/2.0))/(N-1)},
            { WeightType.Hanning, (i,N)=>0.5-0.5*Math.Cos(2*Math.PI*i/(N-1)) },
            { WeightType.BlackMan, (i,N)=>0.42-0.5*Math.Cos(2*Math.PI*i/(N-1))+0.08*Math.Cos(4*Math.PI*i/(N-1)) }
        };

        double[] GenerateWeights(WeightType weightType, int N)
        {
            return Enumerable.Range(0, N).Select((x) => WeightFunctions[weightType](x, N)).ToArray();
        }

        enum FilterType
        {
            High,
            Low
        }

        double[] GenerateH(int N, double cutFrequency, FilterType filterType)
        {
            int M = N - 1;
            int sign = filterType == FilterType.Low ? 1 : -1;
            var centerH = filterType == FilterType.Low ? 2 * cutFrequency : 1 - 2 * cutFrequency;
            return Enumerable.Range(0, N).Select(
                (i) =>
                (i == (M >> 1)) ? centerH :
                sign * Math.Sin(2 * Math.PI * cutFrequency * (i - (M >> 1)))
                / (Math.PI * (i - (M >> 1))))
                .ToArray();
        }

        double[] ApplyFilter(double[] signal, double cutFrequency, int N, WeightType weightType, FilterType filterType)
        {
            var result = new double[signal.Length];
            var w = GenerateWeights(weightType, N);
            var h = GenerateH(N, cutFrequency, filterType);
            for (int k = 0; k < signal.Length; k++)
            {
                int windowN = N;
                if (k + 1 < N)
                {
                    windowN = k + 1;
                }
                for (int m = 0; m < windowN; m++)
                {
                    result[k] += w[m] * signal[k - m] * h[m];
                }
            }
            return result;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            (_settings, _signal) = LoadDataFromFile();
            var fdft = FastDFT(GetComplexData(_signal), Direction.Forward);
            var backSignal = FastDFT(fdft, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "test");
        }

        double CalculateFc(double x, Settings settings) => x / settings.XFrequency * 2;


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
            //return WeightType.Rectangular;
            //if (blackmanRadio.Checked)
            //    return WeightType.BlackMan;
            //if (hammingRadio.Checked)
            //    return WeightType.Hamming;
            //if (hanningRadio.Checked)
            return WeightType.Hanning;
            //if (bartletRadio.Checked)
            //    return WeightType.Bartlet;
            //return WeightType.Rectangular;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void ApplyTransformWithLogChart(Func<Complex[], Direction, Complex[]> transformFunction, double[] signal, Direction direction)
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
            ShowChart(magnitudePoints.Select((p) => new Tuple<double, double>(p.Item1, 20 * Math.Log10(p.Item2))).ToArray()
                , chartXLabel, _settings.YLabel, $"Log Magnitude {transformFunction.Method.Name}");

            var phasePoints = phase.Select((s, i) => new Tuple<double, double>(i * settings.XFrequency / phase.Length, s));
            ShowChart(phasePoints, chartXLabel, _settings.YLabel, $"Phase {transformFunction.Method.Name}");
        }

        private const int M = 8;
        private const int F = M * 360;
        private const double step = 2 * Math.PI / F;

        double Butterworth(double w, int n) => 1.0 / Math.Sqrt(1.0 + Math.Pow(w, 2 * n));

        private List<Tuple<double, double>> CalculateButterworthAFC(int n)
        {
            return Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, double>(i * step, Butterworth(step * i, n))
                ).ToList();
        }

        private double T(int n, double x)
        {
            if (n == 0) return 1;
            if (n == 1) return x;
            return 2 * x * T(n - 1, x) - T(n - 2, x);
        }

        double Sqr(double x) => x * x;
        double Chebyshev1(double x, int n, double epsilon) =>
            1 / Math.Sqrt(1 + Sqr(epsilon * T(n, x)));

        private List<Tuple<double, double>> CalculateChebyshev1AFC(int n, double epsilon)
        {

            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, double>(i * step, Chebyshev1(step * i, n, epsilon))
                ).ToList();
            return result;
        }

        double Chebyshev2(double x, int n, double epsilon) =>
            1.0 / Math.Sqrt(1 + 1 / Sqr(epsilon * T(n, 1 / x)));

        private List<Tuple<double, double>> CalculateChebyshev2AFC(int n, double epsilon)
        {

            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, double>(i * step, Chebyshev2(step * i, n, epsilon))
                ).ToList();
            return result;
        }

        private void ShowButterworthAFC()
        {
            var butterworth5 = CalculateButterworthAFC(5);
            var butterworth8 = CalculateButterworthAFC(8);
            var butterworth11 = CalculateButterworthAFC(11);
            ChartForm butterworthChart = new ChartForm(butterworth5, "w", "", "Butterworth AFC", "N=5");
            butterworthChart.AddSeriresData(butterworth8, "N=8");
            butterworthChart.AddSeriresData(butterworth11, "N=11");
            butterworthChart.Show();
        }

        private void ShowChebyshev1AFC()
        {
            var chebyshev1_5 = CalculateChebyshev1AFC(5, 0.5);
            var chebyshev1_8 = CalculateChebyshev1AFC(8, 0.5);
            var chebyshev1_11 = CalculateChebyshev1AFC(8, 0.4);
            ChartForm chart = new ChartForm(chebyshev1_5, "w", "", "Chebyshev1 AFC", "N=5, E=0.5");
            chart.AddSeriresData(chebyshev1_8, "N=8, E=0.5");
            chart.AddSeriresData(chebyshev1_11, "N=11, E=0.4");
            chart.Show();
        }

        private void ShowChebyshev2AFC()
        {
            var chebyshev2_5 = CalculateChebyshev2AFC(5, 0.1);
            var chebyshev2_8 = CalculateChebyshev2AFC(8, 0.1);
            var chebyshev2_11 = CalculateChebyshev2AFC(11, 0.05);
            ChartForm chart = new ChartForm(chebyshev2_5, "w", "", "Chebyshev2 AFC", "N=5, E=0.1");
            chart.AddSeriresData(chebyshev2_8, "N=8, E=0.1");
            chart.AddSeriresData(chebyshev2_11, "N=11, E=0.2");
            chart.Show();
        }

        //private void button2_Click_2(object sender, EventArgs e)
        //{
        //    if (radioButterworth.Checked)
        //    {
        //        ShowButterworthAFC();
        //    }
        //    if (radioChebyshev1.Checked)
        //    {
        //        ShowChebyshev1AFC();
        //    }
        //    if (radioChebyshev2.Checked)
        //    {
        //        ShowChebyshev2AFC();
        //    }
        //}

        private Complex BatterwothH(Complex s, int n)
        {
            int coeff = n % 2;
            Complex mult = 1 + s * coeff;

            for (int k = 1; k <= (n - coeff) / 2; k++)
            {
                double theta = (2 * k - 1) * Math.PI / (2 * n);
                mult *= (s * s + 2 * Math.Sin(theta) * s + 1);
            }
            return 1 / mult;
        }

        private (List<Tuple<double, double>> afc, List<Tuple<double, double>> ffc) CalculateBatterworth(int n)
        {

            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, Complex>(i * step, BatterwothH(new Complex(0, i * step), n))
                ).ToList();

            var afc = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Magnitude));
            var ffc = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Phase + (p.Item2.Phase > 0 ? -Math.PI : 0)));

            return (afc.ToList(), Unwrap(ffc.ToList()));
        }


        private (List<Tuple<double, double>> afc, List<Tuple<double, double>> ffc) CalculateChebyshev1KwFC(int n, double e)
        {

            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, Complex>(i * step, Chebyshev1Kw(new Complex(0, i * step), n, e))
                ).ToList();

            var afcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Magnitude));
            var ffcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Phase + (p.Item2.Phase > 0 ? -Math.PI : 0)));

            return (afcval.ToList(), Unwrap(ffcval.ToList()));
        }

        private const double unwrapEpsilon = 0.3 / M;

        private List<Tuple<double, double>> Unwrap(List<Tuple<double, double>> source)
        {
            double correction = 0;
            List<Tuple<double, double>> result = new List<Tuple<double, double>>();
            result.Add(new Tuple<double, double>(source[0].Item1, source[0].Item2));
            for (int i = 1; i < source.Count; i++)
            {
                double diff = Math.Abs(source[i].Item2 - source[i - 1].Item2);
                if (diff + unwrapEpsilon >= Math.PI * 0.95) correction += Math.PI;
                result.Add(new Tuple<double, double>(source[i].Item1, source[i].Item2 - correction));
            }
            return result;
        }

        private Complex Chebyshev1Kw(Complex s, int n, double e)
        {
            double arsh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));
            double betta = arsh(1.0 / e) / n;
            double sig0 = -Math.Sinh(betta);

            double Gp = n % 2 == 1 ? 1.0 : 1.0 / Math.Sqrt(1.0 + e * e);
            Complex C = n % 2 == 1 ? -sig0 / (s - sig0) : 1.0;

            Complex mul = 1;
            for (int k = 1; k <= (n - n % 2) / 2; k++)
            {
                double thetta = (2 * k - 1) * Math.PI / (2 * n);
                double sig = -Math.Sin(thetta) * Math.Sinh(betta);
                double w = Math.Cos(thetta) * Math.Cosh(betta);
                mul *= (sig * sig + w * w) / (s * s - 2 * sig * s + sig * sig + w * w);
            }
            return mul * Gp * C;
        }

        private void ShowButterworthKwFC()
        {
            var butterworth5 = CalculateBatterworth(5);
            var butterworth8 = CalculateBatterworth(8);
            var butterworth11 = CalculateBatterworth(11);
            ChartForm afcf = new ChartForm(butterworth5.afc, "w", "", "Butterworth K(w) AFC", "N=5");
            afcf.AddSeriresData(butterworth8.afc, "N=8");
            afcf.AddSeriresData(butterworth11.afc, "N=11");
            afcf.Show();


            ChartForm ffcf = new ChartForm(butterworth5.ffc, "w", "", "Butterworth K(w) PFC", "N=5");
            ffcf.AddSeriresData(butterworth8.ffc, "N=8");
            ffcf.AddSeriresData(butterworth11.ffc, "N=11");
            ffcf.Show();
        }

        private void ShowChebyshev1KwFC()
        {
            var chebyshev1_5 = CalculateChebyshev1KwFC(5, 0.5);
            var chebyshev1_8 = CalculateChebyshev1KwFC(8, 0.5);
            var chebyshev1_11 = CalculateChebyshev1KwFC(30, 0.2);

            ChartForm afcf = new ChartForm(chebyshev1_5.afc, "w", "", "Chebyshev1 K(w) AFC", "N=5 E=0.5");
            afcf.AddSeriresData(chebyshev1_8.afc, "N=8 E=0.5");
            afcf.AddSeriresData(chebyshev1_11.afc, "N=11 E=0.2");
            afcf.Show();

            ChartForm ffcf = new ChartForm(chebyshev1_5.ffc, "w", "", "Chebyshev1 K(w) PFC", "N=5 E=0.5");
            ffcf.AddSeriresData(chebyshev1_8.ffc, "N=8 E=0.5");
            ffcf.AddSeriresData(chebyshev1_11.ffc, "N=11 E=0.2");
            ffcf.Show();
        }

        //private void button1_Click_2(object sender, EventArgs e)
        //{
        //    if (radioButterworth.Checked)
        //    {
        //        ShowButterworthKwFC();
        //    }
        //    if (radioChebyshev1.Checked)
        //    {
        //        ShowChebyshev1KwFC();
        //    }
        //}

        private static double[] ApplyChebyshev1Filter(double[] source, double fc, int N, FilterType type = FilterType.Low)
        {
            var result = new double[source.Length];

            var hc = CreateChebyshevFilter(N, fc, type);

            //y[k] = b0x[k] + b1x[k - 1] + b2x[k - 2] + 
            //    a1y[k - 1] + a2y[k - 2]

            for (int i = N; i < source.Length; i++)
            {
                result[i] = hc.alpha[0] * source[i];
                for (int k = 1; k <= N; k++)
                {
                    result[i] += hc.alpha[k] * source[i - k] + hc.betta[k] * result[i - k];
                }
            }
            return result;
        }


        private static (double[] alpha, double[] betta) CreateChebyshevFilter(int n, double fc, FilterType filterType, int pr = 10)
        {
            // NP количество звеньев (полюсов) 2 – 20 (четное)
            Debug.Assert(n % 2 == 0);
            int NP = n;
            int N = n + 2;//extra space for shift
            double[] a, b, ta, tb;
            double a0;
            double a1;
            double a2;
            double b1;
            double b2;
            a = new double[N + 1];
            b = new double[N + 1];
            ta = new double[N + 1];
            tb = new double[N + 1];

            a[2] = b[2] = 1;

            for (int j = 1; j <= NP / 2; j++)
            {
                //CalcPole(j)
                (a0, a1, a2, b1, b2) = CalcPole(NP, fc, filterType, j, pr);

                //
                a.CopyTo(ta, 0);
                b.CopyTo(tb, 0);
                for (int i = 2; i <= N; i++)
                {
                    a[i] = a0 * ta[i] + a1 * ta[i - 1] + a2 * ta[i - 2];
                    b[i] = tb[i] - b1 * tb[i - 1] - b2 * tb[i - 2];
                }
            }

            b[2] = 0;
            for (int i = 0; i <= n; i++)
            {
                a[i] = a[i + 2];
                b[i] = -b[i + 2];
            }
            // нормировка коэффициента усиления
            ///*
            double SA = 0;
            double SB = 0;
            int k = 0;
            for (int i = 0; i <= n; i++)
            {
                if (filterType == FilterType.Low)
                {
                    SA += a[i];
                    SB += b[i];
                }
                else
                {
                    k = ((i % 2) == 0) ? 1 : -1;
                    SA += a[i] * k;
                    SB += b[i] * k;
                }
            }
            double GAIN = SA / (1 - SB);
            for (int i = 0; i <= n; i++)
            {
                a[i] = a[i] / GAIN;
            }
            //*/
            return (a, b);
        }

        private static (double a0, double a1, double a2, double b1, double b2)
            CalcPole(int NP, double fc, FilterType filterType, int p, int pr)
        {
            //double arsh(double x) => Log(x + Sqrt(x * x + 1));
            double sqr(double x) => x * x;


            ///*
            double phase = PI / 2 / NP + (p - 1) * PI / NP;//PI / (2 * n) + (j - 1) * PI / n;
            double rp = -Cos(phase);
            double ip = Sin(phase);

            if (pr != 0)
            {
                double es = Sqrt(sqr(100.0 / (100.0 - pr)) - 1);
                double vx = 1.0 / NP * Log(1.0 / es + Sqrt(1 / sqr(es) + 1));
                double kx = 1.0 / NP * Log(1.0 / es + Sqrt(1 / sqr(es) - 1));
                kx = (Exp(kx) + Exp(-kx)) / 2;
                //p = p * Sinh(vx) / Cosh(vx);
                rp = rp * (Exp(vx) - Exp(-vx)) / 2 / kx;
                ip = ip * (Exp(vx) + Exp(-vx)) / 2 / kx;
            }
            // преобразование аналоговой области в цифровую
            //Debug.Assert(fc == 0.5);
            double t = 2 * Tan(0.5);
            double w = 2 * PI * fc;
            //m = rp * rp + ip * ip;
            double m = sqr(rp) + sqr(ip);
            double d = 4 - 4 * rp * t + m * sqr(t);
            double x0 = sqr(t) / d;
            double x1 = 2 * sqr(t) / d;
            double x2 = sqr(t) / d;
            double y1 = (8 - 2 * m * sqr(t)) / d;
            double y2 = (-4 - 4 * rp * t - m * sqr(t)) / d;
            double k;
            if (filterType == FilterType.High)
            {
                k = -Cos(w / 2.0 + 0.5) / Cos(w / 2.0 - 0.5);
            }
            else
            {
                k = Sin(0.5 - w / 2.0) / Sin(0.5 + w / 2.0);
            }
            d = 1 + y1 * k - y2 * sqr(k);
            double a0 = (x0 - x1 * k + x2 * sqr(k)) / d;
            double a1 = (-2 * x0 * k + x1 + x1 * sqr(k) - 2 * x2 * k) / d;
            double a2 = (x0 * sqr(k) - x1 * k + x2) / d;
            double b1 = (2 * k + y1 + y1 * sqr(k) - 2 * y2 * k) / d;
            double b2 = (-sqr(k) - y1 * k + y2) / d;
            if (filterType == FilterType.High)
            {
                a1 = -a1;
                b1 = -b1;
            }
            return (a0, a1, a2, b1, b2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (lowRadio.Checked)
            {

                var backSignal =
                    ApplyChebyshev1Filter(
                        _signal,
                        CalculateFc((double)numericUpDown1.Value, _settings),
                        (int)nNumeric.Value,
                        FilterType.Low);
                SaveWav(backSignal, _header, _settings, _fileName, $"low{(int)numericUpDown1.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "Chebyshev1", _settings.YLabel, "Chebyshev1 Low frequencies filter");
            }
            if (highRadio.Checked)
            {
                var backSignal =
                    ApplyChebyshev1Filter(
                        _signal,
                        CalculateFc((double)numericUpDown2.Value, _settings),
                        (int)nNumeric.Value,
                        FilterType.High);
                SaveWav(backSignal, _header, _settings, _fileName, $"high{(int)numericUpDown2.Value}");
                ShowChart(PrepareSignalPreviewData(backSignal, _settings), "Chebyshev1", _settings.YLabel, "Chebyshev1 High frequencies filter");
            }
        }

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    if (lowRadio.Checked)
        //    {
        //        var backSignal =
        //            ApplyChebyshev1Filter(
        //                _signal,
        //                CalculateFc((double)numericUpDown1.Value, _settings),
        //                (int)polesNumeric.Value * 2,
        //                FilterType.Low);
        //        SaveWav(backSignal, _header, _settings, _fileName, $"lowCheb{(int)numericUpDown1.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "Chebyshev1", _settings.YLabel, $"Chebyshev1 Poles={polesNumeric.Value} High frequencies filter {numericUpDown1.Value}Hz");
        //    }
        //    if (highRadio.Checked)
        //    {
        //        var backSignal =
        //            ApplyChebyshev1Filter(
        //                _signal,
        //                CalculateFc((double)numericUpDown2.Value, _settings),
        //                (int)polesNumeric.Value * 2,
        //                FilterType.High);
        //        SaveWav(backSignal, _header, _settings, _fileName, $"highCheb{(int)numericUpDown2.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "Chebyshev1", _settings.YLabel, $"Chebyshev1 Poles={polesNumeric.Value} High frequencies filter {numericUpDown2.Value}Hz");
        //    }
        //
        //    if (lowRadio.Checked)
        //    {
        //
        //        var backSignal =
        //            ApplyFilter(
        //                _signal,
        //                CalculateFc((double)numericUpDown1.Value, _settings),
        //                (int)nNumeric.Value,
        //                GetWeightType(),
        //                FilterType.Low);
        //        SaveWav(backSignal, _header, _settings, _fileName, $"lowWindow{(int)numericUpDown1.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, $"Hanning {nNumeric.Value} High frequencies filter {numericUpDown1.Value}Hz");
        //    }
        //    if (highRadio.Checked)
        //    {
        //        var backSignal =
        //            ApplyFilter(
        //                _signal,
        //                CalculateFc((double)numericUpDown2.Value, _settings),
        //                (int)nNumeric.Value,
        //                GetWeightType(),
        //                FilterType.High);
        //        SaveWav(backSignal, _header, _settings, _fileName, $"highWindow{(int)numericUpDown2.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, $"Hanning {nNumeric.Value} High frequencies filter {numericUpDown2.Value}Hz");
        //    }
        //
        //    if (lowRadio.Checked)
        //    {
        //        var transformedSignal = FastDFTN(GetComplexData(_signal), Direction.Forward);
        //        var backSignal = FastDFTN(LowFrequenciesFilter(transformedSignal, (double)numericUpDown1.Value, _settings), Direction.Inverse);
        //        SaveWav(backSignal.Select((x) => x.Real).ToArray(), _header, _settings, _fileName, $"lowDFT{(int)numericUpDown1.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, $"DFT Low frequencies filter {numericUpDown1.Value}Hz");
        //    }
        //    if (highRadio.Checked)
        //    {
        //        var transformedSignal = FastDFT(GetComplexData(_signal), Direction.Forward);
        //        var backSignal = FastDFT(HighFrequenciesFilter(transformedSignal, (double)numericUpDown2.Value, _settings), Direction.Inverse);
        //        SaveWav(backSignal.Select((x) => x.Real).ToArray(), _header, _settings, _fileName, $"highDFT{(int)numericUpDown2.Value}");
        //        ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, $"DFT High frequencies filter {numericUpDown2.Value}Hz");
        //    }
        //}

        double Log2(double x) => Math.Log(x) / Math.Log(2);

        public static int[][] BuildWalshMatrix(int N)
        {
            int n = FindMaxPower(N)+1;
            int[][] result = new int[N][];
            int[] R = new int[n];
            for (int u = 0; u < N; u++)
            {
                result[u] = new int[N];
                int ut = u;
                int sr = 1 << (n-1);
                R[0] = (ut & sr) != 0 ? 1 : 0;
                for (int i = 1; i < n; i++)
                {
                    R[i] = (ut & sr) != 0 ? 1 : 0;
                    sr >>= 1;
                    R[i] += (ut & sr) != 0 ? 1 : 0;                    
                }
                for (int v = 0; v < N; v++)
                {
                    int vt = v;
                    int sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        sum += R[i] * (vt & 1);
                        vt >>= 1;
                    }
                    result[u][v] = (sum % 2) == 0 ? 1 : -1;
                }
            }
            return result;
        }

        int FromGray(int num)
        {
            int bin;
            for (bin = 0; num > 0; num >>= 1)
            {
                bin ^= num;
            }
            return bin;
            //int mask;
            //for (mask = num >> 1; mask != 0; mask = mask >> 1)
            //{
            //    num = num ^ mask;
            //}
            //return num;
        }

        //H[2]= 1  1
        //      1 -1
        //-H[2]= -1 -1
        //       -1  1

        //H[2N] = H[N]  H[N]
        //        H[N] –H[N]



        private int[][] BuildHadamardMatrixRecursive(int n)
        {
            if (n == 1)
            {
                return new int[][]
                {
                        new int[]{ 1 }
                };
            }
            var submatrix = BuildHadamardMatrixRecursive(n >> 1);
            return MatrixOperationsHelper.CombineSubmatrix(submatrix, submatrix, submatrix, submatrix.NegateValues());
        }



        private int[][] BuildHadamardMatrix(int N)
        {
            int n = FindMaxPower(N)+1;
            int[][] result = new int[N][];
            int[] R = new int[n];
            for (int u = 0; u < N; u++)
            {
                result[u] = new int[N];
                int ut = FromGray(ReverseIndex(u, n));
                int sr = 1 << (n-1);
                R[0] = (ut & sr) != 0 ? 1 : 0;
                for (int i = 1; i < n; i++)
                {
                    R[i] = (ut & sr) != 0 ? 1 : 0;
                    sr >>= 1;
                    R[i] += (ut & sr) != 0 ? 1 : 0;
                    //R[i] %= 2;
                }
                for (int v = 0; v < N; v++)
                {
                    int vt = v;
                    int sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        sum += R[i] * (vt & 1);
                        vt >>= 1;
                    }
                    result[u][v] = (sum % 2) == 0 ? 1 : -1;
                }
            }
            return result;
        }

        private double[] WalshTransform(double[] inputSignal, Direction direction)
        {
            Debug.Assert(inputSignal.Length==1<<FindMaxPower(inputSignal.Length));
            int pointsCount = inputSignal.Length;
            var walsh = BuildWalshMatrix(pointsCount);
            
            double[] transform = new double[pointsCount];
            for (int k = 0; k < pointsCount; k++)
            {
                double sum = 0;
                for (int i = 0; i < pointsCount; i++)
                {
                    sum += inputSignal[i] * walsh[k][i];
                }
                if (direction == Direction.Forward)
                {
                    sum /= pointsCount;
                }

                transform[k] = sum;
            }
            return transform;
        }

        private double[] HadamardTransform(double[] inputSignal, Direction direction)
        {
            Debug.Assert(inputSignal.Length == 1 << FindMaxPower(inputSignal.Length));
            int pointsCount = inputSignal.Length;
            var walsh = BuildHadamardMatrix(pointsCount);
            double[] transform = new double[pointsCount];
            for (int k = 0; k < pointsCount; k++)
            {
                double sum = 0;
                for (int i = 0; i < pointsCount; i++)
                {
                    sum += inputSignal[i] * walsh[k][i];
                }
                if (direction == Direction.Forward)
                {
                    sum /= pointsCount;
                }

                transform[k] = sum;
            }
            return transform;
        }

        private double[] RecursiveHadamardTransform(double[] inputSignal, Direction direction)
        {
            int pointsCount = inputSignal.Length;
            var walsh = BuildHadamardMatrixRecursive(pointsCount);
            double[] transform = new double[pointsCount];
            for (int k = 0; k < pointsCount; k++)
            {
                double sum = 0;
                for (int i = 0; i < pointsCount; i++)
                {
                    sum += inputSignal[i] * walsh[k][i];
                }
                if (direction == Direction.Forward)
                {
                    sum /= pointsCount;
                }

                transform[k] = sum;
            }
            return transform;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            (_settings, _signal) = LoadDataFromFile();
            DisplayData(_settings, _signal);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var walsh = WalshTransform(_signal.Take(1<<FindMaxPower(_signal.Length)).ToArray(), Direction.Forward);
            if (nNumeric.Value > 0)
            {
                for (int i = (int)nNumeric.Value; i < walsh.Length; i++)
                {
                    walsh[i] = 0;
                }
            }
            var backSignal = WalshTransform(walsh, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Walsh");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var hadamard = HadamardTransform(_signal.Take(1<<FindMaxPower(_signal.Length)).ToArray(), Direction.Forward);
            if (nNumeric.Value > 0)
            {
                for (int i = (int)nNumeric.Value; i < hadamard.Length; i++)
                {
                    hadamard[i] = 0;
                }
            }
            var backSignal = HadamardTransform(hadamard, Direction.Inverse);
            ShowChart(PrepareSignalPreviewData(backSignal, _settings), "", _settings.YLabel, "Hadamard");
        }

        private (double[] amplitude, double[] phase) CalculateMetricsHadamard(double[] signal)
        {
            int N = signal.Length;
            if (ShowHalfSpectre)
            {
                N = N / 2;
            }
            double[] phase = new double[N];
            double[] amplitude = new double[N];
            for (int i = 1; i < N; i++)
            {
                amplitude[i-1] = Sqrt(signal[i] * signal[i] + signal[i + 1] * signal[i + 1]);
                phase[i-1] = Atan2(signal[i], signal[i + 1]);
            }
            return (amplitude, phase);
        }

        private void ApplyTransform(Func<double[], Direction, double[]> transformFunction, double[] signal, Direction direction)
        {
            var settings = _settings;
            if (signal == null)
                return;
            var chartXLabel = "";
            var result = transformFunction(signal, direction);
            (double[] amp, double[] phase) = CalculateMetricsHadamard(result);
            var magnitudePoints = amp.Select(
                (a, i) => new Tuple<double, double>(1.0 * i * settings.XFrequency / amp.Length, amp[i])
                );
            ShowChart(magnitudePoints, chartXLabel, _settings.YLabel, $"Magnitude {transformFunction.Method.Name}");
            var phasePoints = phase.Select((s, i) => new Tuple<double, double>(i * settings.XFrequency / phase.Length, s));
            ShowChart(phasePoints, chartXLabel, _settings.YLabel, $"Phase {transformFunction.Method.Name}");
        }

        private void hadamard_Click(object sender, EventArgs e)
        {
            ApplyTransform(HadamardTransform, _signal.Take(1<<FindMaxPower(_signal.Length)).ToArray(), Direction.Forward);
        }

        private void walsh_Click(object sender, EventArgs e)
        {
            ApplyTransform(WalshTransform, _signal.Take(1 << FindMaxPower(_signal.Length)).ToArray(), Direction.Forward);
        }
    }
}
