using SCE_ProductionChain.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCE_ProductionChain.Pages
{
    /// <summary>
    /// Logique d'interaction pour CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        private const int CALENDAR_DAYS = 5;
        private const int CALENDAR_HOURS = 10;

        private App app;

        private SolidColorBrush noWorkBrush;
        private SolidColorBrush WorkBrush;
        private SolidColorBrush spaceBrush;

        public CalendarPage()
        {
            InitializeComponent();

            app = (App)Application.Current;

            noWorkBrush = app.secondaryBrush;
            WorkBrush = new SolidColorBrush(Color.FromRgb(184, 0, 0));
            spaceBrush = app.backgroundBrush;

            this.Loaded += CalendarPage_Loaded;
        }

        private void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            initUI();
            initCalendar();

            if (app.calendarPage == null)
            {
                //msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.calendarPage = this;
            }
        }

        private void initUI()
        {
            for (int col = 3; col <= 11; col += 2)
            {
                for (int row = 3; row <= 21; row++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Fill = noWorkBrush,
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    gdCalendar.Children.Add(rect);
                }
            }
        }

        private void initCalendar()
        {
            User user = null;
            try
            {
                user = app.users[0];
                if (user != null)
                    drawCalendar(user.Calendar);
            }
            catch
            { }
        }

        private void drawCalendar(bool[,] calendar)
        {
            int nbSpace = 0;
            for (int d = 1; d <= CALENDAR_DAYS; d++)
            {
                for (int h = 0; h < CALENDAR_HOURS; h++)
                {
                    int col = d * 2 + 1;
                    int row = h + 3 + nbSpace;
                    var hourRect = (Rectangle)gdCalendar.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                    if (calendar[d - 1, h])
                    {
                        hourRect.Fill = WorkBrush;
                        if (row < 21)
                        {
                            var spaceRect = (Rectangle)gdCalendar.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row + 1 && Grid.GetColumn(e) == col);
                            if (calendar[d - 1, h + 1])
                                spaceRect.Fill = WorkBrush;
                            else
                                spaceRect.Fill = spaceBrush;
                        }
                    }
                    else
                    {
                        hourRect.Fill = noWorkBrush;
                        if (row < 21)
                        {
                            var spaceRect = (Rectangle)gdCalendar.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row + 1 && Grid.GetColumn(e) == col);
                            if (calendar[d - 1, h + 1])
                                spaceRect.Fill = spaceBrush;
                            else
                                spaceRect.Fill = noWorkBrush;
                        }
                    }
                    nbSpace++;
                }
                nbSpace = 0;
            }
        }
    }
}
