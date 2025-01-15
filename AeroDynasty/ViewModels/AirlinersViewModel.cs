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
        public ICommand NavigateManufacturersCommand { get; }

        // Constructor
        public AirlinersViewModel()
        {
            // Bind commands
            NavigateManufacturersCommand = new RelayCommand(NavigateManufacturers);
        }

        // Private funcs


        // Public funcs

        // Command handling
        private void NavigateManufacturers()
        {
            var manufacturersView = new ManufacturersViewModel();
            manufacturersView.ManufacturerOpenRequest += OpenManufacturer;
            CurrentDetailContent = manufacturersView;
        }

        private void OpenManufacturer(Manufacturer manufacturer)
        {
            //Open the manufacturer view
            CurrentDetailContent = new ManufacturerViewModel(manufacturer);
        }
    }
}
