﻿using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class Route : _BaseModel
    {
        // Private vars
        private Airport _origin;
        private Airport _destination;
        private Airline _owner;
        private Price _ticketPrice;
        private ObservableCollection<RouteSchedule> _scheduledFlights;
        private ObservableCollection<Airliner> _assignedAirliners;

        // Public vars
        public double Distance { get => GeoUtilities.CalculateDistance(Origin.Coordinates, Destination.Coordinates); }
        public string Name { get => (Origin?.ICAO ?? string.Empty) + " - " + (Destination?.ICAO ?? string.Empty); }
        public Airline Owner { get => _owner; }
        public Airport Origin
        {
            get => _origin;
            set
            {
                _origin = value;
                OnPropertyChanged(nameof(Origin));
            }
        }
        public Airport Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }
        public Price TicketPrice
        {
            get => _ticketPrice;
            set
            {
                _ticketPrice = value;
                OnPropertyChanged(nameof(TicketPrice));
            }
        }
        public ObservableCollection<RouteSchedule> ScheduledFlights
        {
            get => _scheduledFlights;
            private set
            {
                _scheduledFlights = value;
                OnPropertyChanged(nameof(ScheduledFlights));
            }
        }
        public ObservableCollection<Airliner> AssignedAirliners
        {
            get => _assignedAirliners;
            private set
            {
                _assignedAirliners = value;
                OnPropertyChanged(nameof(AssignedAirliners));
            }
        }

        // Constructor
        public Route(Airport origin, Airport destination, Airline owner, Price ticketPrice)
        {
            Origin = origin;
            Destination = destination;
            _owner = owner;
            TicketPrice = ticketPrice;
            ScheduledFlights = new ObservableCollection<RouteSchedule>();
            AssignedAirliners = new ObservableCollection<Airliner>();
        }

        // Private funcs
        /// <summary>
        /// Check if the airliner is available
        /// </summary>
        /// <param name="newSchedule"></param>
        /// <returns></returns>
        private bool IsAirlinerAvailable(RouteSchedule newSchedule)
        {
            foreach (var existingSchedule in ScheduledFlights)
            {
                if (existingSchedule.AssignedAirliner == newSchedule.AssignedAirliner)
                {
                    // Check for time overlap
                    if (SchedulesOverlap(existingSchedule, newSchedule))
                    {
                        return false; // Conflict found, airliner is not available
                    }
                }
            }
            return true; // No conflict found, airliner is available
        }

        /// <summary>
        /// Check if two RouteSchedules overlap
        /// </summary>
        /// <param name="existingSchedule"></param>
        /// <param name="newSchedule"></param>
        /// <returns></returns>
        private bool SchedulesOverlap(RouteSchedule existingSchedule, RouteSchedule newSchedule)
        {
            // Check same day of week
            if (existingSchedule.Outbound.DepartureDay == newSchedule.Outbound.DepartureDay)
            {
                TimeSpan existingStart = existingSchedule.Outbound.DepartureTime;
                TimeSpan existingEnd = existingSchedule.Inbound.ArrivalTime;

                TimeSpan newStart = newSchedule.Outbound.DepartureTime;
                TimeSpan newEnd = newSchedule.Inbound.ArrivalTime;

                // Check if the times overlap
                if (newStart < existingEnd && newEnd > existingStart)
                {
                    return true; // Conflict found
                }
            }

            // If arrival is next day, check next day's schedule too
            if (existingSchedule.NextDayArrival || newSchedule.NextDayArrival)
            {
                DayOfWeek nextDay = (DayOfWeek)(((int)existingSchedule.Outbound.DepartureDay + 1) % 7);

                if (newSchedule.Outbound.DepartureDay == nextDay)
                {
                    TimeSpan newStart = newSchedule.Outbound.DepartureTime;
                    TimeSpan newEnd = newSchedule.Inbound.ArrivalTime;

                    TimeSpan existingStart = existingSchedule.Outbound.DepartureTime;
                    TimeSpan existingEnd = existingSchedule.Inbound.ArrivalTime;

                    // Check overlap for next day
                    if (newStart < existingEnd && newEnd > existingStart)
                    {
                        return true; // Conflict found
                    }
                }
            }

            return false; // No conflict
        }

        // Public funcs
        public void AssignAirliner(Airliner airliner)
        {
            AssignedAirliners.Add(airliner);
        }

        /// <summary>
        /// Add a new scheduled flight 
        /// </summary>
        /// <param name="newSchedule"></param>
        public void AddSchedule(RouteSchedule newSchedule)
        {
            // Check if the airliner is available during the new schedule
            if (IsAirlinerAvailable(newSchedule))
            {
                ScheduledFlights.Add(newSchedule);
            }
            else
            {
                throw new ApplicationException("The airliner is already assigned to another flight at this time.");
            }
        }

        /// <summary>
        /// Add a new scheduled flight 
        /// </summary>
        /// <param name="departureDay"></param>
        /// <param name="departureTime"></param>
        /// <param name="airliner"></param>
        public void AddSchedule(DayOfWeek departureDay, TimeSpan departureTime, Airliner airliner)
        {
            RouteSchedule newSchedule = new RouteSchedule(Origin, Destination, departureDay, departureTime, airliner);
            AddSchedule(newSchedule);
        }

        public void RemoveAirliner(Airliner airliner)
        {
            // Create a list of flights associated with the airliner to remove
            var flightsToRemove = ScheduledFlights.Where(schedule => schedule.AssignedAirliner == airliner).ToList();

            // Remove those flights from the collection
            foreach (var flight in flightsToRemove)
            {
                ScheduledFlights.Remove(flight);
            }

            // Remove the airliner from assigned airliners list
            AssignedAirliners.Remove(airliner);
        }
    }
}
