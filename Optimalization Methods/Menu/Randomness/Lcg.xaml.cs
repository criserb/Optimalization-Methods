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
    /// Interaction logic for Lcg.xaml
    /// </summary>
    public partial class Lcg : Page
    {
        private bool _loop;
        private long _m, _a, _c, _seed, _counter, _last;
        private const double _lineLength = 10;
        private List<Rectangle> _myTiles;

        public Lcg()
        {
            InitializeComponent();
            _myTiles = InitializeGrid();
        }

        public long Next()
        {
            _last = ((_a * _last) + _c) % _m;

            return _last;
        }

        public long Next(long maxValue)
        {
            return Next() % maxValue;
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _loop = true;
            _seed = 0;
            _counter = 1;
            SettingTiles();
            if (!long.TryParse(MBox.Text, out _m) || !long.TryParse(ABox.Text, out _a) || !long.TryParse(CBox.Text, out _c))
            {
                MessageBox.Show("Nieprawidłowy format ciągu wejściowego");
                return;
            }
            long.TryParse(SeedBox.Text, out _seed);
            // if we set seed as 0 it will be given by actual second
            _last = (_seed == 0) ? DateTime.Now.Ticks % _m : _seed;
            RandomWalk randomWalker = new RandomWalk(myGrid, _myTiles);

            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;

            while (_loop)
            {
                loopNumber.Content = _counter++.ToString();
                randomWalker.Update(Next());
                await Task.Delay(200);
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _loop = false;
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;
        }

        private void SettingTiles()
        {
            foreach (var item in _myTiles)
            {
                item.Fill = Brushes.White;
            }
        }

        private List<Rectangle> InitializeGrid()
        {
            List<Rectangle> myTiles = new List<Rectangle>();
            for (int i = 0; i < myGrid.Height / 10; i++)
            {
                for (int j = 0; j < myGrid.Width / 10; j++)
                {
                    myTiles.Add(new Rectangle
                    {
                        Stroke = Brushes.Gray,
                        Margin = new Thickness(j * _lineLength, i * _lineLength, 0, 0)
                    }
                );
                }
            }
            foreach (var item in myTiles)
            {
                myGrid.Children.Add(item);
            }
            return myTiles;
        }
    }
}
