using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Routes
{
    public class RouteViewModel :_BaseViewModel
    {
        // Private vars
        private Route _route;

        // Public vars
        public Route Route
        {
            get => _route;
            set
            {
                _route = value;
                OnPropertyChanged(nameof(Route));
            }
        }

        public RouteSchedule _test;
        public RouteSchedule testSchedule
        {
            get => _test;
            set
            {
                _test = value;
                OnPropertyChanged(nameof(testSchedule));
            }
        }


        // Command binding
        public ICommand TestCommand { get; }

        // Constructor
        public RouteViewModel(Route route)
        {
            Route = route;
            TestCommand = new RelayCommand(testData);
        }

        // Private funcs
        private void testData()
        {
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Monday, TimeSpan.FromHours(9), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Tuesday, TimeSpan.FromHours(12), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Wednesday, TimeSpan.FromHours(8), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Thursday, TimeSpan.FromHours(7), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Friday, TimeSpan.FromHours(20), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            testSchedule = Route.ScheduledFlights.First();
            OnPropertyChanged(nameof(Route));
            OnPropertyChanged(nameof(Route.ScheduledFlights));
        }

        // Public funcs

        // Command handling
    }
}
