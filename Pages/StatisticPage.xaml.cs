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
    /// Logique d'interaction pour StatisticPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        private App app;

        private Rectangle[] rectTitles;
        private Rectangle[] rectContents;

        public StatisticsPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += StatisticsPage_Loaded;
        }

        private void StatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            resetUI();
            if (app.users.Count > 1)
                initMultiUserUI();
            else
                initSingleUserUI();

            if (app.statisticsPage == null)
            {
                app.identificationPage = this;
            }
        }

        private void resetUI()
        {
            this.singleUserTitleNbHours.Children.Clear();
            this.singleUserTitleNbPieces.Children.Clear();
            this.singleUserTitleRevenuPerHour.Children.Clear();
            this.singleUserTitleEstimatedSalary.Children.Clear();
            this.singleUserContentNbHours.Children.Clear();
            this.singleUserContentNbPieces.Children.Clear();
            this.singleUserContentRevenuPerHour.Children.Clear();
            this.singleUserContentEstimatedSalary.Children.Clear();

            this.multiUserTitleNbPiecesHours.Children.Clear();
            this.multiUserTitleEstimatedSalary.Children.Clear();
            this.multiUserContentNbPiecesHours.Children.Clear();
            this.multiUserContentEstimatedSalary.Children.Clear();

            this.gdSingleUser.Visibility = Visibility.Hidden;
            this.gdMultiUser.Visibility = Visibility.Hidden;
        }

        private void initSingleUserUI()
        {
            User currentUser = app.users[0];
            gdSingleUser.Visibility = Visibility.Visible;
            
            rectTitles = new Rectangle[4];
            rectContents = new Rectangle[4];
            for (int i = 0; i < 4; i++)
            {
                rectTitles[i] = new Rectangle()
                {
                    Fill = app.primaryBrush,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                rectContents[i] = new Rectangle()
                {
                    Fill = app.secondaryBrush,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
            }

            this.singleUserTitleNbHours.Children.Add(rectTitles[0]);
            this.singleUserTitleNbHours.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsNbHoursTitle, Style = FindResource("tbStatisticsTitle") as Style });
            this.singleUserTitleNbPieces.Children.Add(rectTitles[1]);
            this.singleUserTitleNbPieces.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsNbPiecesTitle, Style = FindResource("tbStatisticsTitle") as Style });
            this.singleUserTitleRevenuPerHour.Children.Add(rectTitles[2]);
            this.singleUserTitleRevenuPerHour.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsRevenuPerHoursTitle, Style = FindResource("tbStatisticsTitle") as Style });
            this.singleUserTitleEstimatedSalary.Children.Add(rectTitles[3]);
            this.singleUserTitleEstimatedSalary.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsEstimationSalaryTitle, Style = FindResource("tbStatisticsTitleLarge") as Style });

            this.singleUserContentNbHours.Children.Add(rectContents[0]);
            this.singleUserContentNbHours.Children.Add(new Label() { Content = String.Format("{0:# ###}", currentUser.Statistics.NbHoursWorked), Style = FindResource("lblStatisticsContent") as Style });
            this.singleUserContentNbPieces.Children.Add(rectContents[1]);
            this.singleUserContentNbPieces.Children.Add(new Label() { Content = String.Format("{0:# ###}", currentUser.Statistics.NbPiecesWorked), Style = FindResource("lblStatisticsContent") as Style });
            this.singleUserContentRevenuPerHour.Children.Add(rectContents[2]);
            this.singleUserContentRevenuPerHour.Children.Add(new Label() { Content = String.Format("{0:# ###}", currentUser.Statistics.RevenuePerHour) + ".-", Style = FindResource("lblStatisticsContent") as Style });
            this.singleUserContentEstimatedSalary.Children.Add(rectContents[3]);
            this.singleUserContentEstimatedSalary.Children.Add(new Label() { Content = String.Format("{0:~# ###}", currentUser.Statistics.SalaryEstimation) + ".-", Style = FindResource("lblStatisticsContent") as Style });
        }

        private void initMultiUserUI()
        {
            this.gdMultiUser.Visibility = Visibility.Visible;

            rectTitles = new Rectangle[2];
            rectContents = new Rectangle[2];
            for (int i = 0; i < 2; i++)
            {
                rectTitles[i] = new Rectangle()
                {
                    Fill = app.primaryBrush,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                rectContents[i] = new Rectangle()
                {
                    Fill = app.secondaryBrush,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
            }

            this.multiUserTitleNbPiecesHours.Children.Add(rectTitles[0]);
            this.multiUserTitleNbPiecesHours.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsNbHoursTitle, Style = FindResource("tbStatisticsTitleLarge") as Style });
            this.multiUserTitleEstimatedSalary.Children.Add(rectTitles[1]);
            this.multiUserTitleEstimatedSalary.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsEstimationSalaryTitle, Style = FindResource("tbStatisticsTitleLarge") as Style });

            this.multiUserContentNbPiecesHours.Children.Add(rectContents[0]);
            //this.multiUserContentNbPiecesHours.Children.Add(new Label() { Content = "64", Style = FindResource("lblStatisticsContent") as Style });
            this.multiUserContentEstimatedSalary.Children.Add(rectContents[1]);
            //this.multiUserContentEstimatedSalary.Children.Add(new Label() { Content = "~ 3'565.-", Style = FindResource("lblStatisticsContentLarge") as Style });
        }
    }
}
