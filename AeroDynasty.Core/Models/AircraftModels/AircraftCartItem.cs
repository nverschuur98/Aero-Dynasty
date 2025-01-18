using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.Core;
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
        public AircraftModel AircraftModel { get; set; }
        public Airline Buyer;
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public Price TotalPrice
        {
            get
            {
                return new Price(Amount * AircraftModel.Price.Amount);
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
