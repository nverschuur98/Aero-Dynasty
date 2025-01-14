using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class AirlinesViewModel : _BaseViewModel
    {
        // Private vars
        private ICollectionView _airlines;
        private string _searchString;

        // Public vars
        public ICollectionView Airlines
        {
            get => _airlines;
            private set
            {
                _airlines = value;
                OnPropertyChanged(nameof(Airlines));
            }
        }
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
                Airlines.Refresh();
            }
        }

        // Commands

        // Constructor
        public AirlinesViewModel()
        {
            //Get all airlines
            Airlines = CollectionViewSource.GetDefaultView(GameData.Instance.Airlines);
            Airlines.Filter = FilterAirlines;

            //Apply sorting by name
            Airlines.SortDescriptions.Add(new SortDescription(nameof(Airline.Name), ListSortDirection.Ascending));
        }

        // Private funcs
        private bool FilterAirlines(object item)
        {

            // Check if the item is an Airline object.
            if (!(item is Airline airline))
            {
                return false;
            }

            // If the search string is null or empty, no filtering is applied, so all items pass.
            if (string.IsNullOrEmpty(SearchString))
            {
                return true;
            }

            // Check if the airline's name or country name contains the search string.
            return airline.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                airline.Country.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
           
        // Public funcs
           
        // Commando handling
    }
}
