using SCE_ProductionChain.Model;
using SCE_ProductionChain.Util;
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
        private const int COLSPAN = 3;

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
            foreach (Rectangle rect in rectanglesToRemove)
                gdCalendar.Children.Remove(rect);
        }

        private void initCalendar()
        {
            User user = null;
            try
            {
                //user = app.users[0];
                app.availableUsers[0].Color = Drawer.BodyColors[2];
                app.availableUsers[1].Color = Drawer.BodyColors[3];
                app.users.Add(app.availableUsers[0]);
                app.users.Add(app.availableUsers[1]);
                drawUsersCalendar(app.users[0], app.users[1]);
                //drawUserCalendar(app.users[1]);
                /*
                if (user != null)
                    drawUserCalendar(user);
                    */
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
            for (int d = 0; d < user.Calendar.Days.Count; d++)
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
                    rectTimeSlot.SetValue(Grid.ColumnSpanProperty, COLSPAN);
                    gdCalendar.Children.Add(rectTimeSlot);
                    rectanglesToRemove.Add(rectTimeSlot);

                    row += timeSlot.Duration * 2 - 1;
                    if (row < LAST_ROW)
                    {
                        Rectangle rectEmptySpace = new Rectangle()
                        {
                            Fill = spaceBrush,
                            Stretch = Stretch.Fill
                        };
                        Grid.SetRow(rectEmptySpace, row);
                        Grid.SetColumn(rectEmptySpace, col);
                        rectEmptySpace.SetValue(Grid.ColumnSpanProperty, 3);
                        gdCalendar.Children.Add(rectEmptySpace);
                        rectanglesToRemove.Add(rectEmptySpace);
                        row++;
                    }
                }
                row = INITIAL_ROW;
                col += COLSPAN + 1;
            }
        }

        private void drawUsersCalendar(User user1, User user2)
        {
            int row = INITIAL_ROW;
            int col = INITIAL_COL;
            int index1 = 0;
            int index2 = 0;
            bool doesContinue = true;
            TimeSlot timeSlot1 = null;
            TimeSlot timeSlot2 = null;
            int duretion1 = 0;
            int duretion2 = 0;

            for (int d = 0; d < Model.Calendar.DaysCount; d++)
            {
                Day dayUser1 = user1.Calendar.Days[d];
                Day dayUser2 = user2.Calendar.Days[d];

                while (doesContinue)
                {
                    if (index1 < dayUser1.TimeSlots.Count)
                    {
                        timeSlot1 = dayUser1.TimeSlots[index1];
                        if (duretion1 <= 0)
                            duretion1 = timeSlot1.Duration;
                    }
                    if (index2 < dayUser2.TimeSlots.Count)
                    {
                        timeSlot2 = dayUser2.TimeSlots[index2];
                        if (duretion2 <= 0)
                            duretion2 = timeSlot2.Duration;
                    }

                    int currentDuretion = 0;
                    if (duretion1 <= duretion2)
                        currentDuretion = duretion1;
                    else
                        currentDuretion = duretion2;

                    int rowspan = currentDuretion * 2 - 1;
                    if (timeSlot1.IsWorking && timeSlot2.IsWorking)
                    {
                        Rectangle rectTimeSlot1 = new Rectangle() { Stretch = Stretch.Fill, Fill = user1.Color };
                        Rectangle rectTimeSlot2 = new Rectangle() { Stretch = Stretch.Fill, Fill = user2.Color };

                        Grid.SetRow(rectTimeSlot1, row);
                        Grid.SetColumn(rectTimeSlot1, col);
                        rectTimeSlot1.SetValue(Grid.RowSpanProperty, rowspan);
                        gdCalendar.Children.Add(rectTimeSlot1);

                        Grid.SetRow(rectTimeSlot2, row);
                        Grid.SetColumn(rectTimeSlot2, col + 2);
                        rectTimeSlot2.SetValue(Grid.RowSpanProperty, rowspan);
                        gdCalendar.Children.Add(rectTimeSlot2);
                    }
                    else if (timeSlot1.IsWorking && !timeSlot2.IsWorking)
                        DrawBlock(user1.Color, row, col, rowspan, COLSPAN);
                    else if (!timeSlot1.IsWorking && timeSlot2.IsWorking)
                        DrawBlock(user2.Color, row, col, rowspan, COLSPAN);
                    else
                        DrawBlock(noWorkBrush, row, col, rowspan, COLSPAN);

                    row += rowspan;
                    if (row < LAST_ROW)
                        DrawBlock(spaceBrush, row, col);
                    row++;

                    duretion1 -= currentDuretion;
                    duretion2 -= currentDuretion;
                    if (duretion1 <= 0)
                        index1++;
                    if (duretion2 <= 0)
                        index2++;

                    if (index1 >= dayUser1.TimeSlots.Count && index2 >= dayUser2.TimeSlots.Count)
                        doesContinue = false;
                }
                doesContinue = true;
                index1 = 0;
                index2 = 0;
                row = INITIAL_ROW;
                col += COLSPAN + 1;
            }
        }

        private void DrawBlock(SolidColorBrush color, int row, int col, int rowspan = 0, int colspan = 0)
        {
            Rectangle rect = new Rectangle() { Stretch = Stretch.Fill, Fill = color };

            Grid.SetRow(rect, row);
            Grid.SetColumn(rect, col);
            if (rowspan > 0)
                rect.SetValue(Grid.RowSpanProperty, rowspan);
            if (colspan > 0)
                rect.SetValue(Grid.ColumnSpanProperty, colspan);
            gdCalendar.Children.Add(rect);
        }
    }
}
