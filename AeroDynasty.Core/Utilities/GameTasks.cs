using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        /// <summary>
        /// Task to calculate all executed/scheduled routes in the game
        /// </summary>
        /// <returns></returns>
        internal static async Task CalculateRouteExecutions()
        {
#if DEBUG
            // Start the stopwatch to time the tasks
            var stopwatch = Stopwatch.StartNew();
#endif

            // Get the current fuel price and current day
            double currentFuelPrice = GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount;
            DayOfWeek currentDay = GameState.Instance.CurrentDate.DayOfWeek;
            List<RouteDemand> routeDemands = GameData.Instance.RouteDemands;

            // Check if routeDemands is not empty or null
            if (routeDemands.Count <= 0 || routeDemands == null)
                return;

            // Collect all RouteLegs that have flights today, with their parent Route
            var allRouteLegs = GameData.Instance.Routes
                .SelectMany(route => route.ScheduledFlights, (route, schedule) => new { Route = route, Schedule = schedule })
                .SelectMany(rs => new List<(Route Route, RouteLeg Leg)>
                {
            (rs.Route, rs.Schedule.Outbound),
            (rs.Route, rs.Schedule.Inbound)
                })
                .Where(rl => rl.Leg.ArrivalDay == currentDay)
                .ToList();

            // Group all RouteLegs by (Origin, Destination)
            var groupedLegs = allRouteLegs.GroupBy(rl => (rl.Leg.Origin, rl.Leg.Destination));

            // Process each group concurrently using Task.WhenAll()
            await Task.WhenAll(groupedLegs.Select(group => Task.Run(() =>
            {
                int localDemand = 0;

                // Get the corresponding RouteDemand object, or null if not found
                RouteDemand routeDemand = routeDemands
                    .FirstOrDefault(rd => rd.Origin == group.Key.Origin && rd.Destination == group.Key.Destination);

                // If routeDemand is found, get the demand for the current day; otherwise, localDemand remains 0
                if (routeDemand != null)
                    localDemand = routeDemand.DailyDemand[currentDay]; // Local copy of available demand

#if DEBUG
                Console.WriteLine($"{currentDay} - {group.Key.Origin.ICAO}-{group.Key.Destination.ICAO} - Total Demand: {localDemand}");
#endif

                // Sort RouteLegs by some priority (e.g., largest airline reputation first)
                var sortedLegs = group.OrderByDescending(r => r.Route.Owner.Reputation);

                foreach (var (route, leg) in sortedLegs)
                {
                    // Determine seat availability
                    int legSeatAvailability = leg.AssignedAirliner.Model.maxPax;
                    // Determine how many passengers can be filled on this flight
                    int filledPassengers = Math.Min(localDemand, legSeatAvailability);
                    // Subtract from the local available demand
                    localDemand -= filledPassengers;

                    // Calculate revenue and cost
                    Price revenue = filledPassengers * route.TicketPrice;
                    Price cost = new Price(1000 + (route.Distance * leg.AssignedAirliner.Model.FuelConsumptionRate * currentFuelPrice));

                    // Update airline balance
                    route.Owner.addCash(revenue - cost);
#if DEBUG
                    Console.WriteLine($"{currentDay} - {leg.Origin.ICAO}-{leg.Destination.ICAO} - Departure:{leg.DepartureTime}, Pax:{filledPassengers}, Profit: {revenue - cost}, Demand left: {localDemand}");
#endif
                }
            })));

#if DEBUG
            // Stop the stopwatch after all tasks are completed
            stopwatch.Stop();

            // Log or display the time it took to complete the tasks
            Console.WriteLine($"CalculateRouteExecutions completed in {stopwatch.ElapsedMilliseconds} ms");
#endif
        }

        /// <summary>
        /// Task to calculate the daily fuel price
        /// </summary>
        /// <returns></returns>
        internal static async Task CalculateFuelPrice()
        {
            // Cache year and price map to avoid repeated lookups
            int year = GameState.Instance.CurrentDate.Year;
            var fuelPriceMap = GameData.Instance.GlobalModifiers.FuelPriceMap;

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

        /// <summary>
        /// Task to check on newly available aircraftmodels
        /// </summary>
        /// <returns></returns>
        internal static async Task CheckIsActive()
        {
            //Get the current Date
            DateTime currentDate = GameState.Instance.CurrentDate;

            await GameData.Instance.Airports.CheckIsActiveForAllAsync(currentDate);
            await GameData.Instance.AircraftModels.CheckIsActiveForAllAsync(currentDate);
        }

        /// <summary>
        /// Task to calculate the route demand per month
        /// </summary>
        /// <returns></returns>
        internal static async Task CalculateRouteDemand()
        {
            await CalculateRouteDemand(false);
        }

        /// <summary>
        /// Task to calculate the route demand per month
        /// </summary>
        /// <param name="forceCalculation">Force the calculation</param>
        /// <returns></returns>
        internal static async Task CalculateRouteDemand(bool forceCalculation)
        {
#if DEBUG
            // Start the stopwatch to time the tasks
            var stopwatch = Stopwatch.StartNew();
#endif

            // Get all active airports
            ICollectionView AirportsView = Airport.GetAirports();
            List<Airport> Airports = AirportsView.Cast<Airport>().ToList();

            // Use a concurrent collection for thread safety
            double GlobalPassengers = GameData.Instance.GlobalModifiers.CurrentGlobalPassengers;
            DateTime CurrentDate = GameState.Instance.CurrentDate;
            ConcurrentBag<RouteDemand> RouteDemands = null;

            if (forceCalculation)
            {
                // new year, so calculate all route demands
                RouteDemands = new ConcurrentBag<RouteDemand>(GameData.Instance.RouteDemands);
            }
            else if (CurrentDate.Month == 1)
            {
                // new year, so calculate all route demands
                RouteDemands = new ConcurrentBag<RouteDemand>(GameData.Instance.RouteDemands);
            }
            else if (CurrentDate.Month == 4 || CurrentDate.Month == 11)
            {
                // not a new year, so no need to calculate all demands
                // however check the season influenced routes (Summer start is April, Winter start is November)
                RouteDemands = new ConcurrentBag<RouteDemand>(GameData.Instance.RouteDemands.Where(d => d.IsSeasonInfluenced));
            }

            // If RouteDemands is still null, then no need to calculate anything
            if(RouteDemands == null)
            {
                return;
            }

            // Dictionary for fast lookup of existing demands
            var routeLookup = new ConcurrentDictionary<(Airport, Airport), RouteDemand>(
                GameData.Instance.RouteDemands.ToDictionary(rd => (rd.Origin, rd.Destination))
            );

            // Create a list of tasks for parallel execution
            var tasks = new List<Task>();

            foreach (var origin in Airports)
            {
                tasks.Add(Task.Run(() =>
                {
                    var subTasks = new List<Task>();

                    foreach (var destination in Airports.Where(a => a != origin))
                    {
                        subTasks.Add(Task.Run(() =>
                        {
                            if (!routeLookup.TryGetValue((origin, destination), out RouteDemand routeDemand))
                            {
                                // Create a new RouteDemand if it doesn't exist
                                routeDemand = new RouteDemand(origin, destination);

                                // Add new RouteDemand safely
                                RouteDemands.Add(routeDemand);
                                routeLookup.TryAdd((origin, destination), routeDemand);
                            }

                            // Run demand calculations in serie (first the base should be calculated)
                            routeDemand.CalculateBaseDemand(GlobalPassengers);
                            routeDemand.CalculateDailyDemand();
                        }));

                    }

                    Task.WaitAll(subTasks.ToArray());
                }));
            }

            await Task.WhenAll(tasks);

            // Update GameData after calculations
            GameData.Instance.RouteDemands = RouteDemands.ToList();

#if DEBUG
            // Stop the stopwatch after all tasks are completed
            stopwatch.Stop();

            // [DEBUG] Log or display the time it took to complete the tasks
            Console.WriteLine($"CalculateRouteBaseDemand completed in {stopwatch.ElapsedMilliseconds} ms");
#endif
        }

        // Shared static Random instance
        private static class SharedRandom
        {
            public static readonly Random Instance = new Random();
        }
    }
}
