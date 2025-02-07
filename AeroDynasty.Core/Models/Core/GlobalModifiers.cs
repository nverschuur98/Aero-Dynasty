using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class GlobalModifiers : _BaseModel
    {
        // Private vars
        private Price _currentFuelPrice;
        private double _currentGlobalPassengers;

        // Public vars
        public Price CurrentFuelPrice
        {
            get => _currentFuelPrice;
            set
            {
                _currentFuelPrice = value;
                OnPropertyChanged(nameof(CurrentFuelPrice));
            }
        }
        public double CurrentGlobalPassengers
        {
            get => _currentGlobalPassengers;
            set
            {
                _currentGlobalPassengers = value;
                OnPropertyChanged(nameof(CurrentGlobalPassengers));
            }
        }

        // Maps
        public Dictionary<int, double> FuelPriceMap { get; set; }
        public Dictionary<int, double> GlobalPassengersMap { get; set; }

        // Constructor
        public GlobalModifiers()
        {
            
        }

        // Private funcs

        // Public funcs
    }
}
