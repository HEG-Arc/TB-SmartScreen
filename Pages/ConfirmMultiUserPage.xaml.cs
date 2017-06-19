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
    /// Logique d'interaction pour ConfirmMultiUserPage.xaml
    /// </summary>
    public partial class ConfirmMultiUserPage : Page
    {
        private App app;
        public ConfirmMultiUserPage()
        {
            InitializeComponent();
            this.app = (App)Application.Current;
            this.Loaded += ConfirmMultiUserPage_Loaded;
            this.Unloaded += ConfirmMultiUserPage_Unloaded;
        }        

        private void ConfirmMultiUserPage_Loaded(object sender, RoutedEventArgs e)
        {
            app.onConfirmMultiuserPage = true;
            this.lblTitle.Content = Properties.Resources.ConfirmMultiuserTitle;
            this.tbContent.Text = Properties.Resources.ConfirmMultiuserText;
            this.lblQuestion.Content = Properties.Resources.ConfirmMultiuserQuestion;

            if (app.confirmMultiuserPage == null)
                app.confirmMultiuserPage = this;
        }

        private void btnRefuse_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToCalendarPage(this.NavigationService);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToIdentificationPage(this.NavigationService);
        }

        private void ConfirmMultiUserPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onConfirmMultiuserPage = false;
        }
    }
}
