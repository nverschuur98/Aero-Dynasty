using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AeroDynasty.Core.Enums;
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

        ////Observable non-change Data
        //public ObservableCollection<Airline> Airlines { get; private set; }
        //public ObservableCollection<Airport> Airports { get; private set; }
        //public ObservableCollection<Manufacturer> Manufacturers { get; private set; }
        //public ObservableCollection<AircraftModel> AircraftModels { get; private set; }
        //
        ////Observable change Data
        //public ObservableCollection<Route> Routes { get; set; }
        //public ObservableCollection<Airliner> Airliners { get; set; }
        
        //Maps
        public Dictionary<string, Country> CountryMap { get; private set; }
        public Dictionary<Country, RegistrationTemplate> RegistrationTemplateMap { get; set; }
        //public Dictionary<int, double> Inflations { get; private set; }
        //
        ////Game Time and state
        //private UserData _userData { get; set; }
        //private DateTime _currentDate;
        //private bool _isPaused;
        //
        ////Commands
        //public ICommand PlayCommand { get; set; }
        //public ICommand PauseCommand { get; set; }
        //public ICommand TestCommand { get; set; }

        //Private Constructor
        private GameData()
        {
            ////Set start date
            //CurrentDate = new DateTime(1946, 1, 1);
            //
            //Load Core first
            LoadCoreData();
            //
            ////Load non-change data
            //LoadNonChangeData();
            //
            ////Load change data
            ////THIS NEEDS TO MOVE UNTILL AFTER THE CTOR IS FULLY INITIALIZED
            //LoadAirliners();
            //LoadRoutes();
            //
            ////Init start date
            //Airline arl = Airlines.Where(al => al.Name.Contains("KLM")).FirstOrDefault();
            //
            //UserData = new UserData(arl);
            //
            //IsPaused = true;
            //
            //
            ////Bind commands
            //PlayCommand = new RelayCommand(PlayGame);
            //PauseCommand = new RelayCommand(PauseGame);
            //TestCommand = new RelayCommand(LoadTestData);
        }

        private void LoadCoreData()
        {
            LoadCountries();
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
    }
}
