using System.Collections.Generic;
using System.Windows.Media;

namespace SCE_ProductionChain.Model
{
    public class User
    {
        private ulong bodyId;
        private string username;
        private string code;
        private Calendar calendar;
        private Statistics statistics;
        private SolidColorBrush color;

        public User(ulong bodyId, string username, string code, Calendar calendar, Statistics stats, SolidColorBrush color)
        {
            this.bodyId = bodyId;
            this.Username = username;
            this.Code = code;
            this.calendar = calendar;
            this.Statistics = stats;
            this.Color = color;
        }

        public void ReplaceHours(TimeSlotInfo tsi)//, bool replaceToWorker)
        {
            Day newDay = new Day(new List<TimeSlot>());
            User user = null;

            //if (replaceToWorker)
            //    user = tsi.Worker;
            //else
            //    user = tsi.ExchangeTo;
            user = this;

            int duration = 0;
            foreach (TimeSlot ts in user.Calendar.Days[tsi.DayIndex].TimeSlots)
            {
                duration += ts.Duration;
                int newTimeSlotDuration = 0;
                if (duration >= tsi.From)
                {
                    int currentDuration = duration - ts.Duration + 1;
                    while (currentDuration < tsi.From)
                    {
                        newTimeSlotDuration++;
                        currentDuration++;
                    }
                    if (newTimeSlotDuration > 0)
                        newDay.TimeSlots.Add(new TimeSlot(ts.IsWorking, newTimeSlotDuration));

                    newTimeSlotDuration = 0;
                    while (currentDuration >= tsi.From && currentDuration <= tsi.To)
                    {
                        newTimeSlotDuration++;
                        currentDuration++;
                    }
                    if (newTimeSlotDuration > 0)
                        newDay.TimeSlots.Add(new TimeSlot(!ts.IsWorking, newTimeSlotDuration));

                    newTimeSlotDuration = 0;
                    while (currentDuration > tsi.To && currentDuration <= duration)
                    {
                        newTimeSlotDuration++;
                        currentDuration++;
                    }
                    if (newTimeSlotDuration > 0)
                        newDay.TimeSlots.Add(new TimeSlot(ts.IsWorking, newTimeSlotDuration));
                }
                else
                {
                    newDay.TimeSlots.Add(ts);
                }
            }
            //app.users[app.users.IndexOf(tsi.Worker)].Calendar.Days[tsi.DayIndex] = newDay;
            user.Calendar.Days[tsi.DayIndex] = newDay;
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

        public Calendar Calendar
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
