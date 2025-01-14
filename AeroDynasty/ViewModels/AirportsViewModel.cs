using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class AirportsViewModel : _BaseViewModel
    {
        //Private vars
        private ICollectionView _airports;
        private string _searchString;
        private ICollectionView _airportsDomestic;
        private string _searchStringDomestic;

        //Public vars
        public ICollectionView Airports
        {
            get => _airports;
            private set
            {
                _airports = value;
                OnPropertyChanged(nameof(Airports));
            }
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
                Airports.Refresh();
            }
        }

        public ICollectionView AirportsDomestic
        {
            get => _airportsDomestic;
            private set
            {
                _airportsDomestic = value;
                OnPropertyChanged(nameof(AirportsDomestic));
            }
        }

        public string SearchStringDomestic
        {
            get => _searchStringDomestic;
            set
            {
                _searchStringDomestic = value;
                OnPropertyChanged(nameof(SearchStringDomestic));
                AirportsDomestic.Refresh();
            }
        }

        //Commands

        //Constructor
        public AirportsViewModel()
        {
            //Get all airports
            Airports = CollectionViewSource.GetDefaultView(GameData.Instance.Airports);
            Airports.Filter = FilterAirports;

            //Get all domestic airports
            AirportsDomestic = CollectionViewSource.GetDefaultView(GameData.Instance.Airports.Where(a => a.Country == GameData.Instance.UserData.Airline.Country).ToList());
            AirportsDomestic.Filter = FilterAirportsDomestic;

            // Apply sorting by Name (ascending)
            Airports.SortDescriptions.Add(new SortDescription(nameof(Airport.Name), ListSortDirection.Ascending));
            AirportsDomestic.SortDescriptions.Add(new SortDescription(nameof(Airport.Name), ListSortDirection.Ascending));
        }

        //Private functions
        private bool FilterAirports(object item)
        {
            if (!(item is Airport airport))
            {
                return false;
            }

            if (string.IsNullOrEmpty(SearchString))
            {
                return true;
            }

            return airport.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                   airport.IATA?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                   airport.ICAO?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                   airport.Country?.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private bool FilterAirportsDomestic(object item)
        {
            if (!(item is Airport airport))
            {
                return false;
            }

            if (string.IsNullOrEmpty(SearchStringDomestic))
            {
                return true;
            }

            return airport.Name?.IndexOf(SearchStringDomestic, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                   airport.IATA?.IndexOf(SearchStringDomestic, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                   airport.ICAO?.IndexOf(SearchStringDomestic, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
