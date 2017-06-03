using Microsoft.Kinect;
using POC_MultiUserIdification_Collider.Util;
using POC_MultiUserIndification_Collider;
using POC_MultiUserIndification_Collider.Model;
using POC_MultiUserIndification_Collider.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POC_MultiUserIdification_Collider.Pages
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private App app;
        private MultiSourceFrameReader msfr;

        public MainPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            msfr = app.msfr;
            if (app.mainPage == null)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.mainPage = this;
            }            
        }

        private void updateUI()
        {
            spIdentifiedList.Children.Clear();
            for (int i = 0; i < app.users.Count; i++)
            {
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                Grid grid = new Grid() { Width = 80, Height = 80, Background = Drawer.BodyColors[i], Margin = new Thickness(0, 10, 0, 0) };
                Label label = new Label() { Content = app.users[i].Username, FontSize = 20, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };

                sp.Children.Add(grid);
                sp.Children.Add(label);
                                
                spIdentifiedList.Children.Add(sp);
            }

            lblNbUnidentified.Content = app.unidentifiedBodies.Count;
            //updateIdentifyButton();
        }

        private void updateIdentifyButton()
        {
            if(app.unidentifiedBodies.Count > 0 && btnIdentify.Visibility != Visibility.Visible)
                btnIdentify.Visibility = Visibility.Visible;

            if (app.unidentifiedBodies.Count == 0 && btnIdentify.Visibility != Visibility.Hidden)
                btnIdentify.Visibility = Visibility.Hidden;
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if(this.NavigationService != null)
                updateUI();
        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            if(app.identificationPage != null)
                this.NavigationService.Navigate(app.identificationPage);
            else
                this.NavigationService.Navigate(new IdentificationPage());
        }
    }
}
