using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
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
using Accord.Statistics;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;
using LiveCharts.Wpf;

namespace Optimalization_Methods.Menu.Randomness
{
    /// <summary>
    /// Interaction logic for CauchyDistribution.xaml
    /// </summary>
    public partial class Cauchy : Page
    { 
        public ChartValues<ObservablePoint> Pdf { get; set; }

        public Cauchy()
        {
            InitializeComponent();
            GeneratePlot();
        }

        private void GeneratePlot()
        {
            DataContext = null;

            Pdf = new ChartValues<ObservablePoint>();
            CauchyDistribution myCauchy = new CauchyDistribution(Double.Parse(LocationBox.Text), Double.Parse(ScaleBox.Text));

            for (double i = Double.Parse(RangeFromBox.Text); i <= Double.Parse(RangeToBox.Text); i += Double.Parse(StepBox.Text))
            {
                Pdf.Add(new ObservablePoint(i, myCauchy.ProbabilityDensityFunction(i)));
            }

            DataContext = this;

        }

        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            GeneratePlot();
        }
    }
}
