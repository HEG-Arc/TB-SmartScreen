using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCE_ProductionChain.Model
{
    public class Statistics
    {
        private int nbHoursWorked;
        private int nbPiecesWorked;
        private double revenuePerHour;
        private double salaryEstimation;

        public Statistics(int nbHoursWorked, int nbPiecesWorked, double revenuePerHour, double salaryEstimation)
        {
            this.NbHoursWorked = nbHoursWorked;
            this.NbPiecesWorked = nbPiecesWorked;
            this.RevenuePerHour = revenuePerHour;
            this.SalaryEstimation = salaryEstimation;
        }

        public int NbHoursWorked
        {
            get
            {
                return nbHoursWorked;
            }

            set
            {
                nbHoursWorked = value;
            }
        }

        public int NbPiecesWorked
        {
            get
            {
                return nbPiecesWorked;
            }

            set
            {
                nbPiecesWorked = value;
            }
        }

        public double RevenuePerHour
        {
            get
            {
                return revenuePerHour;
            }

            set
            {
                revenuePerHour = value;
            }
        }

        public double SalaryEstimation
        {
            get
            {
                return salaryEstimation;
            }

            set
            {
                salaryEstimation = value;
            }
        }
    }
}
