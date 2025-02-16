using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.ViewModels.Settings
{
    public class NewGameViewModel : _BaseViewModel
    {
        // Private vars
        private int _selectedStartingYear;
        private Airline _selectedAirline;
        private bool _canCreateNewGame;

        // Public vars
        public List<int> AvailableStartingYears { get; set; }
        public int SelectedStartingYear
        {
            get => _selectedStartingYear;
            set
            {
                _selectedStartingYear = value;
                OnPropertyChanged(nameof(SelectedStartingYear));
                CanCreateNewGame = canCreateNewGame();
            }
        }
        public ObservableCollection<Airline> AvailableAirlines { get; set; }
        public Airline SelectedAirline
        {
            get => _selectedAirline;
            set
            {
                _selectedAirline = value;
                OnPropertyChanged(nameof(SelectedAirline));
                CanCreateNewGame = canCreateNewGame();
            }
        }
        public bool CanCreateNewGame
        {
            get => _canCreateNewGame;
            set
            {
                _canCreateNewGame = value;
                OnPropertyChanged(nameof(CanCreateNewGame));
            }
        }

        // Commands and events
        public ICommand CreateNewGameCommand { get; }
        public event Action CloseRequest;

        // Constructor
        public NewGameViewModel()
        {
            // Static list for starting years
            AvailableStartingYears = new List<int>();

            for(int y = 1946; y <= DateTime.Now.Year; y++)
            {
                AvailableStartingYears.Add(y);
            }

            // Static list for airlines
            AvailableAirlines = GameData.Instance.Airlines;

            // Bind commands
            CreateNewGameCommand = new RelayCommand(CreateNewGame);
        }

        // Private funcs
        private bool canCreateNewGame()
        {
            bool can = SelectedAirline != null && SelectedStartingYear != 0;
            return can;
        }

        // Public funcs

        // Command and event handling
        private void CreateNewGame()
        {
            if (!_canCreateNewGame)
                return;

            SaveGameManager.NewGame(SelectedAirline, SelectedStartingYear);
            CloseRequest?.Invoke();
        }
    }
}
