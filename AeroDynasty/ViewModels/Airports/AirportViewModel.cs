using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

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

        public ICollectionView DestinationAirports { get; set; }
        public ICollectionView DailyDemandAirports { get; set; }

        // Commands
        public ICommand OpenAirportCommand { get; }
        public event Action<Airport> OpenAirportRequest;

        // Constructor
        public AirportViewModel(Airport airport)
        {
            Airport = airport;

            // Create a collection from the filtered list
            var airportCollection = new ObservableCollection<Airport>(
                GameData.Instance.Routes
                    .Where(r => r.Origin == Airport || r.Destination == Airport)
                    .Select(r => r.Origin == Airport ? r.Destination : r.Origin)
                    .Distinct()
                    .ToList()
            );

            // Assign an ICollectionView that wraps the collection
            DestinationAirports = CollectionViewSource.GetDefaultView(airportCollection);

            // Get airport demands
            DailyDemandAirports = RouteDemand.GetRouteDemands(Airport);

            // Bind commands
            OpenAirportCommand = new RelayCommand(OpenAirport);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void OpenAirport(object parameters)
        {
            // If directly an airport is selected
            if (parameters is Airport selectedAirport)
            {
                OpenAirportRequest?.Invoke(selectedAirport);
            }

            // If a routedemand is selected
            if (parameters is RouteDemand selectedRouteDemand)
            {
                OpenAirportRequest?.Invoke(selectedRouteDemand.Destination);
            }
        }
    }
}
