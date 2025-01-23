using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteLeg
    {
        // Private vars

        // Public vars
        public TimeSpan DepartureTime { get; set; }
        public DayOfWeek DepartureDay { get; set; }
        public TimeSpan FlightDuration { get { return _flightDuration; } }
        public TimeSpan ArrivalTime { get { return _arrivalTime; } }
        public DayOfWeek ArrivalDay { get { return _arrivalDay; } }
        public Airliner AssignedAirliner { get; set; }
        public Airport Origin { get; set; }
        public Airport Destination { get; set; }
        public String Name { get { return _name; } }
        public bool IsNextDay { get => isNextDay(); }

        /// <summary>
        /// Creates a routeleg which is one leg of a route, from Origin to Destination, using the assigned airliner. 
        /// </summary>
        /// <param name="departureTime"></param>
        /// <param name="departureDay"></param>
        /// <param name="assignedAirliner"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        public RouteLeg(TimeSpan departureTime, DayOfWeek departureDay, Airliner assignedAirliner, Airport origin, Airport destination)
        {
            DepartureTime = departureTime;
            DepartureDay = departureDay;
            AssignedAirliner = assignedAirliner;
            Origin = origin;
            Destination = destination;
        }

        // Private funcs
        private double _distance
        {
            get
            {
                return GeoUtilities.CalculateDistance(Origin.Coordinates, Destination.Coordinates);
            }
        }

        private TimeSpan _flightDuration
        {
            get
            {
                TimeSpan flightTime = TimeSpan.FromHours(_distance / AssignedAirliner.Model.CruisingSpeed);
                return new TimeSpan(0, (int)Math.Ceiling(flightTime.TotalMinutes), 0);
            }
        }

        private TimeSpan _arrivalTime
        {
            get
            {
                return DepartureTime + FlightDuration;
            }
        }

        private DayOfWeek _arrivalDay
        {
            get
            {
                if (ArrivalTime < DepartureTime)
                {
                    return (DayOfWeek)(((int)DepartureDay + 1) % 7);
                }
                else
                {
                    return DepartureDay;
                }
            }
        }

        private bool isNextDay()
        {
            if(DepartureDay != ArrivalDay)
            {
                return true;
            }

            return false;
        }

        private String _name
        {
            get
            {
                return Origin.ICAO + " - " + Destination.ICAO;
            }
        }

        // Public funcs

    }
}
