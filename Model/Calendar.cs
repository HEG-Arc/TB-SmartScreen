﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCE_ProductionChain.Model
{
    public class Calendar
    {
        List<Day> days;

        public Calendar(List<Day> days)
        {
            this.Days = days;
        }

        public List<Day> Days
        {
            get
            {
                return days;
            }

            set
            {
                days = value;
            }
        }
    }
}
