using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using POC_GestureNavigation.Pages;
using System.Windows;

namespace POC_GestureNavigation
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
            KinectRegion.SetKinectRegion(this, kinectRegion);
            ((App)Application.Current).kinectRegion = kinectRegion;
            ((App)Application.Current).lblTime = lblTime;
            ((App)Application.Current).btnBack = this.btnBack;
            ((App)Application.Current).lblTitle = this.lblTitle;
            ((App)Application.Current).tbSubTitle = this.tbSubTitle;
            this.frame.Navigate(new MainPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Navigate(new MainPage());
        }
    }
}
