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
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    if (FunctionBox.Text == String.Empty)
                        return;
                    myFirefly = new FireflyAlgorithm(DetailsBox, numFireflies, maxEpochs, minX, maxX);
                    plotInfo = myFirefly.OwnFunction(FunctionBox.Text, ReadArgs());
                    break;
                case 1:
                    myFirefly = new FireflyAlgorithm(DetailsBox, numFireflies, maxEpochs, minX, maxX, values);
                    plotInfo = myFirefly.Demo();
                    break;
                default:
                    break;
            }
            if (plotInfo.SeriesIn != null)
                GeneratePlot(plotInfo);
        }

        private string ReadArgs()
        {
            //List<Argument> args = new List<Argument>();
            String[] items = ArgumentsBox.Text.
               Split(new String[] { ",", Environment.NewLine },
               StringSplitOptions.RemoveEmptyEntries);

            string[] tmp;
            string args = "";

            for (int i = 0; i < items.Length; i++)
            {
                tmp = items[i].Split(new String[] { "=", Environment.NewLine },
               StringSplitOptions.RemoveEmptyEntries);
                args += tmp[1];
                if (i + 1 < items.Length)
                   args += ',';
            }

            foreach (var item in args)
            {
                MessageBox.Show(item.ToString());
            }
            return args;
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    BtnPlot.IsEnabled = true;
                    BtnLoad.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    BtnLoad.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
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
            values = ReadValues();
            //BtnPlot.IsEnabled = true;
        }
    }
}
