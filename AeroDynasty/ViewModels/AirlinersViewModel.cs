using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            CurrentDetailContent = new ManufacturersViewModel();
        }
    }
}
