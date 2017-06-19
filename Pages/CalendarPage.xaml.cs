using SCE_ProductionChain.Model;
using SCE_ProductionChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private List<Button> ButtonsToRemove;
        private List<KeyValuePair<Button, TimeSlotInfo>> ButtonsReferencial { get; set; }

        public CalendarPage()
        {
            InitializeComponent();

            app = (App)Application.Current;

            noWorkBrush = app.secondaryBrush;
            WorkBrush = new SolidColorBrush(Color.FromRgb(184, 0, 0));
            spaceBrush = app.backgroundBrush;

            ButtonsReferencial = new List<KeyValuePair<Button, TimeSlotInfo>>();
            ButtonsToRemove = new List<Button>();

            this.Loaded += CalendarPage_Loaded;
            this.Unloaded += CalendarPage_Unloaded;
        }

        private void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            initUI();
            initCalendar();

            if (app.calendarPage == null)
                app.calendarPage = this;
            app.onCalendarPage = true;
            app.pageBeforeMultiUserExit = app.calendarPage;
        }

        private void initUI()
        {
            foreach (Button rect in ButtonsToRemove)
                gdCalendar.Children.Remove(rect);
        }

        private void initCalendar()
        {
            try
            {
                if (app.users.Count == 1)
                    DrawUserCalendar(app.users[0]);
                else if (app.users.Count > 1)
                {
                    DrawUsersCalendar(app.users[0], app.users[1]);
                    UpdateExchangeCircles(app.timeSlotsToTransact);
                }
                DrawUsernames(app.users);
            }
            catch
            { }
        }

        private void DrawUsernames(List<User> users)
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

        private void DrawUserCalendar(User user)
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
                    Button rectTimeSlot = new Button()
                    {
                        Style = FindResource("btnTimeSlot") as Style,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    if (timeSlot.IsWorking)
                        rectTimeSlot.Background = WorkBrush;
                    else
                        rectTimeSlot.Background = noWorkBrush;
                    Grid.SetRow(rectTimeSlot, row);
                    Grid.SetColumn(rectTimeSlot, col);
                    rectTimeSlot.SetValue(Grid.RowSpanProperty, timeSlot.Duration * 2 - 1);
                    rectTimeSlot.SetValue(Grid.ColumnSpanProperty, COLSPAN);
                    gdCalendar.Children.Add(rectTimeSlot);
                    ButtonsToRemove.Add(rectTimeSlot);

                    row += timeSlot.Duration * 2 - 1;
                    if (row < LAST_ROW)
                    {
                        Button rectEmptySpace = new Button()
                        {
                            Style = FindResource("btnTimeSlot") as Style,
                            Background = spaceBrush,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        Grid.SetRow(rectEmptySpace, row);
                        Grid.SetColumn(rectEmptySpace, col);
                        rectEmptySpace.SetValue(Grid.ColumnSpanProperty, 3);
                        gdCalendar.Children.Add(rectEmptySpace);
                        ButtonsToRemove.Add(rectEmptySpace);
                        row++;
                    }
                }
                row = INITIAL_ROW;
                col += COLSPAN + 1;
            }
        }

        private void DrawUsersCalendar(User user1, User user2)
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
            int rowPosition = 1;

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
                        Button rectTimeSlot1 = new Button()
                        {
                            Style = FindResource("btnTimeSlot") as Style,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Background = user1.Color
                        };
                        Button rectTimeSlot2 = new Button()
                        {
                            Style = FindResource("btnTimeSlot") as Style,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Background = user2.Color
                        };

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
                        DrawBlock(user1.Color, row, col, rowspan, COLSPAN, true,
                                  new TimeSlotInfo(user1, user2, d, rowPosition, rowPosition + currentDuretion - 1,
                                                   row, col, rowspan, COLSPAN));
                    else if (!timeSlot1.IsWorking && timeSlot2.IsWorking)
                        DrawBlock(user2.Color, row, col, rowspan, COLSPAN, true,
                                  new TimeSlotInfo(user2, user1, d, rowPosition, rowPosition + currentDuretion - 1,
                                                   row, col, rowspan, COLSPAN));
                    else
                        DrawBlock(noWorkBrush, row, col, rowspan, COLSPAN);

                    row += rowspan;
                    if (row < LAST_ROW)
                        DrawBlock(spaceBrush, row, col);
                    row++;
                    rowPosition += currentDuretion;

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
                rowPosition = 1;
            }
        }

        private void DrawBlock(SolidColorBrush color, int row, int col, int rowspan = 0, int colspan = 0, bool exchangable = false, TimeSlotInfo timeSlotInfo = null)
        {
            Button rect;
            if (exchangable)
            {
                rect = new Button()
                {
                    Style = FindResource("btnTimeSlot") as Style,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = color
                };
                rect.Click += Rect_Click;
            }
            else
                rect = new Button()
                {
                    Style = FindResource("btnTimeSlot") as Style,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = color
                };

            Grid.SetRow(rect, row);
            Grid.SetColumn(rect, col);
            if (rowspan > 0)
                rect.SetValue(Grid.RowSpanProperty, rowspan);
            if (colspan > 0)
                rect.SetValue(Grid.ColumnSpanProperty, colspan);
            gdCalendar.Children.Add(rect);

            if (timeSlotInfo != null)
                ButtonsReferencial.Add(new KeyValuePair<Button, TimeSlotInfo>(rect, timeSlotInfo));
        }

        private void Rect_Click(object sender, RoutedEventArgs e)
        {
            TimeSlotInfo tsi = new TimeSlotInfo();

            // Récupération des informations de la plage horaire
            foreach (KeyValuePair<Button, TimeSlotInfo> kv in this.ButtonsReferencial)
            {
                if (kv.Key.Equals((Button)sender))
                    tsi = kv.Value;
            }

            if (app.timeSlotsToTransact.Contains(tsi))
                app.timeSlotsToTransact.Remove(tsi);
            else
                app.timeSlotsToTransact.Add(tsi);

            UpdateExchangeCircles(app.timeSlotsToTransact);
        }

        private void RemoveAllExchangeCircles()
        {
            List<UIElement> elementToRemove = new List<UIElement>();
            for (int i = 0; i < gdCalendar.Children.Count; i++)
            {
                if (gdCalendar.Children[i] is Image)
                    elementToRemove.Add(gdCalendar.Children[i]);
            }

            foreach (UIElement uiElement in elementToRemove)
                gdCalendar.Children.Remove(uiElement);
        }

        private void UpdateExchangeCircles(List<TimeSlotInfo> timeSlotsToTransact)
        {
            RemoveAllExchangeCircles();

            int exchangeCircleSize = 0;
            foreach (TimeSlotInfo tsi in timeSlotsToTransact)
            {
                exchangeCircleSize = 27 * ((tsi.To - tsi.From > 0) ? tsi.To - tsi.From : 1);
                Image imgExchangeCircle = new Image()
                {
                    Source = new BitmapImage(new System.Uri("../Images/exchange_circle.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = exchangeCircleSize,
                    Width = exchangeCircleSize
                };
                Grid.SetRow(imgExchangeCircle, tsi.GridRow);
                Grid.SetColumn(imgExchangeCircle, tsi.GridColumn);
                if (tsi.GridRowspan > 0)
                    imgExchangeCircle.SetValue(Grid.RowSpanProperty, tsi.GridRowspan);
                if (tsi.GridColspan > 0)
                    imgExchangeCircle.SetValue(Grid.ColumnSpanProperty, tsi.GridColspan);
                gdCalendar.Children.Add(imgExchangeCircle);
            }
        }

        private void CalendarPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onCalendarPage = false;
        }
    }
}
