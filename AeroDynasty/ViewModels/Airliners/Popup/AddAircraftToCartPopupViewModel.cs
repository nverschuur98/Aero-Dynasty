using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Airliners.Popup
{
    public class AddAircraftToCartPopupViewModel : _BaseViewModel
    {
        // Private vars

        // Public vars

        // Commands and Events
        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action CloseRequest;

        // Constructor
        public AddAircraftToCartPopupViewModel()
        {

            // Bind commands to actions
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void Add()
        {

        }

        private void Cancel()
        {
            CloseRequest?.Invoke();
        }
    }
}
