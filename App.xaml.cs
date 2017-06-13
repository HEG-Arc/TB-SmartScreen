using SCE_ProductionChain.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SCE_ProductionChain
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {       
        internal Page identificationPage;

        private List<KeyValuePair<string, string>> availableUsers = new Dictionary<String, String>
        {
            {"USER-JEFF-SOKOLI-732195", "Jeff Sokoli"},
            {"USER-MARC-ABRAHAM-789554", "Marc Abraham"},
        }.ToList();

        internal List<User> users { get; set; }
        internal List<ulong> trackedBodies { get; set; }
        internal List<ulong> unidentifiedBodies { get; set; }

        public App()
        {
            this.users = new List<User>();
        }
        public List<KeyValuePair<string, string>> AvailableUsers
        {
            get
            { return this.availableUsers; }
            set
            { this.availableUsers = value; }
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
