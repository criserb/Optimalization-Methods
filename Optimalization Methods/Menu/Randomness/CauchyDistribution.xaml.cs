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

namespace Optimalization_Methods.Menu.Randomness
{
    /// <summary>
    /// Interaction logic for CauchyDistribution.xaml
    /// </summary>
    public partial class Cauchy : Page
    {
        /// <summary>
        /// Probability density function
        /// </summary>
        public ChartValues<double> Pdf { get; set; }

        public Cauchy()
        {
            InitializeComponent();     
        } 

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

            DataContext = null;

            Pdf = new ChartValues<double>();
            CauchyDistribution myCauchy = new CauchyDistribution(Double.Parse(RangeFromBox.Text),Double.Parse(RangeFromBox.Text));

            for (double i = Double.Parse(RangeFromBox.Text); i <= Double.Parse(RangeToBox.Text); i += Double.Parse(StepBox.Text))
            {
                Pdf.Add(myCauchy.ProbabilityDensityFunction(i));
            }

            DataContext = this;
        }
    }
}
