using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class AirportsViewModel : _BaseViewModel
    {
        //Private vars
        private _BaseViewModel _currentDetailContent;

        //Public vars
        public _BaseViewModel CurrentDetailContent
        {
            get => _currentDetailContent;
            set
            {
                _currentDetailContent = value;
                OnPropertyChanged(nameof(CurrentDetailContent));
            }
        }

        //Commands
        public ICommand NavigateAirportsCommand { get; }

        //Constructor
        public AirportsViewModel()
        {

            // Bind commands
            NavigateAirportsCommand = new RelayCommand(NavigateAirports);
        }

        //Private functions

        // Public funcs

        // Command handling
        private void NavigateAirports()
        {
            var content = new Airports.AirportsViewModel();
            content.OpenAirportRequest += NavigateAirport;
            CurrentDetailContent = content;
        }

        private void NavigateAirport(Airport airport)
        {
            var content = new Airports.AirportViewModel(airport);
            content.OpenAirportRequest += NavigateAirport;
            CurrentDetailContent = content;
        }
    }
}
