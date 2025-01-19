using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Airliners
{
    public class FleetViewModel : _BaseViewModel
    {
        // Private vars
        private ICollectionView _fleet;

        // Public vars
        public ICollectionView Fleet
        {
            get => _fleet;
            set
            {
                _fleet = value;
                OnPropertyChanged(nameof(Fleet));
            }
        }

        // Commands

        // Constructor
        public FleetViewModel()
        {
            // Get the fleet from gamedata
            Fleet = CollectionViewSource.GetDefaultView(GameData.Instance.Airliners.Where(a => a.Owner == GameData.Instance.UserData.Airline));
        }

        // Private funcs

        // Public funcs

        // Command handling

    }
}
