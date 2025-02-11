using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteDemand
    {
        private readonly UInt16[] _dailyDemand = new UInt16[7];
        public bool IsSeasonInfluenced { get; private set; }
        public Airport Origin { get; private set; }
        public Airport Destination { get; private set; }
        public double BaseFactor { get; private set; }
        public UInt16 BaseDemand { get; private set; }

        // Provides array-like access using DayOfWeek as index
        public UInt16 this[DayOfWeek day]
        {
            get => _dailyDemand[(int)day];
            set => _dailyDemand[(int)day] = value;
        }

        // Tweakable constants:
        private static readonly double _globalPassengerFactor = 0.2;   // scales global passenger count into our demand units
        private static readonly UInt16 _minDemand = 0;                       // minimum demand threshold
        private static readonly double _maxDistanceForScaling = 11250;   // km at which the distance factor becomes 0 (or nearly 0)

        // Cached size and type weights for performance
        private static readonly Dictionary<Enums.AirportSize, double> _sizeWeights = new Dictionary<Enums.AirportSize, double>
        {
            { Enums.AirportSize.Smallest, 0.15 },
            { Enums.AirportSize.VerySmall, 0.3 },
            { Enums.AirportSize.Small, 0.5 },
            { Enums.AirportSize.Medium, 1.0 },
            { Enums.AirportSize.Large, 1.25 },
            { Enums.AirportSize.VeryLarge, 1.6 },
            { Enums.AirportSize.Largest, 1.8 }
        };

        private static readonly Dictionary<Enums.AirportType, double> _typeWeights = new Dictionary<Enums.AirportType, double>
        {
            { Enums.AirportType.Domestic, 0.45 },
            { Enums.AirportType.Regional, 0.8 },
            { Enums.AirportType.International, 1.2 }
        };

        public RouteDemand(Airport origin, Airport destination)
        {
            Origin = origin;
            Destination = destination;
            IsSeasonInfluenced = (Origin.Season != Enums.FocusSeason.AllYear || Destination.Season != Enums.FocusSeason.AllYear) ? true : false;

            BaseFactor = CalculateBaseFactor();
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <returns></returns>
        public static ICollectionView GetRouteDemands(Airport airport)
        {
            return GetRouteDemands(airport, _ => true); // Calls the overloaded method with no extra filter
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <param name="additionalFilter">Selecting criteria</param>
        /// <returns></returns>
        public static ICollectionView GetRouteDemands(Airport airport, Func<RouteDemand, bool> additionalFilter)
        {
            var filteredRouteDemands = GameData.Instance.RouteDemands.Where(r => r.Origin == airport).Where(additionalFilter);
            var routeDemandsView = CollectionViewSource.GetDefaultView(filteredRouteDemands);
            routeDemandsView.SortDescriptions.Add(new SortDescription(nameof(RouteDemand.BaseFactor), ListSortDirection.Descending));

            return routeDemandsView;
        }

        /// <summary>
        /// Calculates the base demand for the route.
        /// Certain routes are skipped (demand = 0) based on unrealistic criteria.
        /// </summary>
        /// <param name="globalPassengers">The current global passenger count or modifier.</param>
        public void CalculateBaseDemand(double globalPassengers)
        {
            // Multiply the scaled global passenger count, the average airport weight, and the distance factor.
            double rawDemand = (globalPassengers) * BaseFactor;

            BaseDemand = Math.Max((UInt16)rawDemand, _minDemand);
            //Console.WriteLine($"BaseDemand; {Origin.Name}; {Destination.Name}; {BaseDemand}");
        }

        /// <summary>
        /// Distributes the BaseDemand over the days of the week according to pre-defined daily multipliers.
        /// </summary>
        public void CalculateDailyDemand()
        {
            double[] demandDistribution = GetDemandDistribution();

            for (int i = 0; i < 7; i++)
            {
                _dailyDemand[i] = (UInt16)(BaseDemand * demandDistribution[i]);
            }
        }

        private double CalculateBaseFactor()
        {
            double distance = GeoUtilities.CalculateDistance(Origin.Coordinates, Destination.Coordinates);

            if(Origin.ICAO == "EHAM" && Destination.IATA == "SUG")
            {

            }

            // Skip routes that don't make sense:
            // 0. Same city flights
            if(Origin.Town == Destination.Town)
            {
                BaseDemand = 0;
                return 0;
            }

            // 1. Domestic/Regional routes that are more than 2500 km apart.
            if ((Origin.Type == Enums.AirportType.Domestic || Origin.Type == Enums.AirportType.Regional) &&
                (Destination.Type == Enums.AirportType.Domestic || Destination.Type == Enums.AirportType.Regional) &&
                distance > 2500)
            {
                BaseDemand = 0;
                //Console.WriteLine($"Skipping demand calculation for {Origin.ICAO} to {Destination.ICAO}: Too far for domestic/regional routes.");
                return 0;
            }

            // 2. Domestic routes (both airports domestic) in different countries, unless very close (<= 300 km)
            if (Origin.Type == Enums.AirportType.Domestic && Destination.Type == Enums.AirportType.Domestic &&
                Origin.Country != Destination.Country && distance > 300)
            {
                BaseDemand = 0;
                //Console.WriteLine($"Skipping demand calculation for {Origin.ICAO} to {Destination.ICAO}: Different countries and too far.");
                return 0;
            }

            // 3. Small airports that are to far away
            if ((Origin.PassengerSize == Enums.AirportSize.Smallest || Origin.PassengerSize == Enums.AirportSize.VerySmall || Origin.PassengerSize == Enums.AirportSize.Small) &&
                (Destination.PassengerSize == Enums.AirportSize.Smallest || Destination.PassengerSize == Enums.AirportSize.VerySmall || Destination.PassengerSize == Enums.AirportSize.Small) &&
                distance > 3500)
            {
                BaseDemand = 0;
                //Console.WriteLine($"Skipping demand calculation for {Origin.ICAO} to {Destination.ICAO}: Too far for airport size.");
                return 0;
            }

            // 4. Small airports that are to far away
            if (((Origin.PassengerSize == Enums.AirportSize.Smallest || Origin.PassengerSize == Enums.AirportSize.VerySmall || Origin.PassengerSize == Enums.AirportSize.Small) && (Destination.PassengerSize == Enums.AirportSize.Large || Destination.PassengerSize == Enums.AirportSize.VeryLarge || Destination.PassengerSize == Enums.AirportSize.Largest) ||
                (Destination.PassengerSize == Enums.AirportSize.Smallest || Destination.PassengerSize == Enums.AirportSize.VerySmall || Destination.PassengerSize == Enums.AirportSize.Small) && (Origin.PassengerSize == Enums.AirportSize.Large || Origin.PassengerSize == Enums.AirportSize.VeryLarge || Origin.PassengerSize == Enums.AirportSize.Largest)) &&
                distance > 2500)
            {
                BaseDemand = 0;
                //Console.WriteLine($"Skipping demand calculation for {Origin.ICAO} to {Destination.ICAO}: Too far for airport size.");
                return 0;
            }

            // Compute individual weights.
            double destinationWeight = Math.Min(_sizeWeights[Destination.PassengerSize], _sizeWeights[Origin.PassengerSize]) * _typeWeights[Destination.Type];

            // Use the average weight to represent the route's attractiveness.
            double averageWeight = destinationWeight;

            // Calculate a normalized distance factor:
            // 1 when distance = 0, and decreases linearly to 0 when distance >= _maxDistanceForScaling.
            double distanceFactor = Math.Max(0.125, 1 - (distance / _maxDistanceForScaling));

            // Now combine the factors:
            // Multiply the scaled global passenger count, the average airport weight, and the distance factor.
            double baseFactor = _globalPassengerFactor * averageWeight * distanceFactor;

#if DEBUG
            //Console.WriteLine($"{Origin.ICAO} - {Destination.ICAO}; Destination Weight =; {destinationWeight};");
            //Console.WriteLine($"{Origin.ICAO} - {Destination.ICAO}; Average Weight =; {averageWeight};");
            //Console.WriteLine($"{Origin.ICAO} - {Destination.ICAO}; Distance Factor =; {distanceFactor};");
            //Console.WriteLine($"{Origin.ICAO} - {Destination.ICAO}; Base Factor =; {baseFactor} = {_globalPassengerFactor} * {averageWeight} * {distanceFactor};");

#endif
            return baseFactor;
        }

        /// <summary>
        /// Returns an array of 7 multipliers (one for each day) based on the sizes of the origin and destination.
        /// Index 0 = Sunday, 1 = Monday, etc.
        /// </summary>
        private double[] GetDemandDistribution()
        {
            // Use if/else (compatible with C# 7.3) to select the appropriate multiplier array.
            if (Origin.PassengerSize == Enums.AirportSize.Largest ||
                Origin.PassengerSize == Enums.AirportSize.VeryLarge ||
                Origin.PassengerSize == Enums.AirportSize.Large)
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        return new[] { 1.1, 1.05, 1.0, 1.0, 0.95, 0.9, 1.15 };
                    case Enums.AirportSize.Large:
                        return new[] { 1.0, 1.1, 1.2, 1.2, 1.1, 0.8, 0.7 };
                    case Enums.AirportSize.Medium:
                        return new[] { 0.95, 1.0, 1.1, 1.1, 1.0, 0.9, 0.85 };
                    default:
                        return new[] { 1.2, 0.9, 0.85, 0.85, 0.9, 1.1, 1.3 };
                }
            }
            else if (Origin.PassengerSize == Enums.AirportSize.Medium ||
                     Origin.PassengerSize == Enums.AirportSize.Small)
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        return new[] { 0.9, 1.1, 1.3, 1.3, 1.1, 0.7, 0.6 };
                    case Enums.AirportSize.Large:
                        return new[] { 1.0, 1.05, 1.15, 1.15, 1.05, 0.85, 0.75 };
                    case Enums.AirportSize.Medium:
                        return new[] { 1.1, 0.9, 0.9, 0.9, 0.9, 1.2, 1.3 };
                    default:
                        return new[] { 1.3, 0.8, 0.8, 0.8, 0.8, 1.2, 1.4 };
                }
            }
            else
            {
                switch (Destination.PassengerSize)
                {
                    case Enums.AirportSize.Largest:
                    case Enums.AirportSize.VeryLarge:
                        return new[] { 1.3, 0.8, 0.85, 0.85, 0.8, 1.2, 1.5 };
                    case Enums.AirportSize.Large:
                        return new[] { 1.25, 0.85, 0.85, 0.85, 0.85, 1.15, 1.4 };
                    case Enums.AirportSize.Medium:
                        return new[] { 1.2, 0.9, 0.9, 0.9, 0.9, 1.1, 1.3 };
                    default:
                        return new[] { 1.4, 0.7, 0.7, 0.7, 0.7, 1.3, 1.5 };
                }
            }
        }
    }
}
