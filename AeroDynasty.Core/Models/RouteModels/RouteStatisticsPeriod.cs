using AeroDynasty.Core.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteStatisticsPeriod
    {
        public DateTime Date { get; set; }
        public Price Balance { get; set; }
        public double Passengers { get; set; }
        public double FillingPercentage { get; set; }

        public RouteStatisticsPeriod(DateTime date)
        {
            Date = new DateTime(date.Year, date.Month, 1); // Always make sure the date is set at the first of the month
            Balance = new Price(0);
        }
    }
}
