using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Utilities;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AircraftModels;

namespace AeroDynasty.ViewModels.Airliners
{
    public class ManufacturerViewModel : _BaseViewModel
    {
        // Private vars
        private Manufacturer _manufacturer;

        // Public vars
        public Manufacturer Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged(nameof(Manufacturer));
            }
        }

        // Commands

        // Constructor
        public ManufacturerViewModel(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        // Private funcs

        // Public funcs

        // Command handling
    }
}
