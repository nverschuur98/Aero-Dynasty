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

        // Constructor
        public GlobalModifiers()
        {
            CurrentFuelPrice = new Price(0.7);
        }

        // Private funcs

        // Public funcs
    }
}
