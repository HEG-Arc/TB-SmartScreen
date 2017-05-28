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
        private string code;

        public User(string code, uint color)
        {
            this.color = color;
            this.code = code;
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
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }
    }
}
