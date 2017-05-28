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

namespace POC_GestureNavigation.Pages
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private App app;

        public MainPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            String scores = "";

            scores += "Object : ";
            if (app.objectScore != null)
                scores += app.objectScore;
            else
                scores += "-";

            tbxScores.Text = scores;
        }

        private void btnComponents_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ComponentsPage());
        }

        private void btnObject_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ObjectPage());
        }
    }
}
