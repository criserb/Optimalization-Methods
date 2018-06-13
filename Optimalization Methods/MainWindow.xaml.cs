using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Optimalization_Methods.Menu;
using Optimalization_Methods.Menu.Randomness;

namespace Optimalization_Methods
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowHideMenu(string storyboardhide, Button myButton, Button myButton2, StackPanel pnl)
        {
            Storyboard sb = Resources[storyboardhide] as Storyboard;
            sb.Begin(pnl);

            if (storyboardhide.Contains("Show"))
            {
                myButton.Visibility = System.Windows.Visibility.Visible;
                myButton2.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (storyboardhide.Contains("Hide"))
            {
                myButton.Visibility = System.Windows.Visibility.Hidden;
                myButton2.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Content = new LcgView();
            TitleLabel.Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            myFrame.Content = new CauchyView();
            TitleLabel.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            myFrame.Content = new FireflyView();
            TitleLabel.Visibility = Visibility.Hidden;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            myFrame.Content = new PsoView();
            TitleLabel.Visibility = Visibility.Hidden;
        }
    }
}
