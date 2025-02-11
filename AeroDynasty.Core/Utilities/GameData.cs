using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using static AeroDynasty.Core.Utilities.SaveGameManager;

namespace AeroDynasty.Core.Utilities
{
    public class GameData : _BaseViewModel
    {
        //Singleton Instance
        private static GameData _instance;
        public static GameData Instance => _instance ?? (_instance = new GameData());

        //Observable Core Data
        public ObservableCollection<Area> Areas { get; private set; }
        public ObservableCollection<Country> Countries { get; private set; }
        public ObservableCollection<RegistrationTemplate> RegistrationTemplates { get; set; }

        //Observable non-change Data
        public GlobalModifiers GlobalModifiers { get; private set; }
        public ObservableCollection<Airline> Airlines { get; private set; }
        public ObservableCollection<Airport> Airports { get; private set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; private set; }
        public ObservableCollection<AircraftModel> AircraftModels { get; private set; }
        public List<RouteDemand> RouteDemands { get; set; }
        
        //Observable change Data
        public ObservableCollection<Route> Routes { get; set; }
        public ObservableCollection<Airliner> Airliners { get; set; }
        
        //Maps
        public Dictionary<Area, Country> AreaCountryMap { get; private set; }
        public Dictionary<string, Country> CountryMap { get; private set; }
        public Dictionary<Country, RegistrationTemplate> RegistrationTemplateMap { get; set; }
        //public Dictionary<int, double> Inflations { get; private set; }
        
        // Userdata
        private UserData _userData { get; set; }
        public UserData UserData
        {
            get => _userData;
            set
            {
                _userData = value;
                OnPropertyChanged(nameof(UserData));
            }
        }

        //Commands

        //Private Constructor
        private GameData()
        {
            // Setup the game data
            SetupGameData();
            
            // Init airline
            //Airline arl = Airlines.Where(al => al.Name.Contains("KLM")).FirstOrDefault();
            UserData = new UserData();

            // Execute tasks for starting values
            //GameLoadedTasks();

            // Register all game tasks
            RegisterGameTasks();

            // First time route demand calculations
            //GameTasks.CalculateRouteBaseDemand();
        }

        //Private funcs

        /// <summary>
        /// Load all core data
        /// Core data will never change during any game
        /// </summary>
        private void LoadCoreData()
        {
            LoadAreas();
            LoadCountries();
            SetAreaCountryMap();
            LoadGlobalModifiers();
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
            RouteDemands = new List<RouteDemand>();
        }

