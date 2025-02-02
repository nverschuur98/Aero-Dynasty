using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.ViewModels.Settings
{
    public class NewGameViewModel : _BaseViewModel
    {
        // Private vars

        // Public vars
        public List<int> AvailableStartingYears { get; set; }
        public int SelectedStartingYear { get; set; }
        public List<Airline> AvailableAirlines { get; set; }
        public Airline SelectedAirline { get; set; }

        // Commands
        public ICommand CreateNewGameCommand
        { get; }

        // Constructor
        public NewGameViewModel()
        {
            // Static list for starting years
            AvailableStartingYears = new List<int>();
            AvailableStartingYears.Add(1946);
            AvailableStartingYears.Add(1947);
            AvailableStartingYears.Add(1948);

            // Static list for airlines
            AvailableAirlines = new List<Airline>();
            AvailableAirlines.Add(GameData.Instance.Airlines.Where(a => a.Name.Contains("KLM")).First());

            // Bind commands
            CreateNewGameCommand = new RelayCommand(CreateNewGame);
        }

        // Private vars

        // Public vars

        // Command and event handling
        private void CreateNewGame()
        {
            //SaveGameManager.NewGame();
        }
    }
}
