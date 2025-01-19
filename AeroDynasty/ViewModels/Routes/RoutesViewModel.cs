using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Routes
{
    public class RoutesViewModel : _BaseViewModel
    {
        // Private vars
        private ICollectionView _routes;
        private string _searchString;

        // Public vars
        public ICollectionView Routes { get; set; }
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
                Routes.Refresh();
            }
        }

        // Commands and events

        // Constructor
        public RoutesViewModel()
        {
            Routes = CollectionViewSource.GetDefaultView(GameData.Instance.Routes.Where(r => r.Owner == GameData.Instance.UserData.Airline));
            Routes.SortDescriptions.Add(new SortDescription(nameof(Route.Origin.Name), ListSortDirection.Ascending));
            Routes.Filter = FilterRoutes;
        }

        // Private funcs
        private bool FilterRoutes(object item)
        {
            // Check if the item is an Route object
            if(!(item is Route route))
            {
                return false;
            }

            // If the string is null or empty, no filtering is applied
            if (string.IsNullOrEmpty(SearchString))
            {
                return true;
            }

            // Check if routes origin or destination name or ICAO is in the list
            return route.Origin?.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                route.Origin?.ICAO?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                route.Destination?.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                route.Destination?.ICAO?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                route.Name?.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        // Public funcs

        // Command and event handling

    }
}
