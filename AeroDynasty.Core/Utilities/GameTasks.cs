﻿using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var stopwatch = Stopwatch.StartNew();
#endif

            // Get the current fuel price and current day
            double currentFuelPrice = GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount;
            DateTime currentDate = GameState.Instance.CurrentDate;
            DayOfWeek currentDay = currentDate.DayOfWeek;
            List<RouteDemand> routeDemands = GameData.Instance.RouteDemands;

            // Check if routeDemands is not empty or null
            if (routeDemands == null || routeDemands.Count == 0)
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
            var groupedLegs = allRouteLegs
                .GroupBy(rl => (rl.Leg.Origin, rl.Leg.Destination))
                .ToList();

            // Get the number of logical processors (cores + virtual cores)
            int logicalCores = Environment.ProcessorCount;

            // Define batch size (dividing RouteLegs over logical cores)
            int batchSize = Math.Max(1, groupedLegs.Count / logicalCores);

            // Create tasks for each batch
            var tasks = new List<Task>();

            // Split the grouped legs into batches and process each batch in parallel
            for (int i = 0; i < groupedLegs.Count; i += batchSize)
            {
                var batch = groupedLegs.Skip(i).Take(batchSize).ToList(); // This is the subset of the grouped legs to process
                tasks.Add(Task.Run(() =>
                {
                    foreach (var group in batch)
                    {
                        int localDemand = 0;

                        // Ensure routeDemands is not null
                        if (routeDemands == null)
                        {
                            throw new InvalidOperationException("routeDemands is not set.");
                        }

                        // Get the corresponding RouteDemand object, or null if not found
                        RouteDemand routeDemand = routeDemands.FirstOrDefault(rd =>
                            rd.Origin == group.Key.Origin && rd.Destination == group.Key.Destination);

                        // If routeDemand is found, get the demand for the current day; otherwise, localDemand remains 0
                        if (routeDemand != null)
                            localDemand = routeDemand[currentDay]; // Local copy of available demand

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
                            Price ticketIncome = filledPassengers * route.TicketPrice;

                            Price fuelCosts = new Price(route.Distance * leg.AssignedAirliner.Model.FuelConsumptionRate * currentFuelPrice) * -1; // Costs
                            Price airlinerCosts = new Price(1000) *-1; // Costs

                            Price cost = fuelCosts + airlinerCosts;
                            Price balance = ticketIncome + cost;

                            // Update route statistics
                            route.RouteStatistics.UpdateCurrentMonth(currentDate, ticketIncome, airlinerCosts, fuelCosts, filledPassengers, legSeatAvailability);

                            // Update airline balance
                            route.Owner.addCash(balance);

#if DEBUG
                            Console.WriteLine($"{currentDay} - {leg.Origin.ICAO}-{leg.Destination.ICAO} - Departure: {leg.DepartureTime}, Pax: {filledPassengers}, Profit: {ticketIncome + cost}, Demand left: {localDemand}");
#endif
                        }
                    }
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

#if DEBUG
            stopwatch.Stop();
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
        /// Task to check on newly available aircraftmodels and airports
        /// Based on the _PeriodModel class
        /// </summary>
        /// <returns></returns>
        internal static async Task CheckIsActive()
        {
            //Get the current Date
            DateTime currentDate = GameState.Instance.CurrentDate;

            await GameData.Instance.Airports.CheckIsActiveForAllAsync(currentDate);
            await GameData.Instance.AircraftModels.CheckIsActiveForAllAsync(currentDate);

            // Execute airport expansions that match the current date
            await GameData.Instance.Airports.CheckExpansionsForAllAsync(currentDate);
        }

        /// <summary>
        /// Task to check if there is an area that needs changing to an other country
        /// </summary>
        /// <returns></returns>
        internal static async Task CheckAreaChanges()
        {
            DateTime currentDate = GameState.Instance.CurrentDate;
            List<AreaChange> changesToExecute = GameData.Instance.AreaChanges.Where(c => c.ChangeDate <= currentDate).ToList();

            // If nothing to change, exit
            if (changesToExecute.Count() <= 0)
            {
                return;
            }

            // Execute changes
            await Task.Run(() =>
            {
                List<AreaChange> changesToRemove = new List<AreaChange>();

                // Execute the change in area
                foreach (AreaChange change in changesToExecute)
                {
                    AreaChangeManager.AssignAreaToCountry(change.Area, change.Country);
                    changesToRemove.Add(change);
                }

                // Remove the change from the change list
                foreach (AreaChange change in changesToRemove)
                {
                    GameData.Instance.AreaChanges.Remove(change);
                }
            });
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
            List<RouteDemand> RouteDemands = null;

            if (forceCalculation || CurrentDate.Month == 1)
            {
                // new year, so calculate all route demands
                RouteDemands = GameData.Instance.RouteDemands;
            }
            else if (CurrentDate.Month == 4 || CurrentDate.Month == 11)
            {
                // not a new year, but check season-influenced routes
                RouteDemands = GameData.Instance.RouteDemands.Where(d => d.IsSeasonInfluenced).ToList();
            }

            // If RouteDemands is still null, no need to calculate anything
            if (RouteDemands == null) return;

            // Dictionary for fast lookup of existing demands
            //var routeLookup = new ConcurrentDictionary<(Airport, Airport), RouteDemand>(GameData.Instance.RouteDemands.ToDictionary(rd => (rd.Origin, rd.Destination)));
            var routeLookup = new ConcurrentDictionary<(Airport, Airport), RouteDemand>(
                GameData.Instance.RouteDemands
                    .Where(rd => rd != null && rd.Origin != null && rd.Destination != null) // Ensure RouteDemand and its properties are not null
                    .ToDictionary(rd => (rd.Origin, rd.Destination))
            );

            // Get the number of logical processors (cores + virtual cores)
            int logicalCores = Environment.ProcessorCount;

            // Define batch size (dividing airports over logical cores)
            int batchSize = Math.Max(1, Airports.Count / logicalCores);

            // Create tasks for each batch
            var tasks = new List<Task>();

            // Split airports into batches and process each batch in parallel
            for (int i = 0; i < Airports.Count; i += batchSize)
            {
                var batch = Airports.Skip(i).Take(batchSize).ToList();
                tasks.Add(Task.Run(() =>
                {
                    foreach (var origin in batch)
                    {
                        foreach (var destination in Airports)
                        {
                            if (destination == origin) continue; // Skip same airport

                            // Check if RouteDemand exists, otherwise create it
                            if (!routeLookup.TryGetValue((origin, destination), out RouteDemand routeDemand))
                            {
                                routeDemand = new RouteDemand(origin, destination);
                                routeDemand.CalculateBaseDemand(GlobalPassengers);

                                if (routeDemand.BaseDemand <= GlobalPassengers / 100) continue; // Skip adding unnecessary routes

                                // Add only if it passes the demand check
                                if (routeDemand.Origin != null && routeDemand.Destination != null)
                                {
                                    lock (RouteDemands)
                                    {
                                        RouteDemands.Add(routeDemand);
                                        routeLookup[(origin, destination)] = routeDemand;
                                    }
                                }
                            }
                            else
                            {
                                // Only update demand calculations if RouteDemand already exists
                                routeDemand.CalculateBaseDemand(GlobalPassengers);

                                if (routeDemand.BaseDemand <= GlobalPassengers / 100)
                                {
                                    lock (RouteDemands)
                                    {
                                        RouteDemands.Remove(routeDemand);
                                    }
                                    continue;
                                }

                            }

                            // Now safe to calculate daily demand
                            routeDemand.CalculateDailyDemand();

                            if(routeDemand.Origin == null || routeDemand.Destination == null || routeDemand.BaseDemand == 0)
                            {
                                throw new Exception($"Error at {routeDemand.Origin} - {routeDemand.Destination}");
                            }
                        }
                    }
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // After all calculations are complete, filter out RouteDemands with zero base demand
            GameData.Instance.RouteDemands.RemoveAll(r => r == null || r.Origin == null || r.Destination == null);


            // Update GameData after calculations (only valid demands will be added)
            //GameData.Instance.RouteDemands = new List<RouteDemand>(validRouteDemands);


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
