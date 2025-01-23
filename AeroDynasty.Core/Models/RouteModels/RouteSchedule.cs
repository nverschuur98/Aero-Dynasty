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
        public TimeSpan TurnaroundTime { get; set; }
        public bool NextDayArrival { get => (Outbound.IsNextDay || Inbound.IsNextDay) ? true : false; }

        public RouteSchedule(Airport Origin, Airport Destination, DayOfWeek DayOfWeek, TimeSpan DepartureTime, Airliner AssignedAirliner)
        {
            TurnaroundTime = TimeSpan.FromMinutes(30);

            RouteLeg _out = new RouteLeg(DepartureTime, DayOfWeek, AssignedAirliner, Origin, Destination);
            RouteLeg _in = new RouteLeg(_out.ArrivalTime + TurnaroundTime, _out.ArrivalDay, AssignedAirliner, Destination, Origin);

            Outbound = _out;
            Inbound = _in;
        }
    }
}
