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
        private int gridRow;
        private int gridColumn;
        private int gridRowspan;
        private int gridColspan;

        public TimeSlotInfo()
        {
            this.Worker = null;
            this.ExchangeTo = null;
            this.DayIndex = 0;
            this.From = 0;
            this.To = 0;
            this.GridRow = 0;
            this.GridColumn = 0;
            this.gridRowspan = 0;
            this.GridColspan = 0;
        }

        public TimeSlotInfo(User worker, User exchangeTo, int dayIndex, int from, int to)
        {
            this.Worker = worker;
            this.ExchangeTo = exchangeTo;
            this.DayIndex = dayIndex;
            this.From = from;
            this.To = to;
            this.GridRow = 0;
            this.GridColumn = 0;
            this.gridRowspan = 0;
            this.GridColspan = 0;
        }

        public TimeSlotInfo(User worker, User exchangeTo, int dayIndex, int from, int to, int gridRow, int gridColumn, int gridRowspan, int gridColspan)
        {
            this.worker = worker;
            this.exchangeTo = exchangeTo;
            this.dayIndex = dayIndex;
            this.from = from;
            this.to = to;
            this.GridRow = gridRow;
            this.GridColumn = gridColumn;
            this.GridRowspan = gridRowspan;
            this.GridColspan = gridColspan;
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

        public int GridRow
        {
            get
            {
                return gridRow;
            }

            set
            {
                gridRow = value;
            }
        }

        public int GridColumn
        {
            get
            {
                return gridColumn;
            }

            set
            {
                gridColumn = value;
            }
        }

        public int GridRowspan
        {
            get
            {
                return gridRowspan;
            }

            set
            {
                gridRowspan = value;
            }
        }

        public int GridColspan
        {
            get
            {
                return gridColspan;
            }

            set
            {
                gridColspan = value;
            }
        }

        public override string ToString()
        {
            return "Worker : " + this.worker.Username + "\n" +
                   "ExchangeTo : " + this.exchangeTo.Username + "\n" +
                   "DayIndey : " + this.dayIndex + "\n" +
                   "From : " + this.from + "\n" +
                   "To : " + this.to + "\n" +
                   "GridRow : " + this.gridRow + "\n" +
                   "GridColumn : " + this.gridColumn;
        }
    }
}
