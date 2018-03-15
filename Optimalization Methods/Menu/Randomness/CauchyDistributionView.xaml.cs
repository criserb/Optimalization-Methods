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
    public partial class CauchyView : Page
    {
        public ChartValues<ObservablePoint> Pdf { get; set; }
        private double from, to, location, scale;

        public CauchyView()
        {
            InitializeComponent();
            GeneratePlot();
        }

        private void GeneratePlot()
        {
            DataContext = null;

            if (!double.TryParse(RangeFromBox.Text, out from) || !double.TryParse(RangeToBox.Text, out to) ||
                !double.TryParse(LocationBox.Text, out location) || !double.TryParse(ScaleBox.Text, out scale))
            {
                MessageBox.Show("Nieprawidłowy format ciągu wejściowego");
                return;
            }

            double step = (Math.Abs(from) + Math.Abs(to)) / 100;

            Pdf = new ChartValues<ObservablePoint>();
            CauchyDistribution myCauchy = new CauchyDistribution(Double.Parse(LocationBox.Text), Double.Parse(ScaleBox.Text));

            for (double i = from; i <= to; i += step)
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
