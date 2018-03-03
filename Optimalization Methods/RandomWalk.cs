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

namespace Optimalization_Methods
{
    class RandomWalk
    {
        private Grid _myGrid;
        private List<Rectangle> _myTiles;

        public RandomWalk(Grid myGrid, List<Rectangle> myTiles)
        {
            this._myGrid = myGrid;
            _myTiles = myTiles;
        }

        public void Update(long randomNumber)
        {

            if (_myTiles[Convert.ToInt32(randomNumber % _myTiles.Count)].Fill != Brushes.White)
            {
                SolidColorBrush newBrush = (SolidColorBrush)_myTiles[Convert.ToInt32(randomNumber % _myTiles.Count)].Fill;
                int R = newBrush.Color.R;
                R = (R + 75) % 255;
                int G = newBrush.Color.G;
                G = (G - 35) % 255;
                int B = newBrush.Color.B;
                B = (B + 15) % 255;
                _myTiles[Convert.ToInt32(randomNumber % _myTiles.Count)].Fill = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));
            }
            else
                _myTiles[Convert.ToInt32(randomNumber % _myTiles.Count)].Fill = Brushes.DarkGreen; //#FF006400
        }
    }
}
