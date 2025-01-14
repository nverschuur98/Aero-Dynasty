using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;

namespace AeroDynasty.Core.Utilities
{
    public class GameData : _BaseViewModel
    {
        //Singleton Instance
        private static GameData _instance;
        public static GameData Instance => _instance ?? (_instance = new GameData());

        //Observable Core Data
        public ObservableCollection<Country> Countries { get; private set; }
        public ObservableCollection<RegistrationTemplate> RegistrationTemplates { get; set; }

        //Observable non-change Data
        public ObservableCollection<Airline> Airlines { get; private set; }
        public ObservableCollection<Airport> Airports { get; private set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; private set; }
        public ObservableCollection<AircraftModel> AircraftModels { get; private set; }
        
        ////Observable change Data
        //public ObservableCollection<Route> Routes { get; set; }
        //public ObservableCollection<Airliner> Airliners { get; set; }
        
        //Maps
        public Dictionary<string, Country> CountryMap { get; private set; }
        public Dictionary<Country, RegistrationTemplate> RegistrationTemplateMap { get; set; }
        //public Dictionary<int, double> Inflations { get; private set; }
        
        // Userdata
        private UserData _userData { get; set; }
        
        //Commands
        //public ICommand TestCommand { get; set; }

        //Private Constructor
        private GameData()
        {           
            //Load Core first
            LoadCoreData();
            
            //Load non-change data
            LoadNonChangeData();
            
            ////Load change data
            ////THIS NEEDS TO MOVE UNTILL AFTER THE CTOR IS FULLY INITIALIZED
            //LoadAirliners();
            //LoadRoutes();
            //
            //Init start date
            Airline arl = Airlines.Where(al => al.Name.Contains("KLM")).FirstOrDefault();
            
            UserData = new UserData(arl);
            
            //Bind commands
            //TestCommand = new RelayCommand(LoadTestData);
        }

        //Private funcs

        /// <summary>
        /// Load all core data
        /// Core data will never change during any game
        /// </summary>
        private void LoadCoreData()
        {
            LoadCountries();
        }

        /// <summary>
        /// Load non change data
        /// Non change data will not be edited by the user, but can be edited by the game
        /// </summary>
        private void LoadNonChangeData()
        {
            LoadAirlines();
            LoadAirports();
            LoadManufacturers();
            LoadAircrafts();
        }

        /// <summary>
        /// Load change data
        /// This is data that will be edited by the user
        /// </summary>
        private void LoadChangedata()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loading the airline data from the data files
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadAirlines()
        {
            try
            {
                string JSONString = File.ReadAllText("Assets/AirlineData.json");
                JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
                JsonElement JSONRoot = JSONDoc.RootElement;

                // Create a list to hold the airlines
                var airlines = new List<Airline>();

                foreach (JsonElement airlineData in JSONRoot.EnumerateArray())
                {
                    // Extract the data
                    string name = airlineData.GetProperty("Name").ToString();
                    double cashbalance = Convert.ToDouble(airlineData.GetProperty("Cash Balance").ToString());
                    double reputation = Convert.ToDouble(airlineData.GetProperty("Reputation").ToString());
                    string countrycode = airlineData.GetProperty("CountryCode").ToString();

                    // Check if the country exists
                    if (CountryMap.TryGetValue(countrycode, out var country))
                    {
                        // Create the airline instance
                        Airline airline = new Airline(name, country, cashbalance, reputation);
                        airlines.Add(airline);
                    }
                    else
                    {
                        throw new Exception($"No such country found while creating airline: {name}.");

                    }
                }

                Airlines = new ObservableCollection<Airline>(airlines);
            }
            catch
            {
                throw new Exception("Error while creating airlines.");
            }
        }

