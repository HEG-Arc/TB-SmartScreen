using Microsoft.Kinect;
using POC_MultiUserIndification_Collider;
using POC_MultiUserIndification_Collider.Model;
using POC_MultiUserIndification_Collider.Pages;
using System.Windows;
using System.Windows.Controls;

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
            lvUsersIdentified.Items.Clear();
            foreach (User user in app.users)
            {
                lvUsersIdentified.Items.Add(user.Username);
            }
            lblNbUnidentified.Content = app.trackedBodies.Count;
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
