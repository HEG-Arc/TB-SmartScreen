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
    /// Logique d'interaction pour ComponentsPage.xaml
    /// </summary>
    public partial class ComponentsPage : Page
    {
        public ComponentsPage()
        {
            InitializeComponent();
            this.Loaded += ComponentsPage_Loaded;
        }

        private void ComponentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).btnBack.Visibility = Visibility.Visible;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;

            switch(rb.Content.ToString())
            {
                case "Blue":
                    gridColor.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    break;
                case "Red":
                    gridColor.Background = new SolidColorBrush(Color.FromRgb(255,0,0));
                    break;
                case "Green":
                    gridColor.Background = new SolidColorBrush(Color.FromRgb(0,255,0));
                    break;
            }            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            string text = "";

            bool a = cbA.IsChecked ?? false;
            bool b = cbB.IsChecked ?? false;
            bool c = cbC.IsChecked ?? false;

            if (a)
                text += "A";
            if (b)
                text += "B";
            if (c)
                text += "C";

            lblABC.Content = text;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Decimal d = new Decimal(Slider.Value);
            lblSliderValue.Content = Math.Round(d, 0);
        }
    }
}