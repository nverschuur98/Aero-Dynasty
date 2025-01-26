using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.RouteModels;
using AeroDynasty.Core.Utilities;
using AeroDynasty.ViewModels.Routes;

namespace AeroDynasty.ViewModels
{
    public class RoutesViewModel : _BaseViewModel
    {
        // Private vars
        private _BaseViewModel _currentDetailContent;

        // Public vars
        public _BaseViewModel CurrentDetailContent
        {
            get => _currentDetailContent;
            set
            {
                _currentDetailContent = value;
                OnPropertyChanged(nameof(CurrentDetailContent));
            }
        }

        // Commands
        public ICommand NavigateRoutesCommand { get; }
        public ICommand NavigateNewRouteCommand { get; }

        // Constructor
        public RoutesViewModel()
        {

            // Bind commands
            NavigateRoutesCommand = new RelayCommand(NavigateRoutes);
            NavigateNewRouteCommand = new RelayCommand(NavigateNewRoute);
        }

        // Private funcs
        private void closeCurrentDetailContent()
        {
            CurrentDetailContent = null;
        }

        // Public funcs

        // Command and event handling
        private void NavigateRoutes()
        {
            var content = new Routes.RoutesViewModel();
            content.RouteOpenRequest += NavigateRoute;
            CurrentDetailContent = content;
        }

        private void NavigateRoute(Route route)
        {
            var content = new Routes.RouteViewModel(route);
            content.EditRouteRequest += NavigateEditRoute;
            CurrentDetailContent = content;
        }

        private void NavigateNewRoute()
        {
            var content = new EditRouteViewModel();
            content.CloseRequest += closeCurrentDetailContent;
            CurrentDetailContent = content;
        }

        private void NavigateEditRoute(Route route)
        {
            var content = new EditRouteViewModel(route);
            content.CloseRequest += closeCurrentDetailContent;
            CurrentDetailContent = content;
        }

    }
}
