using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Utilities
{
    internal class GameTasks
    {
        internal static async Task TestTask()
        {
            Console.WriteLine("Entering Test Task");
            await Task.Delay(900);
            Console.WriteLine("Finished Test Task");
        }

        internal static async Task CalculateRouteExecutions()
        {
            // Get the current fuel price and day of the week
            double currentFuelPrice = GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount;
            DayOfWeek currentDay = GameState.Instance.CurrentDate.DayOfWeek;

            // Loop through all routes in the game
            foreach (Route route in GameData.Instance.Routes)
            {
                // Check if there are any assigned airliners and schedules flights
                if(route.ScheduledFlights.Count <= 0 || route.AssignedAirliners.Count <= 0)
                {
                    // No scheduled flights or assigned airliners, so no need to calculate
                    continue;
                }

                // Create a dictionary to hold demand for each day of the week
                // Fixed values; awaiting implementation of demands
                Dictionary<DayOfWeek, int> dailyDemand = new Dictionary<DayOfWeek, int>()
                {
                    { DayOfWeek.Sunday, 100 },
                    { DayOfWeek.Monday, 150 },
                    { DayOfWeek.Tuesday, 120 },
                    { DayOfWeek.Wednesday, 130 },
                    { DayOfWeek.Thursday, 140 },
                    { DayOfWeek.Friday, 160 },
                    { DayOfWeek.Saturday, 180 }
                };
                Price totalDailyRevenue = new Price(0);
                Price totalDailyCost = new Price(0);

                // Loop through all the schedules 
                foreach(RouteSchedule schedule in route.ScheduledFlights)
                {
                    // Local function to calculate the flight
                    void calculateFlight(RouteLeg leg)
                    {
                        if(dailyDemand[currentDay] > 0)
                        {
                            // Check how many seats are available, and in demand.
                            int legSeatAvailability = leg.AssignedAirliner.Model.maxPax;
                            int legPassengers = Math.Min(dailyDemand[currentDay], legSeatAvailability);

                            // Lower daily demand, based on availability 
                            dailyDemand[currentDay] -= legPassengers;

                            // Add ticket sales to daily revenue
                            totalDailyRevenue += legPassengers * route.TicketPrice;
                        }

                        // Add airliner costs to daily costs
                        // Fixed value; awaiting implementation of costs
                        totalDailyCost += 1000 + (route.Distance * leg.AssignedAirliner.Model.FuelConsumptionRate * currentFuelPrice);
                    }

                    // Check if outbound flight is today
                    if (schedule.Outbound.ArrivalDay == currentDay)
                        calculateFlight(schedule.Outbound);

                    // Check if inbound flight is today
                    if (schedule.Inbound.ArrivalDay == currentDay)
                        calculateFlight(schedule.Inbound);
                }

                // Calculate daily profit
                Price totalDailyProfit = totalDailyRevenue - totalDailyCost;

                // Set new balance of route owner
                route.Owner.addCash(totalDailyProfit);
            }
        }

        internal static async Task CalculateFuelPrice()
        {
            // Cache year and price map to avoid repeated lookups
            int year = GameState.Instance.CurrentDate.Year;
            var fuelPriceMap = GameData.Instance.FuelPriceMap;

            // Cache current price
            double currentFuelPrice = GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount;

            // Retrieve relevant price boundaries
            double currentYearPrice = fuelPriceMap.TryGetValue(year, out var cyp) ? cyp : 0.0;
            double lastYearPrice = fuelPriceMap.TryGetValue(year - 1, out var lyp) ? lyp : currentYearPrice;
            double nextYearPrice = fuelPriceMap.TryGetValue(year + 1, out var nyp) ? nyp : currentYearPrice;

            // Precompute min and max boundaries
            double min = Math.Min(lastYearPrice, nextYearPrice);
            double max = Math.Max(lastYearPrice, nextYearPrice);

            // Add overshoot if difference is small
            double difference = max - min;
            if (difference < 0.1)
            {
                min = Math.Max(0, min - 0.05); // Ensure min >= 0
                max += 0.05;
            }

            // Apply a smoothing factor
            const double smoothingFactor = 0.15; // Adjust to control changes

            // Use a static Random instance to reduce overhead
            double randomPrice = min + (SharedRandom.Instance.NextDouble() * (max - min)); // Generate random value
            double newFuelPrice = currentFuelPrice + (randomPrice - currentFuelPrice) * smoothingFactor;

            // Round only once before updating
            GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount = Math.Round(newFuelPrice, 4);
        }

        // Shared static Random instance
        private static class SharedRandom
        {
            public static readonly Random Instance = new Random();
        }
    }
}
