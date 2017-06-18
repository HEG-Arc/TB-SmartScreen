using SCE_ProductionChain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCE_ProductionChain.Pages
{
    /// <summary>
    /// Logique d'interaction pour ConfirmExchangePage.xaml
    /// </summary>
    public partial class ConfirmExchangePage : Page
    {
        private App app;
        public ConfirmExchangePage()
        {
            InitializeComponent();

            this.app = (App)Application.Current;
            this.Loaded += ConfirmExchangePage_Loaded;
            this.Unloaded += ConfirmExchangePage_Unloaded;
        }

        private void ConfirmExchangePage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onConfirmationPage = false;
        }

        private void ConfirmExchangePage_Loaded(object sender, RoutedEventArgs e)
        {
            app.onConfirmationPage = true;
            if (app.confirmExchangePage == null)
                app.confirmExchangePage = this;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            foreach (TimeSlotInfo tsi in app.timeSlotsToTransact)
            {
                //if(app.users[0].Equals(tsi.Worker))
                app.users[0].ReplaceHours(tsi);
                app.users[1].ReplaceHours(tsi);
            }
            app.timeSlotsToTransact.Clear();
            app.navigateToCalendarPage(this.NavigationService);
        }

        private void btnRefuse_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
