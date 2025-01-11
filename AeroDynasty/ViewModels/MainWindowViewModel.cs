using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class MainWindowViewModel : _BaseViewModel
    {
        private _BaseViewModel _currentContent;

        public _BaseViewModel CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public string FormattedCurrentDate
        {
            get => GameData.Instance.FormattedCurrentDate;
        }
        public UserData UserData { get; set; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateRoutesCommand { get; }

        public MainWindowViewModel()
        {
            UserData = GameData.Instance.UserData;

            // Subscribe to PropertyChanged event of GameData
            //GameData.Instance.PropertyChanged += GameData_PropertyChanged;

            NavigateHomeCommand = new RelayCommand(NavigateHome);
            NavigateRoutesCommand = new RelayCommand(NavigateRoutes);
        }

        private void NavigateHome()
        {
            CurrentContent = new HomeViewModel();
        }

        private void NavigateRoutes()
        {
            CurrentContent = new RoutesViewModel();
        }
    }
}
