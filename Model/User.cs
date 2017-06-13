﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCE_ProductionChain.Model
{
    public class User
    {
        private ulong bodyId;
        private string username;
        private string code;

        public User(ulong bodyId, string username, string code)
        {
            this.bodyId = bodyId;
            this.Username = username;
            this.Code = code;
        }

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
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