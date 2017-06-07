using POC_MultiUserIndification_Collider;
using POC_MultiUserIndification_Collider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POC_MultiUserIdification_Collider.Util
{
    public class Users
    {
        public static void UpdateUsers()
        {
            bool remove = true;
            for (int i = 0; i < ((App)Application.Current).users.Count; i++)
            {
                foreach (ulong id in ((App)Application.Current).trackedBodies)
                {
                    if (((App)Application.Current).users[i].BodyId.Equals(id))
                        remove = false;
                }

                if (remove)
                    ((App)Application.Current).users.Remove(((App)Application.Current).users[i]);
                remove = true;
            }
        }

        public static void UpdateUnidentified()
        {
            List<ulong> trackedBodies = ((App)Application.Current).trackedBodies;
            List<User> users = ((App)Application.Current).users;
            
            bool add = true;
            ((App)Application.Current).unidentifiedBodies.Clear();
            foreach (ulong id in trackedBodies)
            {
                foreach(User user in users)
                {
                    if (id.Equals(user.BodyId))
                        add = false;
                }

                if (add)
                    ((App)Application.Current).unidentifiedBodies.Add(id);
                else
                    add = true;
            }
        }

        public static User getUser(ulong bodyId)
        {
            User res = null;
            for (int i = 0; i < ((App)Application.Current).users.Count; i++)
            {
                if (((App)Application.Current).users[i].BodyId.Equals(bodyId))
                {
                    res = ((App)Application.Current).users[i];
                    break;
                }
            }
            return res;
        }
    }
}
