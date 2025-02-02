using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Utilities;
using AeroDynasty.ViewModels.Settings;

namespace AeroDynasty.ViewModels
{
    public class SettingsViewModel : _BaseViewModel
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
        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        // Constructor
        public SettingsViewModel()
        {
            NewCommand = new RelayCommand(New);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
        }

        // Private vars

        // Public vars

        // Command and event handling
        private void New()
        {
            //SaveGameManager.NewGame();
            var content = new NewGameViewModel();
            content.CloseRequest += closeCurrentDetailContent;
            CurrentDetailContent = content;
        }
        private void Save()
        {
            SaveGameManager.SaveGame("Save/savegame.json");
        }

        private void Load()
        {
            SaveGameManager.LoadGame("Save/savegame.json");
        }

        private void closeCurrentDetailContent()
        {
            CurrentDetailContent = null;
        }
    }
}
