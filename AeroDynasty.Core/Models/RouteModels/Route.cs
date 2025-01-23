using AeroDynasty.Core.Models.AirlineModels;
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
            set
            {
                _scheduledFlights = value;
                OnPropertyChanged(nameof(ScheduledFlights));
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
        }

        // Private funcs

        // Public funcs
    }
}
