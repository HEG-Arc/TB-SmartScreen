using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private string username;
        private MultiSourceFrameReader msfr;
        private Body[] bodies;

        public MainPage()
        {
            InitializeComponent();
            username = ((App)Application.Current).User.Value;
            msfr = ((App)Application.Current).msfReader;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblUser.Content = "Welcome, " + username;

            bodies = new Body[6];
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf;
            bool oneBodyTracked = false;
            try
            {
                msf = e.FrameReference.AcquireFrame();
                if (msf != null)
                {
                    using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                    {
                        if (bodyFrame != null)
                        {
                            bodyFrame.GetAndRefreshBodyData(bodies);
                            foreach (Body body in bodies)
                            {
                                if (body.IsTracked)
                                    oneBodyTracked = true;
                            }
                        }
                    }
                }
            }
            catch
            { }
            lblDebug.Content = "debug : " + oneBodyTracked;
        }

        private void Navigate (Page page)
        {
            this.NavigationService.Navigate(page);
        }
    }
}
