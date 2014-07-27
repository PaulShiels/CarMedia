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
using System.Windows.Threading;

namespace CarMedia
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void btnRadio_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            if (!Music.mediaPlayerIsPlaying && !Music.mediaPaused && !Music.mediaStopped)
            {
                NavigationService.Navigate(MainWindow.musicPage);
            }
            else
            {
                NavigationService.Navigate(MainWindow.musicPage);
                //musicPage.Visibility = Visibility.Visible;
                
            }
        }
          
    }
}
