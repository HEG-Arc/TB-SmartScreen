using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        App app;
        private string username;
        private MultiSourceFrameReader msfr;
        private Body[] bodies;

        public MainPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            msfr = app.msfr;

            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
            app.timer.Tick += Timer_Tick;

            if(app.cptMsfrE == 2)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.cptMsfrE++;
            }            
        }        

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
            else
                this.NavigationService.Navigate(new IdentificationPage());
            app.timer.Stop();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            username = app.User.Value;            
            this.lblUser.Content = "Welcome, " + username;            
            bodies = new Body[6];            
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if(this.NavigationService != null)
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
                                lblDebug.Content = "debug : " + oneBodyTracked;
                                if (!oneBodyTracked)
                                    app.timer.Start();
                                else
                                {
                                    if (app.timer.IsEnabled)
                                        app.timer.Stop();
                                }

                            }
                        }
                    }
                }
                catch
                { }
            }            
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.timer.Tick -= Timer_Tick;
        }
    }
}