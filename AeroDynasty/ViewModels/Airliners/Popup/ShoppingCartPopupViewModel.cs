using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels.Airliners.Popup
{
    public class ShoppingCartPopupViewModel : _BaseViewModel
    {
        // Private vars
        private ObservableCollection<AircraftCartItem> _shoppingCart;

        // Public vars
        public ObservableCollection<AircraftCartItem> ShoppingCart
        {
            get => _shoppingCart;
            set
            {
                _shoppingCart = value;
                OnPropertyChanged(nameof(ShoppingCart));
            }
        }

        public int ShoppingCartCount { get => ShoppingCart.Count(); }
        public int ShoppingCartAircraftCount { get => ShoppingCart.Sum(a => a.Amount); }
        public Price ShoppingCartTotalPrice { get => new Price(ShoppingCart.Sum(a => a.TotalPrice.Amount)); }

        // Commands and Events
        public ICommand OrderCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }
        public event Action OrderCartRequest;
        public event Action CancelCartRequest;
        public event Action CloseRequest;

        // Constructor
        public ShoppingCartPopupViewModel(ObservableCollection<AircraftCartItem> shoppingCart)
        {
            ShoppingCart = shoppingCart;

            // Bind commands to actions
            OrderCommand = new RelayCommand(Order);
            CancelCommand = new RelayCommand(Cancel);
            CloseCommand = new RelayCommand(Close);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void Order()
        {
            OrderCartRequest?.Invoke();
            CloseRequest?.Invoke();
        }

        private void Cancel()
        {
            //Empty Cart
            CancelCartRequest?.Invoke();
            CloseRequest?.Invoke();
        }

        private void Close()
        {
            CloseRequest?.Invoke();
        }
    }
}
