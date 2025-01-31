using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.ViewModels.Airports
{
    public class AirportViewModel : _BaseViewModel
    {
        // Private vars
        private Airport _airport;

        // Public vars
        public Airport Airport
        {
            get => _airport;
            set
            {
                _airport = value;
                OnPropertyChanged(nameof(Airport));
            }
        }

        // Commands

        // Constructor
        public AirportViewModel(Airport airport)
        {
            Airport = airport;
        }

        // Private funcs

        // Public funcs

    }
}
