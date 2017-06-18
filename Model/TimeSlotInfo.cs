using System.Drawing;

namespace SCE_ProductionChain.Model
{
    public class TimeSlotInfo
    {
        private User worker;
        private User exchangeTo;
        private int dayIndex;
        private int from;
        private int to;

        public TimeSlotInfo()
        {
            this.Worker = null;
            this.ExchangeTo = null;
            this.DayIndex = 0;
            this.From = 0;
            this.To = 0;
        }

        public TimeSlotInfo(User worker, User exchangeTo, int dayIndex, int from, int to)
        {
            this.Worker = worker;
            this.ExchangeTo = exchangeTo;
            this.DayIndex = dayIndex;
            this.From = from;
            this.To = to;
        }

        public User Worker
        {
            get
            {
                return worker;
            }

            set
            {
                worker = value;
            }
        }

        public User ExchangeTo
        {
            get
            {
                return exchangeTo;
            }

            set
            {
                exchangeTo = value;
            }
        }

        public int DayIndex
        {
            get
            {
                return dayIndex;
            }

            set
            {
                dayIndex = value;
            }
        }

        public int From
        {
            get
            {
                return from;
            }

            set
            {
                from = value;
            }
        }

        public int To
        {
            get
            {
                return to;
            }

            set
            {
                to = value;
            }
        }

        public override string ToString()
        {
            return "Worker : " + this.worker.Username + "\n" +
                   "ExchangeTo : " + this.exchangeTo.Username + "\n" +
                   "DayIndey : " + this.dayIndex + "\n" +
                   "From : " + this.from + "\n" +
                   "To : " + this.to;
        }
    }
}
