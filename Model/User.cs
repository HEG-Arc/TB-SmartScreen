using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SCE_ProductionChain.Model
{
    public class User
    {
        private ulong bodyId;
        private string username;
        private string code;
        private bool[,] calendar;
        private Statistics statistics;
        private SolidColorBrush color;

        public User(ulong bodyId, string username, string code, bool[,] calendar, Statistics stats, SolidColorBrush color)
        {
            this.bodyId = bodyId;
            this.Username = username;
            this.Code = code;
            this.calendar = calendar;
            this.Statistics = stats;
            this.Color = color;
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

        public bool[,] Calendar
        {
            get
            {
                return calendar;
            }

            set
            {
                calendar = value;
            }
        }

        public Statistics Statistics
        {
            get
            {
                return statistics;
            }

            set
            {
                statistics = value;
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }
    }
}
