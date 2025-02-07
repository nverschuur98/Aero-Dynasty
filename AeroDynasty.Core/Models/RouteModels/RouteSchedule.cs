using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.AirportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteSchedule
    {
        // Private vars

        // Public vars
        public RouteLeg Outbound { get; }
        public RouteLeg Inbound { get; }
        public Airliner AssignedAirliner { get; }
        public TimeSpan TurnaroundTime { get; set; }
        public bool NextDayArrival { get => (Outbound.IsNextDay || Inbound.IsNextDay) ? true : false; }

        public RouteSchedule(Airport Origin, Airport Destination, DayOfWeek DayOfWeek, TimeSpan DepartureTime, Airliner assignedAirliner)
        {
            TurnaroundTime = TimeSpan.FromMinutes(30);
            AssignedAirliner = assignedAirliner;

            RouteLeg _out = new RouteLeg(DepartureTime, DayOfWeek, AssignedAirliner, Origin, Destination);

            DayOfWeek _inDepartureDay = _out.ArrivalDay;
            TimeSpan _inDepartureTime = _out.ArrivalTime + TurnaroundTime;


            // Check if departure day is next day
            if ((_inDepartureTime) < _out.ArrivalTime)
            {
                _inDepartureDay = (DayOfWeek)(((int)_out.DepartureDay + 1) % 7);
            }

            RouteLeg _in = new RouteLeg(_inDepartureTime, _inDepartureDay, AssignedAirliner, Destination, Origin);

            Outbound = _out;
            Inbound = _in;
        }
    }
}
