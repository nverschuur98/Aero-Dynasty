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
            DayOfWeek currentDay = GameState.Instance.CurrentDate.DayOfWeek;
            Console.WriteLine("Start calculating the route executions");
            Console.WriteLine($"Current day of the week: {currentDay}");

            // Loop through all routes in the game
            foreach(Route route in GameData.Instance.Routes)
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
                double totalDailyRevenue = 0;
                double totalDailyCost = 0;

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
                            totalDailyRevenue += legPassengers * route.TicketPrice.Amount;
                        }

                        // Add airliner costs to daily costs
                        // Fixed value; awaiting implementation of costs
                        totalDailyCost += 1000;
                    }

                    // Check if outbound flight is today
                    if (schedule.Outbound.ArrivalDay == currentDay)
                        calculateFlight(schedule.Outbound);

                    // Check if inbound flight is today
                    if (schedule.Inbound.ArrivalDay == currentDay)
                        calculateFlight(schedule.Inbound);
                }

                // Calculate daily profit
                double totalDailyProfit = totalDailyRevenue - totalDailyCost;

                // Set new balance of route owner
                route.Owner.addCash(totalDailyProfit);
                Console.WriteLine($"Added a total of {totalDailyProfit} to {route.Owner.Name} for route {route.Name}");
            }
        }
    }
}
