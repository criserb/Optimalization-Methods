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
        private Grid myGrid;
        private List<Rectangle> myTiles;
        private SolidColorBrush newBrush;

        public RandomWalk(List<Rectangle> myTiles)
        {
            this.myTiles = myTiles;
        }

        public void Update(long randomNumber)
        {
                newBrush = (SolidColorBrush)myTiles[Convert.ToInt32(randomNumber % myTiles.Count)].Fill;
                int R = newBrush.Color.R;
                R = (R + 22) % 255;
                int G = newBrush.Color.G;
                G = (G + 160) % 255;
                int B = newBrush.Color.B;
                B = (B + 133) % 255;
                myTiles[Convert.ToInt32(randomNumber % myTiles.Count)].Fill = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B)); 
        }
    }
}
