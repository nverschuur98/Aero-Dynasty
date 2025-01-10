using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class MainWindowViewModel : _BaseViewModel
    {
        private _BaseViewModel _currentContent;

        public _BaseViewModel CurrentContent
        {
            get
            {
                return _currentContent;
            }
            set
            {
                _currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateRoutesCommand { get; }

        public MainWindowViewModel()
        {
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
