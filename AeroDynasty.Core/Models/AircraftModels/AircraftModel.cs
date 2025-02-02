using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AeroDynasty.Core.Models.AircraftModels
{
    public class AircraftModel : _PeriodModel
    {
        public string Name { get; private set; }
        public string Family { get; private set; }
        public Price Price { get; private set; }
        public AircraftType Type { get; private set; }
        public EngineType EngineType { get; private set; }
        public int CruisingSpeed { get; private set; } //in km/h
        public int maxPax { get; private set; }
        public int maxCargo { get; private set; } //in kilogram
        public double maxRange { get; private set; } //in kilometer
        public int minRunwayLength { get; private set; } //in meter
        public double FuelConsumptionRate { get; private set; } // in L/km
        public double OperatingCostRate { get; private set; } // in dollar/km
        public Manufacturer Manufacturer { get; private set; }
        public string FormattedIntroductionDate => StartDate.ToString("dd-MMM-yyyy");
        public string FormattedRetirementDate => EndDate.ToString("dd-MMM-yyyy");

        public AircraftModel(
            string name,
            string family,
            Price price,
            AircraftType type,
            EngineType engineType,
            int cruisingSpeed,
            int maxPax,
            int maxCargo,
            double maxRange,
            int minRunwayLength,
            double fuelConsumptionRate,
            double operatingCostRate,
            Manufacturer manufacturer)
        {
            Name = name;
            Family = family;
            Price = price;
            Type = type;
            EngineType = engineType;
            CruisingSpeed = cruisingSpeed;
            this.maxPax = maxPax; // Use "this." to avoid shadowing, but it's not strictly necessary here.
            this.maxCargo = maxCargo; // Use "this." to avoid shadowing.
            this.maxRange = maxRange; // Use "this." to avoid shadowing.
            this.minRunwayLength = minRunwayLength; // Use "this." to avoid shadowing.
            FuelConsumptionRate = fuelConsumptionRate;
            OperatingCostRate = operatingCostRate;
            Manufacturer = manufacturer;
        }

        /// <summary>
        /// Return all active aircraftmodels in the game as an ICollectionView
        /// </summary>
        /// <returns></returns>
        public static ICollectionView GetAircraftModels()
        {
            return GetAircraftModels(_ => true); // Calls the overloaded method with no extra filter
        }

        /// <summary>
        /// Return all active aircraftmodels in the game as an ICollectionView
        /// </summary>
        /// <param name="additionalFilter">Selecting criteria</param>
        /// <returns></returns>
        public static ICollectionView GetAircraftModels(Func<AircraftModel, bool> additionalFilter)
        {
            var filteredAircraftModels = GameData.Instance.AircraftModels.Where(a => a.IsActive).Where(additionalFilter);
            var aircraftModelsView = CollectionViewSource.GetDefaultView(filteredAircraftModels);
            aircraftModelsView.SortDescriptions.Add(new SortDescription(nameof(AircraftModel.Name), ListSortDirection.Ascending));

            return aircraftModelsView;
        }
    }
}
