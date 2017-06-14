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
        private const int TITLE_FONT_SIZE = 38;
        private const int CONTENT_FONT_SIZE = 124;
        private const int TEXT_BLOCK_SMALL_WIDTH = 315;
        private const int TEXT_BLOCK_LARGE_WIDTH = 550;
        private const int TITLE_MARGIN_LEFT = 25;

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
            initSingleUserUI();

            if (app.statisticsPage == null)
            {
                app.identificationPage = this;
            }
        }

        private void initSingleUserUI()
        {
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

            this.titleNbHours.Children.Add(rectTitles[0]);
            this.titleNbHours.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsNbHoursTitle, Width = TEXT_BLOCK_SMALL_WIDTH, FontSize = TITLE_FONT_SIZE, Margin = new Thickness(TITLE_MARGIN_LEFT,0,0,0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
            this.titleNbPieces.Children.Add(rectTitles[1]);
            this.titleNbPieces.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsNbPiecesTitle, Width = TEXT_BLOCK_SMALL_WIDTH, FontSize = TITLE_FONT_SIZE, Margin = new Thickness(TITLE_MARGIN_LEFT, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
            this.titleRevenuPerHour.Children.Add(rectTitles[2]);
            this.titleRevenuPerHour.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsRevenuPerHoursTitle, Width = TEXT_BLOCK_SMALL_WIDTH, FontSize = TITLE_FONT_SIZE, Margin = new Thickness(TITLE_MARGIN_LEFT, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
            this.titleEstimatedSalary.Children.Add(rectTitles[3]);
            this.titleEstimatedSalary.Children.Add(new TextBlock() { Text = Properties.Resources.StatisticsEstimationSalaryTitle, Width = TEXT_BLOCK_LARGE_WIDTH, FontSize = TITLE_FONT_SIZE, Margin = new Thickness(TITLE_MARGIN_LEFT, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });

            this.contentNbHours.Children.Add(rectContents[0]);
            this.contentNbHours.Children.Add(new Label() { Content = "64", FontSize = CONTENT_FONT_SIZE, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            this.contentNbPieces.Children.Add(rectContents[1]);
            this.contentNbPieces.Children.Add(new Label() { Content = "1'456", FontSize = CONTENT_FONT_SIZE, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            this.contentRevenuPerHour.Children.Add(rectContents[2]);
            this.contentRevenuPerHour.Children.Add(new Label() { Content = "22.-", FontSize = CONTENT_FONT_SIZE, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            this.contentEstimatedSalary.Children.Add(rectContents[3]);
            this.contentEstimatedSalary.Children.Add(new Label() { Content = "~ 3'565.-", FontSize = CONTENT_FONT_SIZE, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
        }
    }
}
