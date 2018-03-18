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
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.regressiontesting;

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
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    plotInfo = myFirefly.Start(FunctionChoice.Michalewicz);
                    break;
                case 1:
                    plotInfo = myFirefly.Start(FunctionChoice.Rosenbrock);
                    break;
                case 2:
                    plotInfo = myFirefly.Start(FunctionChoice.Ackley);
                    break;
                case 3:
                    plotInfo = myFirefly.Start(FunctionChoice.Rastrigin);
                    break;
                default:
                    break;
            }
            if (plotInfo.SeriesIn != null)
                GeneratePlot(plotInfo);
        }

        public ChartValues<double> Series1 { get; set; }

        private void GeneratePlot(PlotInfo plotInfo)
        {
            DataContext = null;

            Series1 = new ChartValues<double>();

            for (int i = 0; i < plotInfo.SeriesIn.Count; i++)
            {
                Series1.Add(plotInfo.SeriesIn[i]);
            }

            DataContext = this;
        }

        private void BtnLoadFromText_Click(object sender, RoutedEventArgs e)
        {
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
            FileNameLbl.Content = "Points from input box";
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
                FileNameLbl.Content = "File in memory: " + openFileDialog1.SafeFileName;
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
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            return myList;
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                values = ReadValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            FileNameLbl.Foreground = Brushes.Green;
            BtnPlot.IsEnabled = true;
        }
    }
}
