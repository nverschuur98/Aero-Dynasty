using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using System;
using System.Collections.Generic;
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

        // Public vars
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
        public string Name { get => (Origin?.ICAO ?? string.Empty) + " - " + (Destination?.ICAO ?? string.Empty); }
        public Airline Owner { get => _owner; }
        public Price TicketPrice
        {
            get => _ticketPrice;
            set
            {
                _ticketPrice = value;
                OnPropertyChanged(nameof(TicketPrice));
            }
        }
        //public double Distance { GeoUtilities.CalculateDistance(Origin.Coordinates, Destination.Coordinates);}

    // Constructor
    public Route(Airport origin, Airport destination, Airline owner, Price ticketPrice)
        {
            Origin = origin;
            Destination = destination;
            _owner = owner;
            TicketPrice = ticketPrice;
        }

        // Private funcs

        // Public funcs
    }
}
