using Microsoft.Kinect;
using SCE_ProductionChain.Model;
using SCE_ProductionChain.Pages;
using SCE_ProductionChain.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCE_ProductionChain
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int _CALENDAR_DAYS = 5;

        private const int _CALENDAR_HOURS = 10;

        private const int _LIMIT_USERS = 2;

        private const double _IMPLICIT_ZONE = 1.75;

        internal Page splashScreen;

        internal Page identificationPage;

        internal Page calendarPage;

        internal Page statisticsPage;

        internal Page confirmMultiuserPage;

        internal Page confirmExchangePage;

        internal Page confirmUserExitPage;

        internal Page pageBeforeMultiUserExit;
        internal List<User> users { get; set; }
        internal List<ulong> trackedBodies { get; set; }
        internal List<ulong> unidentifiedBodies { get; set; }
        internal MultiSourceFrameReader msfr { get; set; }
        internal bool onIdentificationPage { get; set; }
        internal bool onConfirUserExitPage { get; set; }
        internal bool onConfirmMultiuserPage { get; set; }
        internal bool onConfirmExchangePage { get; set; }
        internal bool onCalendarPage { get; set; }
        internal bool onStatisticsPage { get; set; }
        internal bool onSplashScreen { get; set; }
        internal bool userTwoLoggedOut { get; set; }
        internal bool userInImplicitZone { get; set; }
        internal List<TimeSlotInfo> timeSlotsToTransact { get; set; }

        internal SolidColorBrush primaryBrush { get; set; }
        internal SolidColorBrush secondaryBrush { get; set; }
        internal SolidColorBrush backgroundBrush { get; set; }

        internal List<User> availableUsers { get; set; }

        public App()
        {
            this.users = new List<User>();
            this.trackedBodies = new List<ulong>();
            this.unidentifiedBodies = new List<ulong>();
            this.onIdentificationPage = false;
            this.onCalendarPage = false;
            this.onStatisticsPage = false;
            this.userTwoLoggedOut = false;
            this.onSplashScreen = false;
            this.availableUsers = new GenerateUsers().getUsers();
            this.timeSlotsToTransact = new List<TimeSlotInfo>();

            primaryBrush = new SolidColorBrush(Color.FromRgb(77, 77, 77));
            secondaryBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            backgroundBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        public void navigateToCalendarPage(NavigationService ns)
        {
            if (calendarPage != null)
                ns.Navigate(calendarPage);
            else
                ns.Navigate(new CalendarPage());
        }

        public void navigateToCalendarPage(Frame frame)
        {
            if (calendarPage != null)
                frame.Navigate(calendarPage);
            else
                frame.Navigate(new CalendarPage());
        }

        public void navigateToPageBeforeExit(NavigationService ns)
        {
            if (pageBeforeMultiUserExit != null)
                ns.Navigate(pageBeforeMultiUserExit);
            else
                ns.Navigate(new CalendarPage());
        }

        public void navigateToPageBeforeExit(Frame frame)
        {
            if (pageBeforeMultiUserExit != null)
                frame.Navigate(pageBeforeMultiUserExit);
            else
                frame.Navigate(new CalendarPage());
        }

        public void navigateToConfirmMultiuserPage(NavigationService ns)
        {
            if (confirmMultiuserPage != null)
                ns.Navigate(confirmMultiuserPage);
            else
                ns.Navigate(new ConfirmMultiUserPage());
        }

        public void navigateToConfirmMultiuserPage(Frame frame)
        {
            if (confirmMultiuserPage != null)
                frame.Navigate(confirmMultiuserPage);
            else
                frame.Navigate(new ConfirmMultiUserPage());
        }

        public void navigateToConfirmExchangePage(NavigationService ns)
        {
            if (confirmExchangePage != null)
                ns.Navigate(confirmExchangePage);
            else
                ns.Navigate(new ConfirmExchangePage());
        }

        public void navigateToConfirmExchangePage(Frame frame)
        {
            if (confirmExchangePage != null)
                frame.Navigate(confirmExchangePage);
            else
                frame.Navigate(new ConfirmExchangePage());
        }

        public void navigateToConfirmUserExitPage(NavigationService ns)
        {
            if (confirmUserExitPage != null)
                ns.Navigate(confirmUserExitPage);
            else
                ns.Navigate(new ConfirmUserExitPage());
        }

        public void navigateToConfirmUserExitPage(Frame frame)
        {
            if (confirmUserExitPage != null)
                frame.Navigate(confirmUserExitPage);
            else
                frame.Navigate(new ConfirmUserExitPage());
        }

        public void navigateToIdentificationPage(NavigationService ns)
        {
            if (identificationPage != null)
                ns.Navigate(identificationPage);
            else
                ns.Navigate(new IdentificationPage());
        }

        public void navigateToIdentificationPage(Frame ns)
        {
            if (identificationPage != null)
                ns.Navigate(identificationPage);
            else
                ns.Navigate(new IdentificationPage());
        }

        public void navigateToSplashScreen(NavigationService ns)
        {
            if (splashScreen != null)
                ns.Navigate(splashScreen);
            else
                ns.Navigate(new SplashSreen());
        }

        public int CALENDAR_DAYS
        {
            get
            {
                return _CALENDAR_DAYS;
            }
        }

        public int CALENDAR_HOURS
        {
            get
            {
                return _CALENDAR_HOURS;
            }
        }

        public int LIMIT_USERS
        {
            get
            {
                return _LIMIT_USERS;
            }
        }

        public double IMPLICIT_ZONE
        {
            get
            {
                return _IMPLICIT_ZONE;
            }
        }

        public void UpdateUsers()
        {
            bool remove = true;
            for (int i = 0; i < users.Count; i++)
            {
                foreach (ulong id in trackedBodies)
                {
                    if (users[i].BodyId.Equals(id))
                        remove = false;
                }

                if (remove)
                {
                    if (users.Count > 1)
                        userTwoLoggedOut = true;
                    users.Remove(users[i]);
                }                    
                remove = true;
            }
        }

        public void UpdateUnidentified()
        {
            //List<ulong> trackedBodies = trackedBodies;
            //List<User> users = users;

            bool add = true;
            unidentifiedBodies.Clear();
            foreach (ulong id in trackedBodies)
            {
                foreach (User user in users)
                {
                    if (id.Equals(user.BodyId))
                        add = false;
                }

                if (add)
                    unidentifiedBodies.Add(id);
                else
                    add = true;
            }
        }

        public User getUser(ulong bodyId)
        {
            User res = null;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].BodyId.Equals(bodyId))
                {
                    res = users[i];
                    break;
                }
            }
            return res;
        }

        private void rectNotExchangeableHours_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("No Exchange !");
        }
    }
}
