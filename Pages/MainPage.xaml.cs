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
            this.renderUnidentifedPeopleList();
            this.renderIdentifiedPeopleList();
            bodies = new Body[6];            
        }

        private void renderUnidentifedPeopleList()
        {
            this.UnidentifedList.Children.Clear();
            for(int i = 0; i < app.currentPeople.Count; i++)
            {
                Grid grid = new Grid() { Width = 80, Height = 80, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + app.currentPeople[i].ToString("X6"))) };
                this.UnidentifedList.Children.Add(grid);
            }
        }
        
        private void renderIdentifiedPeopleList()
        {
            this.IdentifiedUsers.Children.Clear();
            for (int i = 0; i < app.users.Count; i++)
            {
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                Grid grid = new Grid() { Width = 80, Height = 80, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + app.users[i].Color.ToString("X6"))), Margin = new Thickness(0,10,0,0) };
                Label label = new Label() { Content = app.users[i].Code, FontSize = 20, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10,0,0,0) };

                sp.Children.Add(grid);
                sp.Children.Add(label);
                IdentifiedUsers.Children.Add(sp);
            }            
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