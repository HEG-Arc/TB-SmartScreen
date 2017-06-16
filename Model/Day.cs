using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCE_ProductionChain.Model
{
    public class Day
    {
        List<TimeSlot> timeSlots;

        public Day(List<TimeSlot> timeSlots)
        {
            this.TimeSlots = timeSlots;
        }

        public List<TimeSlot> TimeSlots
        {
            get
            {
                return timeSlots;
            }

            set
            {
                timeSlots = value;
            }
        }
    }
}
