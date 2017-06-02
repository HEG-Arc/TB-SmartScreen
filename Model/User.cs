using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_MultiUserIndification_Collider.Model
{
    public class User
    {
        private ulong bodyId;
        private string username;

        public User(ulong bodyId, string username)
        {
            this.bodyId = bodyId;
            this.Username = username;
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public ulong BodyId
        {
            get
            {
                return bodyId;
            }

            set
            {
                bodyId = value;
            }
        }
    }
}
