using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.Win32;

namespace Optimalization_Methods.Menu
{
    /// <summary>
    /// Interaction logic for Firefly.xaml
    /// </summary>
    public partial class FireflyView : Page
    {
        private int numFireflies, maxEpochs;
        private double minX, maxX;
        private List<double> values;
        private FireflyAlgorithm myFirefly;

        public FireflyView()
        {
            InitializeComponent();
        }

        private bool ValidateData()
        {
            if (!int.TryParse(FirefliesBox.Text, out numFireflies) ||
                   !int.TryParse(EpochsBox.Text, out maxEpochs) ||
                   !double.TryParse(MinXBox.Text, out minX) ||
                   !double.TryParse(MaxXBox.Text, out maxX))
            {
                MessageBox.Show("Wrong input format");
                return false;
            }
            return true;
        }

        private async void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData()) return;

            PlotInfo plotInfo = new PlotInfo();
            myFirefly = new FireflyAlgorithm(numFireflies, maxEpochs, minX, maxX, values);
            FunctionDelegate functionDelegate = null;
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    functionDelegate = Functions.Michalewicz;
                    break;
                case 1:
                    functionDelegate = Functions.Rosenbrock;
                    break;
                case 2:
                    functionDelegate = Functions.Ackley;
                    break;
                case 3:
                    functionDelegate = Functions.Rastrigin;
                    break;
                case 4:
                    functionDelegate = Functions.Sphere;
                    break;
                case 5:
                    functionDelegate = Functions.Levy;
                    break;
                case 6:
                    functionDelegate = Functions.Griewank;
                    break;
                case 7:
                    functionDelegate = Functions.Schwefel;
                    break;
                default:
                    break;
            }
            plotInfo = await CalculatePlotInfo(myFirefly, functionDelegate);
            ShowDetails(plotInfo.BestPostion, plotInfo.TrueMin, plotInfo.Val, plotInfo.Error);
            GeneratePlot(plotInfo);
        }

        public Task<PlotInfo> CalculatePlotInfo(FireflyAlgorithm myFirefly, FunctionDelegate functionDelegate)
        {
            return Task.Factory.StartNew(() => myFirefly.Start(functionDelegate));
        }

        void ShowVector(double[] v, int dec, bool nl)
        {
            for (int i = 0; i < v.Length; ++i)
                AddToTextBoxWithoutNewLine(v[i].ToString("F" + dec) + " ");
            if (nl == true)
                AddToTextBox(" ");
            AddToTextBox(" ");
        }

        private void ShowDetails(double[] bestPosition, double trueMin, double z, double error)
        {
            DetailsBox.Text = String.Empty;
            AddToTextBox($"True min at: {Math.Round(trueMin, 5)}");
            AddToTextBox($"Best solution found:");
            ShowVector(bestPosition, 4, false);
            AddToTextBox($"Value of function at best position: {Math.Round(z, 5)}");
            AddToTextBox($"Error at best position: {Math.Round(error, 5)}");
        }

        public void AddToTextBox(string s)
        {
            DetailsBox.Text += s + '\n';
        }

        void AddToTextBoxWithoutNewLine(string s)
        {
            DetailsBox.Text += s;
        }

        public ChartValues<ObservablePoint> Series1 { get; set; }

        public async void GeneratePlot(PlotInfo plotInfo)
        {
            DataContext = null;
            Series1 = new ChartValues<ObservablePoint>();
            DataContext = this;

            Cursor = Cursors.Wait;

            int step = maxEpochs / 10;
            int c = 0;

            for (int i = 0; i < maxEpochs; i++)
            {
                Series1.Add(plotInfo.SeriesIn.Dequeue());
                if (c++ % step == 0)
                    await Task.Delay(200);
            }

            Cursor = Cursors.Arrow;
        }

        private void BtnLoadFromText_Click(object sender, RoutedEventArgs e)
        {
            if (PointsBox.Text == string.Empty)
            {
                FileNameLbl.Foreground = (Brush)Application.Current.Resources["Alizarin"];
                FileNameLbl.Content = "None points in memory";
                return;
            }
            try
            {
                values = ReadValuesFromText();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            BtnPlot.IsEnabled = true;
            FileNameLbl.Foreground = (Brush)Application.Current.Resources["GreenSea"];
            FileNameLbl.Content = "Points from input box are in memory";
        }

        private List<double> ReadValuesFromText()
        {
            List<double> myList = new List<double>();

            String[] items = PointsBox.Text.
               Split(new String[] { "/", Environment.NewLine },
               StringSplitOptions.RemoveEmptyEntries);


            for (int i = 0; i < items.Length; i++)
            {
                myList.Add(double.Parse(items[i]));
            }

            return myList;
        }

        private List<double> ReadValues()
        {
            List<double> myList = new List<double>();

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                FileNameLbl.Content = "Points from file: " + openFileDialog1.SafeFileName;
                FileNameLbl.Foreground = (Brush)Application.Current.Resources["GreenSea"];
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    {
                        string line;

                        // Read and display lines from the file until 
                        // the end of the file is reached. 
                        while ((line = sr.ReadLine()) != null)
                        {
                            myList.Add(double.Parse(line));
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileNameLbl.Content = "None points in memory";
                    FileNameLbl.Foreground = (Brush)Application.Current.Resources["Alizarin"];
                    BtnPlot.IsEnabled = false;
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                BtnPlot.IsEnabled = true;
            }
            return myList;
        }

        private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            values = ReadValues();
        }
    }
}
