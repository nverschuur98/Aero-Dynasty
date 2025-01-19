using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.Core.Models.AircraftModels
{
    public class AircraftCartItem : _BaseModel
    {
        // Private vars
        private int _amount;

        // Public vars
        public AircraftModel AircraftModel { get; set; }
        public Airline Buyer;

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public Price TotalPrice
        {
            get
            {
                return new Price(Amount * AircraftModel.Price.Amount);
            }
        }

        // Commands
        public ICommand BuyAircraftsCommand { get; }

        // Constructor
        public AircraftCartItem(AircraftModel aircraftModel, int amount, Airline buyer)
        {
            Amount = amount;
            AircraftModel = aircraftModel;
            Buyer = buyer;

            // Bind commands
            BuyAircraftsCommand = new RelayCommand(BuyAircrafts);
        }

        // Private funcs

        // Public funcs

        // Command handling
        private void BuyAircrafts()
        {
            // Check if cartitem is not empty
            if(Amount <= 0)
            {
                return;
            }

            for(int i = 0; i < Amount; i++)
            {
                Airliner a = new Airliner(new Registration(Buyer.Country), AircraftModel, Buyer, GameState.Instance.CurrentDate);

                // Check if sufficient money is available
                if (Buyer.SufficientCash(a.Model.Price))
                {
                    Buyer.SubtractCash(a.Model.Price);

                    GameData.Instance.Airliners.Add(a);
                }
                else
                {
                    // Not enough money available
                    return;
                }
            }
        }

    }
}
