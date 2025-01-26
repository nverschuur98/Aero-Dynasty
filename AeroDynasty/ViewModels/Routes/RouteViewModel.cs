using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;
using AeroDynasty.ViewModels.Routes.Popup;

namespace AeroDynasty.ViewModels.Routes
{
    public class RouteViewModel :_BaseViewModel
    {
        // Private vars
        private Route _route;
        private string _popupTitle;
        private bool _popupVisible;
        private _BaseViewModel _popupContent;
        private Airliner _selectedAirliner;

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

        public string PopupTitle
        {
            get => _popupTitle;
            set
            {
                _popupTitle = value;
                OnPropertyChanged(nameof(PopupTitle));
            }
        }

        public bool PopupVisible
        {
            get => _popupVisible;
            private set
            {
                _popupVisible = value;
                OnPropertyChanged(nameof(PopupVisible));
            }
        }

        public _BaseViewModel PopupContent
        {
            get => _popupContent;
            set
            {
                _popupContent = value;
                PopupVisible = _popupContent != null;
                OnPropertyChanged(nameof(PopupContent));
                Console.WriteLine("Popup visible: " + PopupVisible);
            }
        }

        public Airliner SelectedAirliner
        {
            get => _selectedAirliner;
            set
            {
                _selectedAirliner = value;
                OnPropertyChanged(nameof(SelectedAirliner));
            }
        }

        public DayOfWeek SelectedDepartureDay { get; set; }
        public List<DayOfWeek> DepartureDays
        {
            get
            {
                // Get all DayOfWeek values
                var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

                // Move Sunday to the end, so it starts on Monday
                return days.Skip(1).Concat(days.Take(1)).ToList();
            }
        }

        public TimeSpan SelectedDepartureTime { get; set; }
        public List<TimeSpan> DepartureTimes
        {
            get
            {
                List<TimeSpan> departureTimes = new List<TimeSpan>();

                for(int q = 0; q < 15*4*24; q += 15)
                {
                    departureTimes.Add(new TimeSpan(0, q, 0));
                }

                return departureTimes;
            }
        }

        // Command and event binding
        public ICommand TestCommand { get; }
        public ICommand AddRouteScheduleCommand { get; }
        public ICommand OpenAddAirlinerCommand { get; }
        public ICommand ClearScheduleCommand { get; }

        // Constructor
        public RouteViewModel(Route route)
        {
            Route = route;

            TestCommand = new RelayCommand(testData);

            // Bind commands
            OpenAddAirlinerCommand = new RelayCommand(OpenAddAirliner);
            AddRouteScheduleCommand = new RelayCommand(AddRouteSchedule);

        }

        // Private funcs
        private void testData()
        {
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Monday, TimeSpan.FromHours(9), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Tuesday, TimeSpan.FromHours(12), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Wednesday, TimeSpan.FromHours(8), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Thursday, TimeSpan.FromHours(7), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            Route.ScheduledFlights.Add(new RouteSchedule(Route.Origin, Route.Destination, DayOfWeek.Friday, TimeSpan.FromHours(20), GameData.Instance.Airliners.First(a => a.Owner == GameData.Instance.UserData.Airline)));
            OnPropertyChanged(nameof(Route));
            OnPropertyChanged(nameof(Route.ScheduledFlights));
        }

        // Public funcs

        // Command handling
        private void OpenAddAirliner()
        {
            PopupTitle = "Asign airliner to route " + Route.Name;
            var popupContent = new AddAirlinerToRoutePopupViewModel(Route);
            popupContent.AssignAirlinerRequest += AssignAirliner;
            popupContent.CloseRequest += ClosePopup;
            PopupContent = popupContent;
        }

        private void AddRouteSchedule()
        {
            Route.AddSchedule(SelectedDepartureDay, SelectedDepartureTime, SelectedAirliner);
        }

        // Event handling
        private void AssignAirliner(Airliner airliner)
        {
            Route.AssignAirliner(airliner);
        }

        private void ClosePopup()
        {
            PopupTitle = "";
            PopupContent = null;
        }

    }
}
