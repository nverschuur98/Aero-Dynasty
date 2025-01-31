using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Routes
{
    public class EditRouteViewModel : _BaseViewModel
    {
        // Private vars
        private bool _isNewRoute;
        private ICollectionView _airports;
        private string _routeName;
        private Route _route;
        private Airport _selectedOrigin;
        private Airport _selectedDestination;
        private Price _ticketPrice;

        // Public vars
        public ICollectionView OriginAirports { get; set; }
        public ICollectionView DestinationAirports { get; set; }
        public bool enableAirportSelection { get => _isNewRoute; }

        public Airport SelectedOrigin
        {
            get => _selectedOrigin;
            set
            {
                _selectedOrigin = value;
                OnPropertyChanged(nameof(SelectedOrigin));
                OnPropertyChanged(nameof(RouteDistance));

                // Filter DestinationAirports to exclude the selected origin
                FilterDestinationAirports();
            }
        }
        public Airport SelectedDestination
        {
            get => _selectedDestination;
            set
            {
                _selectedDestination = value;
                OnPropertyChanged(nameof(SelectedDestination));
                OnPropertyChanged(nameof(RouteDistance));
            }
        }

        public string RouteName
        {
            get => _routeName;
            set
            {
                _routeName = value;
                OnPropertyChanged(nameof(RouteName));
            }
        }
        public Route Route
        {
            get => _route;
            private set
            {
                _route = value;
                OnPropertyChanged(nameof(Route));
            }
        }
        public double RouteDistance
        {
            get
            {
                return SelectedOrigin?.Coordinates != null && SelectedDestination?.Coordinates != null
                    ? GeoUtilities.CalculateDistance(SelectedOrigin.Coordinates, SelectedDestination.Coordinates)
                    : 0;
            }
        }
        public Price TicketPrice
        {
            get => _ticketPrice;
            set
            {
                _ticketPrice = value;
                OnPropertyChanged(nameof(TicketPrice));
            }
        }
        
        // Commands and events
        public ICommand SaveRouteCommand { get; private set; }
        public event Action CloseRequest;

        // Constructor for creating a new route
        public EditRouteViewModel()
        {
            RouteName = "Create new route";
            _isNewRoute = true;

            loadData();
            setupCommands();
        }

        // Constructor
        public EditRouteViewModel(Route route)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route), "Route cannot be null");

            RouteName = Route.Name;
            
            loadData();
            setupCommands();

            SelectedOrigin = Route.Origin;
            SelectedDestination = Route.Destination;
            TicketPrice = Route.TicketPrice;
        }

        // Private funcs
        /// <summary>
        /// Load the data regardless of creating a new route or editing one
        /// </summary>
        private void loadData()
        {
            _airports = Airport.GetAirports();
            OriginAirports = _airports;
            DestinationAirports = _airports;

            TicketPrice = new Price(0);
        }

        /// <summary>
        /// Set the commands regardless of creating a new route or editing one
        /// </summary>
        private void setupCommands()
        {
            SaveRouteCommand = new RelayCommand(SaveRoute);
        }

        private void FilterDestinationAirports()
        {
            // If SelectedOrigin is null, reset DestinationAirports to all airports
            if (SelectedOrigin != null && _isNewRoute)
            {
                // Get all destination airports the route owner already flies to (in both directions)
                var existingDestinations = GameData.Instance.Routes
                    .Where(r => r.Owner == GameData.Instance.UserData.Airline &&
                                (r.Origin == SelectedOrigin || r.Destination == SelectedOrigin)) // Check both directions
                    .Select(r => r.Origin == SelectedOrigin ? r.Destination : r.Origin) // Extract the other endpoint of the route
                    .ToList();

                // Create a filtered list by excluding the selected origin and existing destinations (both directions)
                var filteredAirports = _airports.Cast<Airport>()
                    .Where(a => a != SelectedOrigin && !existingDestinations.Contains(a)) // Exclude both directions
                    .ToList();

                // Convert filtered list back to an ICollectionView
                DestinationAirports = CollectionViewSource.GetDefaultView(filteredAirports);

                // Refresh the collection view
                DestinationAirports.Refresh();
            }
            else
            {
                // Reset DestinationAirports if no origin is selected
                DestinationAirports = _airports;
                DestinationAirports.Refresh();
            }

            if(SelectedOrigin == SelectedDestination)
            {
                SelectedDestination = null;
                OnPropertyChanged(nameof(SelectedDestination));
            }

            OnPropertyChanged(nameof(DestinationAirports));
        }

        // Public funcs

        // Command and event handling
        private void SaveRoute()
        {
            if (_isNewRoute)
            {
                Route route = new Route(SelectedOrigin, SelectedDestination, GameData.Instance.UserData.Airline, TicketPrice);
                GameData.Instance.Routes.Add(route);
            }

            if (!_isNewRoute)
            {
                // TODO: Implement updating of route
            }

            CloseRequest?.Invoke();
        }
    }
}
