using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData()) return;

            PlotInfo plotInfo = new PlotInfo();
            myFirefly = new FireflyAlgorithm(DetailsBox, numFireflies, maxEpochs, minX, maxX, values);
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
            plotInfo = myFirefly.Start(functionDelegate);
            if (plotInfo.SeriesIn != null)
                GeneratePlot(plotInfo);
        }

        public ChartValues<ObservablePoint> Series1 { get; set; }

        private void GeneratePlot(PlotInfo plotInfo)
        {
            DataContext = null;

            Series1 = new ChartValues<ObservablePoint>();

            for (int i = 0; i < plotInfo.SeriesIn.Count; i++)
            {
                Series1.Add(plotInfo.SeriesIn[i]);
            }

            DataContext = this;
        }

        private void BtnLoadFromText_Click(object sender, RoutedEventArgs e)
        {
            if (PointsBox.Text == string.Empty)
                return;
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
            FileNameLbl.Foreground = Brushes.Green;
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
                FileNameLbl.Foreground = Brushes.Green;
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
                    FileNameLbl.Foreground = Brushes.Red;
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
