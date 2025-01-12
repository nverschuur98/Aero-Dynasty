using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class AirportsViewModel : _BaseViewModel
    {
        //Private vars
        private ObservableCollection<Airport> _airports;

        //Public vars
        public ObservableCollection<Airport> Airports
        {
            get => _airports;
            set
            {
                _airports = value;
                OnPropertyChanged(nameof(Airports));
            }
        }

        //Commands

        //Constructor
        public AirportsViewModel()
        {
            Airports = GameData.Instance.Airports;
        }
    }
}
