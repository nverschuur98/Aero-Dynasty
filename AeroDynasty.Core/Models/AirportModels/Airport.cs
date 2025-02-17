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
        public Area Area { get; set; }
        public Country Country { get => Area.GetCountry(); }
        public Coordinates Coordinates { get; set; }
        public string Town { get; set; }
        public List<Runway> Runways { get; set; }
        public List<AirportExpansion> Expansions { get; set; }

        public Airport(string name, string iata, string icao, AirportType type, AirportSize passengerSize , FocusSeason season , Area area, Coordinates coordinates, string town, List<Runway> runways, List<AirportExpansion> expansions)
        {
            Name = name;
            IATA = iata;
            ICAO = icao;
            Type = type;
            PassengerSize = passengerSize;
            Season = season;
            Area = area;
            Coordinates = coordinates;
            Town = town;
            Runways = runways;
            Expansions = expansions;
        }

        // Assuming images are stored in the Assets folder
        public string CountryFlag
        {
            get
            {
                string path = $"Assets/Flags/{Country.ISO2Code.ToLower()}.png"; // Adjust the extension as necessary
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
        /// Executes expansions scheduled for the given date.
        /// </summary>
        public async Task ExecuteExpansionsAsync(DateTime currentDate)
        {
            var expansionsToExecute = Expansions.Where(expansion => expansion.Date <= currentDate).ToList();

            if (expansionsToExecute.Count <= 0)
                return;

            foreach (var expansion in expansionsToExecute)
            {
                expansion.Execute(this); // Executes the expansion on this airport
                await Task.CompletedTask; // Placeholder for async logic if needed
            }

            // Optionally remove executed expansions if needed (game logic)
            Expansions.RemoveAll(expansion => expansion.Date <= currentDate);
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
