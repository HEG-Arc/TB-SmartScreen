using SCE_ProductionChain.Model;
using System.Collections.Generic;
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

        private const int INITIAL_ROW = 3;
        private const int INITIAL_COL = 3;
        private const int LAST_ROW = 21;

        private App app;

        private SolidColorBrush noWorkBrush;
        private SolidColorBrush WorkBrush;
        private SolidColorBrush spaceBrush;

        private List<Rectangle> rectanglesToRemove;

        public CalendarPage()
        {
            InitializeComponent();

            app = (App)Application.Current;

            noWorkBrush = app.secondaryBrush;
            WorkBrush = new SolidColorBrush(Color.FromRgb(184, 0, 0));
            spaceBrush = app.backgroundBrush;

            rectanglesToRemove = new List<Rectangle>();

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
            foreach(Rectangle rect in rectanglesToRemove)
                gdCalendar.Children.Remove(rect);
        }

        private void initCalendar()
        {
            User user = null;
            try
            {
                user = app.users[0];
                if (user != null)
                    drawUserCalendar(user);
                drawUsernames(app.users);
            }
            catch
            { }
        }

        private void drawUsernames(List<User> users)
        {
            this.spUsernames.Children.Clear();
            foreach (User user in users)
            {
                Grid grid = new Grid() { Background = user.Color, Style = FindResource("gdUserName") as Style };
                Label label = new Label() { Content = user.Username, Style = FindResource("lblUsername") as Style };

                this.spUsernames.Children.Add(grid);
                this.spUsernames.Children.Add(label);
            }
        }

        private void drawUserCalendar(User user)
        {
            this.WorkBrush = user.Color;

            int row = INITIAL_ROW;
            int col = INITIAL_COL;
            for(int d = 0; d < user.Calendar.Days.Count; d++)
            {
                Day day = user.Calendar.Days[d];
                for (int t = 0; t < user.Calendar.Days[d].TimeSlots.Count; t++)
                {
                    TimeSlot timeSlot = user.Calendar.Days[d].TimeSlots[t];
                    Rectangle rectTimeSlot = new Rectangle()
                    {
                        Stretch = Stretch.Fill
                    };

                    if (timeSlot.IsWorking)
                        rectTimeSlot.Fill = WorkBrush;
                    else
                        rectTimeSlot.Fill = noWorkBrush;
                    Grid.SetRow(rectTimeSlot, row);
                    Grid.SetColumn(rectTimeSlot, col);
                    rectTimeSlot.SetValue(Grid.RowSpanProperty, timeSlot.Duration * 2 - 1);
                    gdCalendar.Children.Add(rectTimeSlot);
                    rectanglesToRemove.Add(rectTimeSlot);

                    row += timeSlot.Duration * 2 - 1;
                    if(row < LAST_ROW)
                    {
                        Rectangle rectEmptySpace = new Rectangle()
                        {
                            Fill = spaceBrush,
                            Stretch = Stretch.Fill
                        };
                        Grid.SetRow(rectEmptySpace, row);
                        Grid.SetColumn(rectEmptySpace, col);
                        gdCalendar.Children.Add(rectEmptySpace);
                        rectanglesToRemove.Add(rectEmptySpace);
                        row++;
                    }                    
                }
                row = INITIAL_ROW;
                col += 2;
            }
        }
    }
}