        /// <summary>
        /// Load change data
        /// This is data that will be edited by the user
        /// </summary>
        private void LoadChangedata()
        {
            LoadAirliners();
            LoadRoutes();
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
                        Airline airline = new Airline(name, country, new Price(cashbalance), reputation);
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
#if DEBUG
            // Start the stopwatch to time the tasks
            var stopwatch = Stopwatch.StartNew();
#endif

            string JSONString = File.ReadAllText("Assets/AirportData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement JSONRoot = JSONDoc.RootElement;

            // Create a list to hold the airports
            var airports = new List<Airport>();

            foreach (JsonElement airportData in JSONRoot.EnumerateArray())
            {
                // Extract the data
                string airportName = airportData.GetProperty("Name").ToString();
                int area = airportData.GetProperty("Area").GetInt32();
                string iata = airportData.GetProperty("IATA").ToString();
                string icao = airportData.GetProperty("ICAO").ToString();
                string town = airportData.GetProperty("Town").ToString();
                AirportType airportType = (AirportType)Enum.Parse(typeof(AirportType), airportData.GetProperty("Type").ToString());
                FocusSeason airportSeason = (FocusSeason)Enum.Parse(typeof(FocusSeason), airportData.GetProperty("Season").ToString());

                // Get size property
                JsonElement size = airportData.GetProperty("Size");
                AirportSize passengerSize = (AirportSize)Enum.Parse(typeof(AirportSize), size.GetProperty("PassengerSize").ToString());

                DateTime startDate;
                DateTime endDate;

                // Extract the period if it exists
                if (airportData.TryGetProperty("Period", out JsonElement period))
                {
                    // Parse the "From" and "To" dates
                    startDate = period.TryGetProperty("From", out var fromProperty) && DateTime.TryParseExact(fromProperty.GetString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var sdate)
                        ? sdate
                        : new DateTime(1900, 01, 01);

                    endDate = period.TryGetProperty("To", out var toProperty) && DateTime.TryParseExact(toProperty.GetString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var edate)
                        ? edate
                        : new DateTime(2199, 12, 31);
                }
                else
                {
                    // If "Period" does not exist, use default dates
                    startDate = new DateTime(1900, 01, 01);
                    endDate = new DateTime(2199, 12, 31);
                }

                // Extract the coordinates
                JsonElement _coordinates = airportData.GetProperty("Coordinates");
                double latitude = _coordinates.GetProperty("Latitude").GetDouble();  // Correctly get double value
                double longitude = _coordinates.GetProperty("Longitude").GetDouble();  // Correctly get double value

                Coordinates coordinates = new Coordinates(latitude, longitude);

                // Get runways property
                List<Runway> runways = new List<Runway>();
                if(airportData.TryGetProperty("Runways", out JsonElement _runways))
                {
                    foreach(JsonElement runway in _runways.EnumerateArray())
                    {
                        string name = runway.GetProperty("Name").GetString();
                        int length = runway.GetProperty("Length").GetInt32();
                        RunwaySurface surface = (RunwaySurface)Enum.Parse(typeof(RunwaySurface), runway.GetProperty("Surface").ToString());

                        runways.Add(new Runway(name, surface, length));
                    }
                }

                // Check if the country exists in the map
                if (AreaCountryMap.TryGetValue(Areas.First(a => a.ID == area), out var country))
                {
                    // Create the Airport instance with the Country reference
                    var airport = new Airport(airportName, iata, icao, airportType, passengerSize, airportSeason, country, coordinates, town, runways);

                    // Set the period for the airport
                    airport.SetPeriod(startDate, endDate);

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
#if DEBUG
            // Stop the stopwatch after all tasks are completed
            stopwatch.Stop();

            // Log or display the time it took to complete the tasks
            Console.WriteLine($"Loading Airports completed in {stopwatch.ElapsedMilliseconds} ms");
#endif
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
        /// Loading the area data from the data files
        /// </summary>
        private void LoadAreas()
        {
            string JSONString = File.ReadAllText("Assets/AreaData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement JSONRoot = JSONDoc.RootElement;

            Areas = new ObservableCollection<Area>();

            foreach (JsonElement area in JSONRoot.EnumerateArray())
            {
                int id = area.GetProperty("Id").GetInt32();
                string name = area.GetProperty("Name").GetString();

                Areas.Add(new Area(id, name));
            }
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

                Country _country = new Country();
                _country.Name = name;
                _country.ISOCode = code;

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

        /// <summary>
        /// Set the AreaCountry mapping based on initial setting
        /// </summary>
        private void SetAreaCountryMap()
        {
            SetAreaCountryMap(new DateTime(1900, 1, 1));
        }

        /// <summary>
        /// Set the AreaCountry mapping based on date
        /// </summary>
        private void SetAreaCountryMap(DateTime date)
        {
            // Set dictionary
            AreaCountryMap = new Dictionary<Area, Country>();

            foreach (Area area in Areas)
            {
                Country country = Countries.First();
                AreaCountryMap.Add(area, country);
            }
        }

        /// <summary>
        /// Loading the global modifiers
        /// </summary>
        private void LoadGlobalModifiers()
        {
            // Initiate the global modifier object
            GlobalModifiers = new GlobalModifiers();

            // Select json file
            string JSONString = File.ReadAllText("Assets/GlobalData.json");
            JsonDocument JSONDoc = JsonDocument.Parse(JSONString);
            JsonElement root = JSONDoc.RootElement;

            // Private function for reading all year values to a dictionary
            void readYearValue(JsonElement element, Dictionary<int, double> dictionary)
            {
                foreach (JsonProperty yearElement in element.EnumerateObject())
                {
                    // Convert the property name to an integer (year)
                    int year = int.Parse(yearElement.Name);

                    // Get the value as a double
                    double value = yearElement.Value.GetDouble();

                    // Example: Do something with the year and price
                    dictionary.Add(year, value);
                }
            }

            // Access the "FuelPrices" object
            GlobalModifiers.FuelPriceMap = new Dictionary<int, double>();
            JsonElement fuelPrices = root.GetProperty("FuelPrices");
            readYearValue(fuelPrices, GlobalModifiers.FuelPriceMap);

            // Access the "GlobalPassengers" object
            GlobalModifiers.GlobalPassengersMap = new Dictionary<int, double>();
            JsonElement globalPassengers = root.GetProperty("GlobalPassengers");
            readYearValue(globalPassengers, GlobalModifiers.GlobalPassengersMap);

            GlobalModifiers.CurrentFuelPrice = new Price(0);
            GlobalModifiers.CurrentGlobalPassengers = 0;
            //GlobalModifiers.CurrentFuelPrice = new Price(GlobalModifiers.FuelPriceMap[y]);
            //GlobalModifiers.CurrentGlobalPassengers = GlobalModifiers.GlobalPassengersMap[y];
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
                aircraft.GetProperty("FuelConsumptionRate").GetDouble(),
                aircraft.GetProperty("OperatingCostRate").GetDouble(),
                manufacturer);

            DateTime startDate;
            DateTime endDate;

            // Extract the period if it exists
            if (aircraft.TryGetProperty("Period", out JsonElement period))
            {
                // Parse the "From" and "To" dates
                startDate = period.TryGetProperty("From", out var fromProperty) && DateTime.TryParseExact(fromProperty.GetString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var sdate)
                    ? sdate
                    : new DateTime(1900, 01, 01);

                endDate = period.TryGetProperty("To", out var toProperty) && DateTime.TryParseExact(toProperty.GetString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var edate)
                    ? edate
                    : new DateTime(2199, 12, 31);
            }
            else
            {
                // If "Period" does not exist, use default dates
                startDate = new DateTime(1900, 01, 01);
                endDate = new DateTime(2199, 12, 31);
            }

            //Set the period in which the aircraft is active.
            aircraftModel.SetPeriod(startDate, endDate);

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

                        AircraftModel aircraftModel = LoadAircraftFromJson(aircraft, Manufacturer);

                        //Add the model to the aircrafts list
                        aircrafts.Add(aircraftModel);
                    }
                }

                //End of the manufacturer
            }

            //Create the observable collection
            AircraftModels = new ObservableCollection<AircraftModel>(aircrafts);

        }

        /// <summary>
        /// Loading the airliner data
        /// </summary>
        private void LoadAirliners()
        {
            Airliners = new ObservableCollection<Airliner>();
        }

        /// <summary>
        /// Loading the route data
        /// </summary>
        private void LoadRoutes()
        {
            Routes = new ObservableCollection<Route>();
        }

        /// <summary>
        /// Register all the game tasks that need to be execuded
        /// </summary>
        private void RegisterGameTasks()
        {
            //GameState.Instance.RegisterDailyTask(GameTasks.TestTask);
            GameState.Instance.RegisterDailyTask(GameTasks.CalculateRouteExecutions);
            GameState.Instance.RegisterDailyTask(GameTasks.CalculateFuelPrice);
            GameState.Instance.RegisterDailyTask(GameTasks.CheckIsActive);

            GameState.Instance.RegisterMonthlyTask(GameTasks.CalculateRouteDemand);
        }

        // Public funcs
        public void SetupGameData()
        {
            // Load Core first
            LoadCoreData();

            // Load non-change data
            LoadNonChangeData();

            // Load change data
            LoadChangedata();
        }

        public void GameLoadedTasks()
        {
            // Get game data 
            int y = GameState.Instance.CurrentDate.Year;
            GlobalModifiers.CurrentFuelPrice = new Price(GlobalModifiers.FuelPriceMap[y]);
            GlobalModifiers.CurrentGlobalPassengers = GlobalModifiers.GlobalPassengersMap[y];

            // Do one of calculations
            GameTasks.CalculateFuelPrice();
            GameTasks.CheckIsActive();
            GameTasks.CalculateRouteDemand(true);
        }
    }
}
