using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class SettingsViewModel : _BaseViewModel
    {
        // Private vars

        // Public vars

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        // Constructor
        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
        }

        // Private vars

        // Public vars

        // Command and event handling
        private void Save()
        {
            SaveGameManager.SaveGame("Save/savegame.json");
        }

        private void Load()
        {
            SaveGameManager.LoadGame("Save/savegame.json");
        }
    }
}
