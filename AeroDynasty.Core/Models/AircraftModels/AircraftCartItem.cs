using AeroDynasty.Core.Models.AirlineModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.AircraftModels
{
    public class AircraftCartItem : _BaseModel
    {
        // Private vars
        private int _amount;

        // Public vars
        public AircraftModel AircraftModel;
        public Airline Buyer;
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        // Constructor
        public AircraftCartItem(AircraftModel aircraftModel, int amount, Airline buyer)
        {
            Amount = amount;
            AircraftModel = aircraftModel;
            Buyer = buyer;
        }

        // Private funcs

        // Public funcs
    }
}
