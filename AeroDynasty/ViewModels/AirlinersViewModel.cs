using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Utilities;
using AeroDynasty.ViewModels.Airliners;

namespace AeroDynasty.ViewModels
{
    public class AirlinersViewModel : _BaseViewModel
    {
        // Private vars
        private _BaseViewModel _currentDetailContent;

        // Public vars 
        public _BaseViewModel CurrentDetailContent
        {
            get => _currentDetailContent;
            set
            {
                _currentDetailContent = value;
                OnPropertyChanged(nameof(CurrentDetailContent));
            }
        }

        // Commands
        public ICommand NavigateFleetCommand { get; }
        public ICommand NavigateManufacturersCommand { get; }

        // Constructor
        public AirlinersViewModel()
        {
            // Bind commands
            NavigateFleetCommand = new RelayCommand(NavigateFleet);
            NavigateManufacturersCommand = new RelayCommand(NavigateManufacturers);
        }

        // Private funcs


        // Public funcs

        // Command and event handling
        private void NavigateFleet()
        {
            var content = new FleetViewModel();
            CurrentDetailContent = content;
        }

        private void NavigateManufacturers()
        {
            var content = new ManufacturersViewModel();
            content.ManufacturerOpenRequest += OpenManufacturer;
            CurrentDetailContent = content;
        }

        private void OpenManufacturer(Manufacturer manufacturer)
        {
            //Open the manufacturer view
            CurrentDetailContent = new ManufacturerViewModel(manufacturer);
        }
    }
}
