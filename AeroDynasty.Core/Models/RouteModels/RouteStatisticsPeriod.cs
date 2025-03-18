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

        // Financial properties
        public Price Balance
        {
            get
            {
                return TicketIncome - (AirlinerCosts + FuelCosts);
            }
        }
        public Price TicketIncome { get; set; }
        public Price AirlinerCosts { get; set; }
        public Price FuelCosts { get; set; }

        // Passenger properties
        public int Passengers { get; set; }
        public int AvailableSeats { get; set; }
        public double FillingPercentage
        {
            get
            {
                return ((double)Passengers / (double)AvailableSeats) * 100;
            }
        }

        public RouteStatisticsPeriod(DateTime date)
        {
            Date = new DateTime(date.Year, date.Month, 1); // Always make sure the date is set at the first of the month
            TicketIncome = new Price(0);
            AirlinerCosts = new Price(0);
            FuelCosts = new Price(0);
        }
    }
}
