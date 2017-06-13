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
    /// Logique d'interaction pour StatisticPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        private App app;

        public StatisticsPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += StatisticsPage_Loaded;
        }

        private void StatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (app.statisticsPage == null)
            {
                // msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.identificationPage = this;
            }
        }
    }
}
