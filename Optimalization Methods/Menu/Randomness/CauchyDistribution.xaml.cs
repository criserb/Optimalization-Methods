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

namespace Optimalization_Methods.Menu.Randomness
{
    /// <summary>
    /// Interaction logic for CauchyDistribution.xaml
    /// </summary>
    public partial class CauchyDistribution : Page
    {
        public CauchyDistribution()
        {
            InitializeComponent();

            var r = new Random();
            ValuesA = new ChartValues<double>();

            for (var i = 0; i < 20; i++)
            {
                ValuesA.Add(r.Next() % 101);
            }

            DataContext = this;
        }

        public ChartValues<double> ValuesA { get; set; }

        private void RandomizeOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            for (var i = 0; i < 20; i++)
            {
                ValuesA[i] = r.Next() % 101;
            }
        }
    }
}
