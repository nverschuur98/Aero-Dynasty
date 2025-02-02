using AeroDynasty.Core.Models.AirportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteDemand
    {
        public Airport Origin { get; private set; }
        public Airport Destination { get; private set; }
        public int BaseDemand { get; set; }
        public Dictionary<DayOfWeek, int> DailyDemand { get; private set; }

        public RouteDemand(Airport origin, Airport destination, int demand)
        {
            Origin = origin;
            Destination = destination;
            BaseDemand = demand;
            DailyDemand = new Dictionary<DayOfWeek, int>() {
                { DayOfWeek.Sunday, 0 },
                { DayOfWeek.Monday, 0 },
                { DayOfWeek.Tuesday, 0 },
                { DayOfWeek.Wednesday, 0 },
                { DayOfWeek.Thursday, 0 },
                { DayOfWeek.Friday, 0 },
                { DayOfWeek.Saturday, 0 }
            };
        }

        public void CalculateDailyDemand()
        {
            // Reset daily demand
            foreach (var key in DailyDemand.Keys.ToList())
            {
                DailyDemand[key] = 0;
            }

            // The origin airport is large
            if (Origin.PassengerSize == Enums.AirportSize.Largest ||
                Origin.PassengerSize == Enums.AirportSize.VeryLarge ||
                Origin.PassengerSize == Enums.AirportSize.Large)
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        // High demand, slight variation across the week
                        DistributeDemand(BaseDemand, 1.1, 1.05, 1.0, 1.0, 0.95, 0.9, 1.15);
                        break;

                    case Enums.AirportSize.Large:
                        // Strong business demand, peaking mid-week
                        DistributeDemand(BaseDemand, 1.0, 1.1, 1.2, 1.2, 1.1, 0.8, 0.7);
                        break;

                    case Enums.AirportSize.Medium:
                        // Mixed demand, slightly higher mid-week
                        DistributeDemand(BaseDemand, 0.95, 1.0, 1.1, 1.1, 1.0, 0.9, 0.85);
                        break;

                    case Enums.AirportSize.Small:
                    case Enums.AirportSize.VerySmall:
                        // Mostly leisure, peaking weekends
                        DistributeDemand(BaseDemand, 1.2, 0.9, 0.85, 0.85, 0.9, 1.1, 1.3);
                        break;
                }
            }
            // The origin airport is medium
            else if (Origin.PassengerSize == Enums.AirportSize.Medium ||
                     Origin.PassengerSize == Enums.AirportSize.Small)
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        // Business-heavy, stronger mid-week demand
                        DistributeDemand(BaseDemand, 0.9, 1.1, 1.3, 1.3, 1.1, 0.7, 0.6);
                        break;

                    case Enums.AirportSize.Large:
                        // Moderate demand, higher during the week
                        DistributeDemand(BaseDemand, 1.0, 1.05, 1.15, 1.15, 1.05, 0.85, 0.75);
                        break;

                    case Enums.AirportSize.Medium:
                        // Evenly spread but slightly higher on weekends
                        DistributeDemand(BaseDemand, 1.1, 0.9, 0.9, 0.9, 0.9, 1.2, 1.3);
                        break;

                    case Enums.AirportSize.Small:
                    case Enums.AirportSize.VerySmall:
                        // Light demand, peaking weekends
                        DistributeDemand(BaseDemand, 1.3, 0.8, 0.8, 0.8, 0.8, 1.2, 1.4);
                        break;
                }
            }
            // The origin airport is small
            else
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        // Mostly leisure, peaks on weekends
                        DistributeDemand(BaseDemand, 1.3, 0.8, 0.85, 0.85, 0.8, 1.2, 1.5);
                        break;

                    case Enums.AirportSize.Large:
                        // Similar to above, but slightly lower overall demand
                        DistributeDemand(BaseDemand, 1.25, 0.85, 0.85, 0.85, 0.85, 1.15, 1.4);
                        break;

                    case Enums.AirportSize.Medium:
                        // Light demand, mostly leisure
                        DistributeDemand(BaseDemand, 1.2, 0.9, 0.9, 0.9, 0.9, 1.1, 1.3);
                        break;

                    case Enums.AirportSize.Small:
                    case Enums.AirportSize.VerySmall:
                        // Very low demand, weekend trips
                        DistributeDemand(BaseDemand, 1.4, 0.7, 0.7, 0.7, 0.7, 1.3, 1.5);
                        break;
                }
            }
        }

        // Helper method to distribute demand
        private void DistributeDemand(int baseDemand, double sun, double mon, double tue, double wed, double thu, double fri, double sat)
        {
            DailyDemand[DayOfWeek.Sunday] = (int)(baseDemand * sun);
            DailyDemand[DayOfWeek.Monday] = (int)(baseDemand * mon);
            DailyDemand[DayOfWeek.Tuesday] = (int)(baseDemand * tue);
            DailyDemand[DayOfWeek.Wednesday] = (int)(baseDemand * wed);
            DailyDemand[DayOfWeek.Thursday] = (int)(baseDemand * thu);
            DailyDemand[DayOfWeek.Friday] = (int)(baseDemand * fri);
            DailyDemand[DayOfWeek.Saturday] = (int)(baseDemand * sat);
        }
    }
}
