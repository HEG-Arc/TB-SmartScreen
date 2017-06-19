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
    /// Logique d'interaction pour ConfirmUserExitPage.xaml
    /// </summary>
    public partial class ConfirmUserExitPage : Page
    {
        App app;
        public ConfirmUserExitPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += ConfirmUserExitPage_Loaded;
            this.Unloaded += ConfirmUserExitPage_Unloaded;
        }        

        private void ConfirmUserExitPage_Loaded(object sender, RoutedEventArgs e)
        {
            app.onConfirUserExitPage = true;
            this.lblTitle.Content = Properties.Resources.ConfirmUserExitTitle;
            this.tbQuestion.Text = Properties.Resources.ConfirmUserExitQuestion;

            if (app.confirmUserExitPage != null)
                app.confirmUserExitPage = this;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToIdentificationPage(this.NavigationService);
        }

        private void btnRefuse_Click(object sender, RoutedEventArgs e)
        {
            app.timeSlotsToTransact.Clear();
            app.navigateToPageBeforeExit(this.NavigationService);
        }

        private void ConfirmUserExitPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onConfirUserExitPage = false;
        }
    }
}
