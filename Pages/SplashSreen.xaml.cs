using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SCE_ProductionChain.Pages
{
    /// <summary>
    /// Logique d'interaction pour SplashSreen.xaml
    /// </summary>
    public partial class SplashSreen : Page
    {
        private App app;
        private MultiSourceFrameReader msfr;
        private DispatcherTimer timer;

        public SplashSreen()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += SplashSreen_Loaded;
            this.Unloaded += SplashSreen_Unloaded;
        }        

        private void SplashSreen_Loaded(object sender, RoutedEventArgs e)
        {
            app.onSplashScreen = true;

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            initUI();

            msfr = app.msfr;
            if (app.splashScreen == null)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.splashScreen = this;
            }
        }

        private void initUI()
        {
            this.imgLogo.Visibility = Visibility.Visible;
            this.lblWelcome.Content = "";
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            this.timer.Stop();
            app.navigateToIdentificationPage(this.NavigationService);
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if(this.NavigationService != null)
            {
                if (app.userInImplicitZone)
                {
                    displayWelcome();
                    this.timer.Start();
                }                
            }            
        }

        private void displayWelcome()
        {
            this.imgLogo.Visibility = Visibility.Hidden;
            this.lblWelcome.Content = "Bienvenue";
            this.lblInstruction.Visibility = Visibility.Hidden;           
        }

        private void SplashSreen_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onSplashScreen = false;
        }
    }
}
