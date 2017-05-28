using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_MultiUserIdentification.Model
{
    public class User
    {
        private uint color;
        private string name;

        public User(string name, uint color)
        {
            this.color = color;
            this.name = name;
        }

        public uint Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

        public string Code
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
