using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class MainWindowViewModel : _BaseViewModel
    {
        //Private Vars
        private _BaseViewModel _currentContent;
        private UserData _userData;

        //Public Vars
        public _BaseViewModel CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public string FormattedCurrentDate{ get => GameState.Instance.FormattedCurrentDate; }
        public string CurrentDayOfWeek { get => GameState.Instance.CurrentDate.DayOfWeek.ToString(); }
        public Price CurrentFuelPrice { get => GameData.Instance.GlobalModifiers.CurrentFuelPrice; }
        public UserData UserData
        {
            get => _userData;
            set
            {
                if (_userData != value)
                {
                    if (_userData != null)
                    {
                        _userData.PropertyChanged -= UserData_PropertyChanged; // Unsubscribe from old instance
                        if (_userData.Airline != null)
                        {
                            _userData.Airline.CashBalance.PropertyChanged -= CashBalance_PropertyChanged;
                        }
                    }

                    _userData = value;
                    OnPropertyChanged(nameof(UserData));
                    OnPropertyChanged(nameof(GameIsLoaded));

                    if (_userData != null)
                    {
                        _userData.PropertyChanged += UserData_PropertyChanged; // Subscribe to new instance
                        if(_userData.Airline != null)
                        {
                            _userData.Airline.CashBalance.PropertyChanged += CashBalance_PropertyChanged;
                        }
                    }
                }
            }
        }
        public bool GameIsLoaded { get
            {
                return UserData.Airline == null ? false : true;
            } 
        }

        //Commands
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateRoutesCommand { get; }
        public ICommand NavigateAirportsCommand { get; }
        public ICommand NavigateAirlinersCommand { get; }
        public ICommand NavigateAirlinesCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }

        public MainWindowViewModel()
        {
            UserData = GameData.Instance.UserData; // This will trigger the setter logic

            // Subscribe to PropertyChanged event of GameData
            //GameData.Instance.PropertyChanged += GameData_PropertyChanged;

            // Subscribe to PropertyChanged event of GameData
            GameState.Instance.PropertyChanged += GameState_PropertyChanged;

            //Bind commands to actions
            NavigateHomeCommand = new RelayCommand(NavigateHome);
            NavigateRoutesCommand = new RelayCommand(NavigateRoutes);
            NavigateAirportsCommand = new RelayCommand(NavigateAirports);
            NavigateAirlinersCommand = new RelayCommand(NavigateAirliners);
            NavigateAirlinesCommand = new RelayCommand(NavigateAirlines);
            NavigateSettingsCommand = new RelayCommand(NavigateSettings);
            PlayCommand = new RelayCommand(Play);
            PauseCommand = new RelayCommand(Pause);
        }

        //Command Handling
        private void NavigateHome()
        {
            CurrentContent = new HomeViewModel();
        }

        private void NavigateRoutes()
        {
            CurrentContent = new RoutesViewModel();
        }
        private void NavigateAirports()
        {
            CurrentContent = new AirportsViewModel();
        }
        private void NavigateAirliners()
        {
            CurrentContent = new AirlinersViewModel();
        }
        private void NavigateAirlines()
        {
            CurrentContent = new AirlinesViewModel();
        }

        private void NavigateSettings()
        {
            CurrentContent = new SettingsViewModel();
        }

        private void Play()
        {
            GameState.Instance.PlayCommand.Execute(null);
        }

        private void Pause()
        {
            GameState.Instance.PauseCommand.Execute(null);
        }

        // Event handler for when properties change in GameData
        /*private void GameData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameData.UserData))
            {
                OnPropertyChanged(nameof(UserData)); // Notify that FormattedCurrentDate has changed
                OnPropertyChanged(nameof(UserData.Airline.CashBalance)); // Notify that FormattedCurrentDate has changed
                OnPropertyChanged(nameof(GameIsLoaded));
            }
        }*/

        private void CashBalance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Price.Amount))
            {
                OnPropertyChanged(nameof(UserData.Airline.CashBalance));
            }
        }

        private void UserData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserData.Airline))
            {
                OnPropertyChanged(nameof(GameIsLoaded)); // Refresh UI when Airline changes
            }
        }

        // Event handler for when properties change in GameState
        private void GameState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameState.CurrentDate))
            {
                OnPropertyChanged(nameof(FormattedCurrentDate)); // Notify that FormattedCurrentDate has changed
                OnPropertyChanged(nameof(CurrentDayOfWeek));
                OnPropertyChanged(nameof(CurrentFuelPrice)); // We know this changes daily, so we can request an update daily
            }
        }

    }
}
