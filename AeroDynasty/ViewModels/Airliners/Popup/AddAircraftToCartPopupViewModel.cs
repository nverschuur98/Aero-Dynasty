using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Airliners.Popup
{
    public class AddAircraftToCartPopupViewModel : _BaseViewModel
    {
        // Private vars
        private AircraftModel _aircraftModel;
        private int _selectedAmount;

        // Public vars
        public List<int> Amounts { get; set; }
        public int SelectedAmount
        {
            get => _selectedAmount;
            set
            {
                _selectedAmount = value;
                OnPropertyChanged(nameof(SelectedAmount));
                OnPropertyChanged(nameof(TotalAmount));
            }
        }
        public Price TotalAmount { get => new Price(AircraftModel.Price.Amount * SelectedAmount); }
        public AircraftModel AircraftModel
        {
            get => _aircraftModel;
            set
            {
                _aircraftModel = value;
                OnPropertyChanged(nameof(AircraftModel));
            }
        }

        // Commands and Events
        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action<AircraftCartItem> AddCartItemRequest;
        public event Action CloseRequest;

        // Constructor
        public AddAircraftToCartPopupViewModel(AircraftModel aircraftModel)
        {
            AircraftModel = aircraftModel;
            Amounts = new List<int>();

            for (int i = 1; i <= 50; i++)
            {
                Amounts.Add(i);
            }

            SelectedAmount = 1;

            // Bind commands to actions
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void Add()
        {
            AddCartItemRequest?.Invoke(new AircraftCartItem(AircraftModel, SelectedAmount, GameData.Instance.UserData.Airline));
            CloseRequest?.Invoke();
        }

        private void Cancel()
        {
            CloseRequest?.Invoke();
        }
    }
}
