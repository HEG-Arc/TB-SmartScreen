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
            users.Add(new User(0, "Jeff Sokoli", "USER-JEFF-SOKOLI-732195", generateJeffsCalendar(), new Statistics(64, 1456, 22, 3565), null));
            users.Add(new User(0, "Marc Abraham", "USER-MARC-ABRAHAM-789554", generateMarcsCalendar(), new Statistics(75, 2078, 22, 3894), null));
            return users;
        }

        private Calendar generateJeffsCalendar()
        {
            // Lundi de 7:00 à 12:00 et de 13:00 à 17:00
            // Mardi de 7:00 à 12:00
            // Jeudi de 7:00 à 12:00
            // Vendredi de 13:00 à 17:00
            List<TimeSlot> tsMonday = new List<TimeSlot>() { new TimeSlot(false, 1), new TimeSlot(true, 4), new TimeSlot(false, 1), new TimeSlot(true, 4) };
            Day monday = new Day(tsMonday);

            List<TimeSlot> tsTuesday = new List<TimeSlot>() { new TimeSlot(true, 5), new TimeSlot(false, 5) };
            Day tuesday = new Day(tsTuesday);

            List<TimeSlot> tsWednesday = new List<TimeSlot>() { new TimeSlot(false, 10) };
            Day wednesday = new Day(tsWednesday);

            List<TimeSlot> tsThursday = new List<TimeSlot>() { new TimeSlot(true, 5), new TimeSlot(false, 5) };
            Day thursday = new Day(tsThursday);

            List<TimeSlot> tsFriday = new List<TimeSlot>() { new TimeSlot(false, 6), new TimeSlot(true, 4) };
            Day friday = new Day(tsFriday);

            List<Day> days = new List<Day>();
            days.Add(monday);
            days.Add(tuesday);
            days.Add(wednesday);
            days.Add(thursday);
            days.Add(friday);

            return new Calendar(days);
        }

        private Calendar generateMarcsCalendar()
        {
            // Lundi de 7:00 à 12:00 et de 13:00 à 17:00
            // Mardi de 13:00 à 16:00
            // Mercredi de 7:00 à 12:00 et de 13:00 à 17:00
            // Jeudi de 7:00 à 12:00
            // Vendredi de 13:00 à 17:00
            List<TimeSlot> tsMonday = new List<TimeSlot>() { new TimeSlot(true, 5), new TimeSlot(false, 1), new TimeSlot(true, 4) };
            Day monday = new Day(tsMonday);

            List<TimeSlot> tsTuesday = new List<TimeSlot>() { new TimeSlot(false, 6), new TimeSlot(true, 4) };
            Day tuesday = new Day(tsTuesday);

            List<TimeSlot> tsWednesday = new List<TimeSlot>() { new TimeSlot(true, 5), new TimeSlot(false, 1), new TimeSlot(true, 4) };
            Day wednesday = new Day(tsWednesday);

            List<TimeSlot> tsThursday = new List<TimeSlot>() { new TimeSlot(true, 5), new TimeSlot(false, 5) };
            Day thursday = new Day(tsThursday);

            List<TimeSlot> tsFriday = new List<TimeSlot>() { new TimeSlot(false, 10) };
            Day friday = new Day(tsFriday);

            List<Day> days = new List<Day>();
            days.Add(monday);
            days.Add(tuesday);
            days.Add(wednesday);
            days.Add(thursday);
            days.Add(friday);

            return new Calendar(days);
        }
    }
}