        /// <summary>
        /// Loading the airport data from the data files
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadAirports()
        {
            try
            {
                string JSONString = File.ReadAllText("Assets/AirportData.json");
                JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
                JsonElement JSONRoot = JSONDoc.RootElement;

                // Create a list to hold the airports
                var airports = new List<Airport>();

                foreach (JsonElement airportData in JSONRoot.EnumerateArray())
                {
                    // Extract the data
                    string airportName = airportData.GetProperty("Name").ToString();
                    string countryCode = airportData.GetProperty("CountryCode").ToString();
                    string iata = airportData.GetProperty("IATA").ToString();
                    string icao = airportData.GetProperty("ICAO").ToString();
                    double demandFactor = Convert.ToDouble(airportData.GetProperty("demandFactor").ToString());

                    // Extract the coordinates
                    JsonElement _coordinates = airportData.GetProperty("Coordinates");
                    double latitude = Convert.ToDouble(_coordinates.GetProperty("Latitude").ToString());
                    double longitude = Convert.ToDouble(_coordinates.GetProperty("Longitude").ToString());
                    Coordinates coordinates = new Coordinates(latitude, longitude);

                    // Check if the country exists in the map
                    if (CountryMap.TryGetValue(countryCode, out var country))
                    {
                        // Create the Airport instance with the Country reference
                        var airport = new Airport(airportName, iata, icao, country, coordinates, demandFactor);
                        airports.Add(airport);
                    }
                    else
                    {
                        // Handle cases where the country is not found if necessary
                        // For example, you can log a warning or create a default Country instance
                        throw new Exception($"No such country found while creating airport: {airportName}.");
                    }
                }
                // Assign the populated list to the ObservableCollection
                Airports = new ObservableCollection<Airport>(airports);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Loading the manufacturer data from the data files
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadManufacturers()
        {
            string JSONString = File.ReadAllText("Assets/ManufacturerData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement JSONRoot = JSONDoc.RootElement;

            //Create a list to hold the manufacturers
            var manufacturers = new List<Manufacturer>();

            foreach (JsonElement manufacturer in JSONRoot.EnumerateArray())
            {
                //Extract data
                string Name = manufacturer.GetProperty("Name").ToString();
                string Description = manufacturer.GetProperty("Description").ToString();
                string CountryCode = manufacturer.GetProperty("Country").ToString();
                DateTime FoundingDate;
                Country Country;

                if (!manufacturer.GetProperty("FoundingDate").TryGetDateTime(out FoundingDate))
                {
                    throw new Exception($"Failed to convert {manufacturer.GetProperty("FoundingDate").ToString()} into DateTime");
                }

                if (!CountryMap.TryGetValue(CountryCode, out Country))
                {
                    throw new Exception($"Failed to convert {CountryCode} into a country");
                }

                //Add to local list
                manufacturers.Add(new Manufacturer(Name, Description, Country, FoundingDate));

            }

            //Create the observable collection
            Manufacturers = new ObservableCollection<Manufacturer>(manufacturers);
        }

        /// <summary>
        /// Loading the country data from the data files
        /// </summary>
        private void LoadCountries()
        {
            string JSONString = File.ReadAllText("Assets/CountryData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement JSONRoot = JSONDoc.RootElement;

            Countries = new ObservableCollection<Country>();
            RegistrationTemplates = new ObservableCollection<RegistrationTemplate>();

            foreach (JsonElement country in JSONRoot.EnumerateArray())
            {
                //Create the country
                string name = country.GetProperty("Name").ToString();
                string code = country.GetProperty("ISOCode").ToString();
                Continent continent = (Continent)Enum.Parse(typeof(Continent), country.GetProperty("Continent").ToString());

                Country _country = new Country();
                _country.Name = name;
                _country.ISOCode = code;
                _country.Continent = continent;

                Countries.Add(_country);
                _country = Countries.Last();

                //Create the registration template
                if (country.TryGetProperty("Registration", out JsonElement registration))
                {
                    string countryID = registration.GetProperty("CountryIdentifier").ToString();
                    bool separator = Convert.ToBoolean(registration.GetProperty("Separator").ToString());
                    string format = registration.GetProperty("Format").ToString();
                
                    RegistrationTemplates.Add(new RegistrationTemplate(countryID, separator, format, _country));
                }
            }

            // Create a mapping of CountryCode to Country instance
            CountryMap = Countries.ToDictionary(c => c.ISOCode, c => c);

            //Create a mapping of Country to Template
            RegistrationTemplateMap = RegistrationTemplates.ToDictionary(t => t.Country, t => t);
        }

        private AircraftModel LoadAircraftFromJson(JsonElement aircraft, Manufacturer manufacturer)
        {
            //Create a new aircraftModel and set the correct properties
            AircraftModel aircraftModel = new AircraftModel(
                aircraft.GetProperty("Name").ToString(),
                aircraft.GetProperty("Family").ToString(),
                new Price(Convert.ToDouble(aircraft.GetProperty("Price").ToString())),
                (AircraftType)Enum.Parse(typeof(AircraftType), aircraft.GetProperty("AircraftType").ToString()),
                (EngineType)Enum.Parse(typeof(EngineType), aircraft.GetProperty("EngineType").ToString()),
                Convert.ToInt32(aircraft.GetProperty("CruisingSpeed").ToString()),
                Convert.ToInt32(aircraft.GetProperty("maxPax").ToString()),
                Convert.ToInt32(aircraft.GetProperty("maxCargo").ToString()),
                Convert.ToInt32(aircraft.GetProperty("maxRange").ToString()),
                Convert.ToInt32(aircraft.GetProperty("minRunwayLength").ToString()),
                manufacturer,
                Convert.ToDateTime(aircraft.GetProperty("IntroductionDate").ToString()),
                Convert.ToDateTime(aircraft.GetProperty("RetirementDate").ToString()));

            return aircraftModel;
        }

        /// <summary>
        /// Loading the aircraft data from the data files
        /// </summary>
        private void LoadAircrafts()
        {
            string JSONString = File.ReadAllText("Assets/ManufacturerData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement JSONRoot = JSONDoc.RootElement;

            //Create a list to hold the aircrafts
            var aircrafts = new List<AircraftModel>();

            //Loop trough all the manufacturers and process the aircrafts
            foreach (JsonElement manufacturer in JSONRoot.EnumerateArray())
            {
                // Use LINQ to find the manufacturer by name
                Manufacturer Manufacturer = Manufacturers.FirstOrDefault(m => m.Name.Equals(manufacturer.GetProperty("Name").ToString(), StringComparison.OrdinalIgnoreCase));

                // Check if the Aircrafts property exists and is an array
                if (manufacturer.TryGetProperty("Aircrafts", out JsonElement aircraftsElement) && aircraftsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement aircraft in aircraftsElement.EnumerateArray())
                    {

                        if (Convert.ToDateTime(aircraft.GetProperty("IntroductionDate").ToString()) <= GameState.Instance.CurrentDate)
                        {
                            AircraftModel aircraftModel = LoadAircraftFromJson(aircraft, Manufacturer);

                            //Add the model to the aircrafts list
                            aircrafts.Add(aircraftModel);
                        }
                    }
                }

                //End of the manufacturer
            }

            //Create the observable collection
            AircraftModels = new ObservableCollection<AircraftModel>(aircrafts);

        }

        #region Game functions

        public UserData UserData
        {
            get => _userData;
            set
            {
                _userData = value;
                OnPropertyChanged(nameof(UserData));
            }
        }

        /*
        public void SaveGame(string filePath)
        {
            bool wasPlaying = !IsPaused;

            if (wasPlaying)
            {
                PauseCommand.Execute(null);
            }

            // Use GameStateManager to save the game
            GameStateManager.SaveGame(this, filePath);

            if (wasPlaying)
            {
                PlayCommand.Execute(null);
            }
        }

        public void LoadGame(string filePath)
        {
            //Reset the game data
            ResetInstance();

            // Use GameStateManager to load the game
            GameStateManager.LoadGame(this, filePath);
        }*/

        #endregion
    }
}
