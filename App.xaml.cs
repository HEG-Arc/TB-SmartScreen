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

namespace SCE_ProductionChain
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int _CALENDAR_DAYS = 5;

        private const int _CALENDAR_HOURS = 10;

        internal Page identificationPage;

        internal Page calendarPage;

        internal Page statisticsPage;

        internal Page confirmMultiuserPage;
        internal List<User> users { get; set; }
        internal List<ulong> trackedBodies { get; set; }
        internal List<ulong> unidentifiedBodies { get; set; }
        internal MultiSourceFrameReader msfr { get; set; }    
        internal bool onIdentificationPage { get; set; }

        internal SolidColorBrush primaryBrush { get; set; }
        internal SolidColorBrush secondaryBrush { get; set; }
        internal SolidColorBrush backgroundBrush { get; set; }
        /*
        private List<KeyValuePair<string, string>> availableUsers = new Dictionary<String, String>
        {
            {"USER-JEFF-SOKOLI-732195", "Jeff Sokoli"},
            {"USER-MARC-ABRAHAM-789554", "Marc Abraham"},
        }.ToList();
        */

        internal List<User> availableUsers { get; set; }

        public App()
        {
            this.users = new List<User>();
            this.trackedBodies = new List<ulong>();
            this.unidentifiedBodies = new List<ulong>();
            this.onIdentificationPage = false;
            this.availableUsers = new GenerateUsers().getUsers();

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

        public void navigateToConfirmMultiuserPage(Frame frame)
        {
            if (confirmMultiuserPage != null)
                frame.Navigate(confirmMultiuserPage);
            else
                frame.Navigate(new ConfirmMultiUserPage());
        }

        public void navigateToIdentificationPage(NavigationService ns)
        {
            if (identificationPage != null)
                ns.Navigate(identificationPage);
            else
                ns.Navigate(new IdentificationPage());
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
                    users.Remove(users[i]);
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
    }
}
