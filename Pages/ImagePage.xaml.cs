using System.Windows.Controls;
using Microsoft.Kinect;
using System.Windows.Media;

namespace POC_GestureNavigation.Pages
{
    /// <summary>
    /// Logique d'interaction pour ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page
    {
        public ImagePage()
        {
            InitializeComponent();
            this.Loaded += ImagePage_Loaded;
        }

        private void ImagePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            slvImage.LayoutTransform = new ScaleTransform(1,1);
        }
    }
}
