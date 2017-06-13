using System.Windows;
using System.Windows.Controls;

namespace SCE_ProductionChain.Pages
{
    /// <summary>
    /// Logique d'interaction pour CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        App app;

        public CalendarPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += CalendarPage_Loaded;
        }

        private void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (app.calendarPage == null)
            {
                //msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.calendarPage = this;
            }
        }
    }
}
