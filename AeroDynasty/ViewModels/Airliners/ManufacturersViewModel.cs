using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Airliners
{
    public class ManufacturersViewModel : _BaseViewModel
    {
        // Private vars
        private ICollectionView _manufacturers;
        private string _searchString;

        // Public vars
        public ICollectionView Manufacturers
        {
            get => _manufacturers;
            private set
            {
                _manufacturers = value;
                OnPropertyChanged(nameof(Manufacturers));
            }
        }
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
                Manufacturers.Refresh();
            }
        }

        // Commands and Events
        public ICommand OpenManufacturerCommand { get; }
        public event Action<Manufacturer> ManufacturerOpenRequest;

        // Constructor
        public ManufacturersViewModel()
        {
            // Get all manufacturers
            Manufacturers = CollectionViewSource.GetDefaultView(GameData.Instance.Manufacturers);
            Manufacturers.Filter = FilterManufacturers;

            // Apply sorting by name
            Manufacturers.SortDescriptions.Add(new SortDescription(nameof(Manufacturer.Name), ListSortDirection.Ascending));

            // Command binding
            OpenManufacturerCommand = new RelayCommand(OpenManufacturer);
        }

        // Private funcs
        private bool FilterManufacturers(object item)
        {

            // Check if the item is an Airline object.
            if (!(item is Manufacturer manufacturer))
            {
                return false;
            }

            // If the search string is null or empty, no filtering is applied, so all items pass.
            if (string.IsNullOrEmpty(SearchString))
            {
                return true;
            }

            // Check if the airline's name or country name contains the search string.
            return manufacturer.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                manufacturer.Country.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        // Public funcs

        // Commando handling
        private void OpenManufacturer(object parameter)
        {
            if(parameter is Manufacturer selectedManufacturer)
            {
                //Raise the event that a manufacturer must be opened
                ManufacturerOpenRequest?.Invoke(selectedManufacturer);
            }
        }
    }
}
