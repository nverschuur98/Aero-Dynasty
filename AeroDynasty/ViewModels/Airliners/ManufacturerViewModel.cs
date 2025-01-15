using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Utilities;
using AeroDynasty.Core.Models;
using AeroDynasty.Core.Models.AircraftModels;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using AeroDynasty.ViewModels.Airliners.Popup;

namespace AeroDynasty.ViewModels.Airliners
{
    public class ManufacturerViewModel : _BaseViewModel
    {
        // Private vars
        private Manufacturer _manufacturer;
        private ICollectionView _aircraftModels;
        private string _popupTitle;
        private bool _popupVisible;
        private _BaseViewModel _popupContent;

        // Public vars
        public Manufacturer Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged(nameof(Manufacturer));
            }
        }
        public ICollectionView AircraftModels
        {
            get => _aircraftModels;
            set
            {
                _aircraftModels = value;
                OnPropertyChanged(nameof(AircraftModels));
            }
        }
        public string PopupTitle
        {
            get => _popupTitle;
            set
            {
                _popupTitle = value;
                OnPropertyChanged(nameof(PopupTitle));
            }
        }
        public bool PopupVisible
        {
            get => _popupVisible;
            private set
            {
                _popupVisible = value;
                OnPropertyChanged(nameof(PopupVisible));
            }
        }
        public _BaseViewModel PopupContent
        {
            get => _popupContent;
            set
            {
                _popupContent = value;
                PopupVisible = _popupContent != null;
                OnPropertyChanged(nameof(PopupContent));
                Console.WriteLine("Popup visible: " + PopupVisible);
            }
        } 

        // Commands
        public ICommand OpenAddAircraftToCartCommand { get; }

        // Constructor
        public ManufacturerViewModel(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
            AircraftModels = CollectionViewSource.GetDefaultView(GameData.Instance.AircraftModels.Where(m => m.Manufacturer == manufacturer));

            // Set sorting to aircraft models
            AircraftModels.SortDescriptions.Add(new SortDescription(nameof(AircraftModel.Name), ListSortDirection.Ascending));

            // Hide popup on start
            PopupVisible = false;
            PopupContent = null;

            // Bind commands to actions
            OpenAddAircraftToCartCommand = new RelayCommand(OpenAddAircraftToCart);
        }

        // Private funcs
        private void ClosePopup()
        {
            PopupTitle = "";
            PopupContent = null;
        }

        // Public funcs

        // Command handling
        private void OpenAddAircraftToCart()
        {
            PopupTitle = "Add aircraft to cart";
            var popupContent = new AddAircraftToCartPopupViewModel();
            popupContent.CloseRequest += ClosePopup;
            PopupContent = popupContent;
        }
    }
}
