﻿using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.AircraftModels
{
    
        /*"FuelConsumptionPerKm": "21.80",
        "OperatingCostPerKm": "1.90"*/

    public class AircraftModel
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
        public DateTime IntroductionDate { get; private set; }
        public string FormattedIntroductionDate => IntroductionDate.ToString("dd-MMM-yyyy");
        public DateTime RetirementDate { get; private set; }
        public string FormattedRetirementDate => RetirementDate.ToString("dd-MMM-yyyy");

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
            Manufacturer manufacturer,
            DateTime introductionDate,
            DateTime retirementDate)
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
            IntroductionDate = introductionDate;
            RetirementDate = retirementDate;
        }
    }
}
