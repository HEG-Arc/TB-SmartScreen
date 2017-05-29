using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Data;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private App app;
        private MultiSourceFrameReader msfr;
        private Body[] bodies;

        private IdentificationPage identificationPage = new IdentificationPage();

        public MainPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            msfr = app.msfr;

            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;

            if (!app.mainPage)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
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
            if (!app.mainPage)
            {
                app.mainPage = true;
                app.spIdentifiedPeople = this.IdentifiedUsers;
                app.spUnidentifiedPeople = this.UnidentifedList;
                app.mPage = this;
            }
            else
            {
                /*
                this.IdentifiedUsers = app.spIdentifiedPeople;
                this.UnidentifedList = app.spUnidentifiedPeople;
                */
            }

            app.timer.Tick += Timer_Tick;
            this.renderUnidentifedPeopleList();
            this.renderIdentifiedPeopleList();
            bodies = new Body[6];
        }

        private void renderUnidentifedPeopleList()
        {
            //app.spUnidentifiedPeople.Children.Clear();
            UnidentifedList.Children.Clear();
            for (int i = 0; i < app.unidentifiedPeople.Count; i++)
            {
                Grid grid = new Grid() { Width = 80, Height = 80, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + app.unidentifiedPeople[i].ToString("X6"))) };
                //app.spUnidentifiedPeople.Children.Add(grid);
                UnidentifedList.Children.Add(grid);
            }

            if (app.unidentifiedPeople.Count == 0 && this.btnIdentify.Visibility != Visibility.Hidden)
                this.btnIdentify.Visibility = Visibility.Hidden;
            if (app.unidentifiedPeople.Count > 0 && this.btnIdentify.Visibility != Visibility.Visible)
                this.btnIdentify.Visibility = Visibility.Visible;
        }

        private void renderIdentifiedPeopleList()
        {
            //app.spIdentifiedPeople.Children.Clear();
            IdentifiedUsers.Children.Clear();
            for (int i = 0; i < app.users.Count; i++)
            {
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                Grid grid = new Grid() { Width = 80, Height = 80, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + app.users[i].Color.ToString("X6"))), Margin = new Thickness(0, 10, 0, 0) };
                Label label = new Label() { Content = app.users[i].Code, FontSize = 20, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };

                sp.Children.Add(grid);
                sp.Children.Add(label);
                //app.spIdentifiedPeople.Children.Add(sp);
                IdentifiedUsers.Children.Add(sp);
            }
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            this.renderIdentifiedPeopleList();
            this.renderUnidentifedPeopleList();

            if (this.NavigationService != null)
            {
                MultiSourceFrame msf;
                bool oneBodyTracked = false;
                try
                {
                    // Update Lists                    

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
                                /*
                                if (!oneBodyTracked)
                                {
                                    if(!app.timer.IsEnabled)
                                        app.timer.Start();
                                }                                    
                                else
                                {
                                    if (app.timer.IsEnabled)
                                        app.timer.Stop();
                                }
                                */
                            }
                        }
                    }
                }
                catch
                { }
            }
        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
            else
            */
            //this.NavigationService.Navigate(new IdentificationPage());
            //this.NavigationService.Navigate(identificationPage);
            if (app.idPage != null)
                this.NavigationService.Navigate(app.idPage);
            else
                this.NavigationService.Navigate(new IdentificationPage());
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.timer.Tick -= Timer_Tick;
        }
    }
}