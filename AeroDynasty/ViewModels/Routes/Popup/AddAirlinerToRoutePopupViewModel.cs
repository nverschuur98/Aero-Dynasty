using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.ViewModels.Routes.Popup
{
    public class AddAirlinerToRoutePopupViewModel : _BaseViewModel
    {
        // Private vars
        private Route _route;
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
        public List<Airliner> Airliners {
            get => GameData.Instance.Airliners
                    .Where(air => air.Owner == Route.Owner &&
                    air.CanExecuteRoute(Route) &&
                    !Route.AssignedAirliners.Contains(air))
                    .ToList();
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

        // Commands and Events
        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action<Airliner> AssignAirlinerRequest;
        public event Action CloseRequest;

        // Constructor
        public AddAirlinerToRoutePopupViewModel(Route route)
        {
            Route = route;

            // Bind commands to actions
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void Add()
        {
            AssignAirlinerRequest?.Invoke(SelectedAirliner);
            CloseRequest?.Invoke();
        }

        private void Cancel()
        {
            CloseRequest?.Invoke();
        }
    }
}
