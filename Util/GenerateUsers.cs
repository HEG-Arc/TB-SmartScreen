using SCE_ProductionChain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCE_ProductionChain.Util
{
    public class GenerateUsers
    {
        private App app;

        public GenerateUsers()
        {
            app = (App)Application.Current;
        }

        public List<User> getUsers()
        {
            List<User> users = new List<User>();
            users.Add(new User(0, "Jeff Sokoli", "USER-JEFF-SOKOLI-732195", generateJeffsCalendar(), new Statistics(64, 1456, 22, 3565)));
            users.Add(new User(0, "Marc Abraham", "USER-MARC-ABRAHAM-789554", generateMarcsCalendar(), new Statistics(75, 2078, 22, 3894)));
            return users;
        }

        private bool[,] generateJeffsCalendar()
        {
            bool[,] calendar = new bool[app.CALENDAR_DAYS, app.CALENDAR_HOURS];
            initCalendar(calendar);

            // Lundi de 7:00 à 12:00 et de 13:00 à 17:00
            // Mardi de 7:00 à 12:00
            // Jeudi de 7:00 à 12:00
            // Vendredi de 13:00 à 17:00
            for (int h = 7; h < 12; h++)
            {
                calendar[0, h - 7] = true;
                calendar[1, h - 7] = true;
                calendar[3, h - 7] = true;
            }
            for (int h = 13; h < 17; h++)
            {
                calendar[0, h - 7] = true;
                calendar[4, h - 7] = true;
            }

            return calendar;
        }

        private bool[,] generateMarcsCalendar()
        {
            bool[,] calendar = new bool[app.CALENDAR_DAYS, app.CALENDAR_HOURS];
            initCalendar(calendar);

            // Lundi de 7:00 à 12:00 et de 13:00 à 17:00
            // Mardi de 13:00 à 16:00
            // Mercredi de 7:00 à 12:00 et de 13:00 à 17:00
            // Jeudi de 7:00 à 12:00
            // Vendredi de 13:00 à 17:00
            for (int h = 7; h < 12; h++)
            {
                calendar[0, h - 7] = true;                
                calendar[2, h - 7] = true;
                calendar[3, h - 7] = true;
            }
            for (int h = 13; h < 17; h++)
            {
                calendar[0, h - 7] = true;
                calendar[1, h - 7] = true;
                calendar[2, h - 7] = true;
                calendar[4, h - 7] = true;
            }

            return calendar;
        }

        private void initCalendar(bool[,] calendar)
        {
            for (int d = 0; d < app.CALENDAR_DAYS; d++)
            {
                for (int h = 0; h < app.CALENDAR_HOURS; h++)
                {
                    calendar[d, h] = false;
                }
            }
        }
    }
}
