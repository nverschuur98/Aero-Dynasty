using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AeroDynasty.Core.Models.AirportModels
{
    public class Airport : _PeriodModel
    {
        public string Name { get; set; }
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public AirportType Type { get; set; }
        public AirportSize PassengerSize { get; set; }
        public FocusSeason Season { get; set; }
        public Country Country { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<Runway> Runways { get; set; }

        public Airport(string name, string iata, string icao, AirportType type, AirportSize passengerSize , FocusSeason season , Country country, Coordinates coordinates, List<Runway> runways)
        {
            Name = name;
            IATA = iata;
            ICAO = icao;
            Type = type;
            PassengerSize = passengerSize;
            Season = season;
            Country = country;
            Coordinates = coordinates;
            Runways = runways;
        }

        // Assuming images are stored in the Assets folder
        public string CountryFlag
        {
            get
            {
                string path = $"Assets/Flags/{Country.ISOCode.ToLower()}.png"; // Adjust the extension as necessary
                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"File not found: {path}");
                }
                return path;
            }
        }

        public string Codes
        {
            get
            {
                return ICAO + "/" + IATA;
            }
        }

        /// <summary>
        /// Return the airport in "(ICAO) Name" format
        /// </summary>
        public string ICAO_Name
        {
            get
            {
                return "(" + ICAO + ") " + Name;
            }
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <returns></returns>
        public static ICollectionView GetAirports()
        {
            return GetAirports(_ => true); // Calls the overloaded method with no extra filter
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <param name="additionalFilter">Selecting criteria</param>
        /// <returns></returns>
        public static ICollectionView GetAirports(Func<Airport, bool> additionalFilter)
        {
            var filteredAirports = GameData.Instance.Airports.Where(a => a.IsActive).Where(additionalFilter);
            var airportsView = CollectionViewSource.GetDefaultView(filteredAirports);
            airportsView.SortDescriptions.Add(new SortDescription(nameof(Airport.Name), ListSortDirection.Ascending));

            return airportsView;
        }

    }

}
