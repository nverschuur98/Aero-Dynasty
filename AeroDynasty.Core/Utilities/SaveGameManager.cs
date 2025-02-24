using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.AirlinerModels;
using AeroDynasty.Core.Models.AirportModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Models.RouteModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AeroDynasty.Core.Utilities.SaveGameManager;

namespace AeroDynasty.Core.Utilities
{
    public class SaveGameManager
    {

        public class GameDataHolder
        {
            //Game Data 
            public UserData UserData { get; set; }
            public GlobalModifiers GlobalModifiers { get; set; }
            public List<Airline> Airlines { get; set; }
            public List<Airliner> Airliners { get; set; }
            public List<Route> Routes { get; set; }

            public void LoadFromGame()
            {
                UserData = GameData.Instance.UserData;
                GlobalModifiers = GameData.Instance.GlobalModifiers;
                Airlines = GameData.Instance.Airlines.ToList();
                Airliners = GameData.Instance.Airliners.ToList();
                Routes = GameData.Instance.Routes.ToList();
            }

        }

        public class GameStateHolder
        {
            //Game Data 
            public GameState State { get; set; }

            public void LoadFromGame()
            {
                State = GameState.Instance;
            }

        }

        public static void NewGame(Airline userAirline, int startYear)
        {

            GameState.Instance.SetupGameState();
            GameState.Instance.CurrentDate = new DateTime(startYear, 1, 1);

            GameData.Instance.SetupGameData();
            GameData.Instance.UserData.Airline = GameData.Instance.Airlines.First(a => a.Name == userAirline.Name);

            GameState.Instance.PauseCommand.Execute(null);

            // Check all the objects with an assigned period on their status
            GameData.Instance.GameLoadedTasks();
        }

        /// <summary>
        /// Save the current game state to a JSON file
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void SaveGame(string filePath)
        {
            // First check if the game is running
            bool wasPaused = GameState.Instance.IsPaused;

            if (!wasPaused)
            {
                GameState.Instance.PauseCommand.Execute(null);
            }

            // Save the game data
            GameDataHolder data = new GameDataHolder();
            data.LoadFromGame();
            GameStateHolder state = new GameStateHolder();
            state.LoadFromGame();

            string json;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    
                    new AirlinerConverter(),
                    new CountryConverter(),
                    new AirportConverter(),
                    new RouteConverter(),
                    new RouteScheduleConverter(),
                    new RouteLegConverter(),
                    new AirlineConverter(),
                    new UserDataConverter(),
                    new GlobalModifiersConverter(),
                    new GameStateConverter()
                }
            };

            try
            {

                // Combine state and data into a single wrapper object
                var saveData = new
                {
                    GameState = state,
                    GameData = data
                };

                // Serialize the wrapper object
                json = JsonSerializer.Serialize(saveData, options);

                // Ensure the directory for the file exists
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Write the JSON content to the file
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error while creating save game: {ex.Message}");
            }

            // Restart the game if it was running
            if (!wasPaused)
            {
                GameState.Instance.PlayCommand.Execute(null);
            }
        }

        /// <summary>
        /// Load the game state from a JSON file
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async void LoadGame(string filePath)
        {
            // First check if the game is running
            bool wasPaused = GameState.Instance.IsPaused;

            if (!wasPaused)
            {
                GameState.Instance.PauseCommand.Execute(null);
            }

            // Before loading all the gamedata, reset all the Game to the initial state
            GameData.Instance.SetupGameData();
            GameState.Instance.SetupGameState();

            // Load the game data
            try
            {
                // Read the JSON from the file
                string json = File.ReadAllText(filePath);

                // Deserialize the data into the GameDataHolder
                var options = new JsonSerializerOptions
                {
                    Converters =
                        {
                            new AirlinerConverter(),
                            //new CountryConverter(),
                            //new AirportConverter(),
                            new RouteConverter(),
                            new RouteLegConverter(),
                            new AirlineConverter(),
                            new UserDataConverter(),
                            new GlobalModifiersConverter(),
                            new GameStateConverter()
                        }
                };

                // Parse the JSON document
                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;
                JsonElement state = root.GetProperty("GameState");
                JsonElement data = root.GetProperty("GameData");

                // Deserialize each JSON element separately using the corresponding converter
                var gameState = JsonSerializer.Deserialize<GameState>(state.GetProperty("State").GetRawText(), options);
                var airlines = JsonSerializer.Deserialize<List<Airline>>(data.GetProperty("Airlines").GetRawText(), options);
                var userData = JsonSerializer.Deserialize<UserData>(data.GetProperty("UserData").GetRawText(), options);
                var globalModifiers = JsonSerializer.Deserialize<GlobalModifiers>(data.GetProperty("GlobalModifiers").GetRawText(), options);

                // Load the data back into the game state
                // As the data is directly injected in the GameDataInstance, no loading is needed

                // Check all the objects with an assigned period on their status
                await GameData.Instance.GameLoadedTasks();

                // After the data is set and checked with GameLoadedTasks, load the remaining change data
                var airliners = JsonSerializer.Deserialize<List<Airliner>>(data.GetProperty("Airliners").GetRawText(), options);
                var routes = JsonSerializer.Deserialize<List<Route>>(data.GetProperty("Routes").GetRawText(), options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while loading the save game: {ex.Message}");
            }

        }
    }

    #region Custom Serializers
    public class GameStateConverter : JsonConverter<GameState>
    {
        public override GameState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var currentDate = Convert.ToDateTime(root.GetProperty("CurrentDate").GetString());

                // Assign value to GameState singleton
                GameState.Instance.CurrentDate = currentDate;

                // Return dummy value
                return GameState.Instance;
            }
        }

        public override void Write(Utf8JsonWriter writer, GameState value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("CurrentDate", value.CurrentDate);
            writer.WriteEndObject();
        }
    }

    public class AirlineConverter : JsonConverter<Airline>
    {
        public override Airline Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var name = root.GetProperty("Name").GetString();
                var reputation = Convert.ToDouble(root.GetProperty("Reputation").GetString());
                var cashBalance = root.GetProperty("CashBalance").GetDouble();

                // Get the airline reference from the GameData singleton
                Airline airline = GameData.Instance.Airlines.FirstOrDefault(a => a.Name == name);
                if (airline == null)
                {
                    // Optionally, handle the case where the airline does not exist in the GameData
                    // For example, you could create a new airline or throw an exception
                    throw new InvalidOperationException($"Airline with name '{name}' not found in GameData.");
                }

                // Update the airline properties
                airline.Reputation = reputation;
                airline.CashBalance = new Price(cashBalance);

                // Ensure that this airline is added to the GameData singleton
                //GameData.Instance.Airlines.Add(airline);

                return airline;
            }
        }

        public override void Write(Utf8JsonWriter writer, Airline value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Name", value.Name);
            writer.WriteString("Reputation", value.Reputation.ToString());
            writer.WriteNumber("CashBalance", value.CashBalance.Amount);
            writer.WriteEndObject();
        }
    }

    public class AirlinerConverter : JsonConverter<Airliner>
    {
        public override Airliner Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var registrationNumber = Convert.ToInt32(root.GetProperty("RegistrationNumber").GetString());
                var modelName = root.GetProperty("Model").GetString();
                var ownerName = root.GetProperty("Owner").GetString();
                var productionDate = Convert.ToDateTime(root.GetProperty("ProductionDate").GetString());

                // Get the model from the GameData singleton
                AircraftModel model = GameData.Instance.AircraftModels.FirstOrDefault(m => m.Name == modelName);
                if (model == null)
                {
                    throw new InvalidOperationException($"Aircraft model '{modelName}' not found in GameData.");
                }

                // Get the airline from the GameData singleton
                Airline owner = GameData.Instance.Airlines.FirstOrDefault(a => a.Name == ownerName);
                if (owner == null)
                {
                    throw new InvalidOperationException($"Airline '{ownerName}' not found in GameData.");
                }

                // Create the registration
                Registration registration = new Registration(owner.Country, registrationNumber);

                // Create the Airliner object
                var airliner = new Airliner(registration, model, owner, productionDate);

                // Add the Airliner to the GameData singleton
                GameData.Instance.Airliners.Add(airliner);

                // Return the Airliner object as required by the JsonConverter
                return airliner;
            }
        }

        public override void Write(Utf8JsonWriter writer, Airliner value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("RegistrationNumber", value.Registration.Number.ToString());
            writer.WriteString("Model", value.Model.Name);
            writer.WriteString("Owner", value.Owner.Name);
            writer.WriteString("ProductionDate", value.ProductionDate.ToString("yyyy-MM-dd"));
            writer.WriteEndObject();
        }
    }

    public class CountryConverter : JsonConverter<Country>
    {
        //public string Name { get; set; }
        //public string ISOCode { get; set; }
        //public Continent Continent { get; set; }

        public override Country Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Country value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("ISOCode", value.ISO2Code.ToString());
            writer.WriteEndObject();
        }
    }

    public class AirportConverter : JsonConverter<Airport>
    {
        //public string Name { get; set; }
        //public string IATA { get; set; }
        //public string ICAO { get; set; }
        //public Country Country { get; set; }
        //public Coordinates Coordinates { get; set; }
        //public double DemandFactor { get; set; }

        public override Airport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Airport value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("ICAO", value.ICAO.ToString());
            writer.WriteEndObject();
        }
    }

    public class RouteConverter : JsonConverter<Route>
    {
        public override Route Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var originICAO = root.GetProperty("Origin_ICAO").GetString();
                var destinationICAO = root.GetProperty("Destination_ICAO").GetString();
                var ownerName = root.GetProperty("Owner").GetString();
                var ticketPrice = Convert.ToDouble(root.GetProperty("TicketPrice").GetString());
                var assignedAirliners = root.GetProperty("AssignedAirliners").EnumerateArray();
                var scheduledFlights = root.GetProperty("ScheduledFlights");

                // Ensure the referenced objects exist in GameData
                Airport origin = GameData.Instance.Airports.FirstOrDefault(a => a.ICAO == originICAO && a.IsActive);
                Airport destination = GameData.Instance.Airports.FirstOrDefault(a => a.ICAO == destinationICAO && a.IsActive);
                Airline owner = GameData.Instance.Airlines.FirstOrDefault(a => a.Name == ownerName);

                if (origin == null || destination == null || owner == null)
                {
                    throw new InvalidOperationException($"Referenced objects not found in GameData. Origin: {origin.Name}, Destination: {destination.Name}, Owner: {owner.Name}");
                }

                // Create a new JsonSerializerOptions with the RouteScheduleConverter that uses the context
                var scheduleOptions = new JsonSerializerOptions(options);
                scheduleOptions.Converters.Add(new RouteScheduleConverter(owner));

                var flights = JsonSerializer.Deserialize<ObservableCollection<RouteSchedule>>(scheduledFlights.GetRawText(), scheduleOptions);

                // Create the route object
                var route = new Route(origin, destination, owner, new Price(ticketPrice));

                // Add assigned airliners to route
                foreach(var value in assignedAirliners)
                {
                    route.AssignAirliner(GameData.Instance.Airliners.First(air => air.Registration.ReturnValue == value.GetString()));
                }

                // Add schedules to route
                foreach(RouteSchedule flight in flights)
                {
                    try
                    {
                        route.AddSchedule(flight);
                    }catch (ApplicationException ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }

                // Add the route to the GameData singleton
                GameData.Instance.Routes.Add(route);

                // Return the route (as required by the JsonConverter signature)
                return route;
            }
        }

        public override void Write(Utf8JsonWriter writer, Route value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Origin_ICAO", value.Origin.ICAO);
            writer.WriteString("Destination_ICAO", value.Destination.ICAO);
            writer.WriteString("Owner", value.Owner.Name);
            writer.WriteString("TicketPrice", value.TicketPrice.Amount.ToString());

            // Write all assigned airliners
            writer.WriteStartArray("AssignedAirliners");
            foreach(Airliner airliner in value.AssignedAirliners)
            {
                writer.WriteStringValue(airliner.Registration.ReturnValue);
            }
            writer.WriteEndArray();

            // Serialize ScheduledFlights using the RouteScheduleConverter
            writer.WritePropertyName("ScheduledFlights");
            JsonSerializer.Serialize(writer, value.ScheduledFlights, options);
            writer.WriteEndObject();
        }
    }

    public class RouteScheduleConverter : JsonConverter<RouteSchedule>
    {
        private readonly Airline _owner;

        public RouteScheduleConverter() { }
        public RouteScheduleConverter(Airline owner)
        {
            _owner = owner;
        }

        public override RouteSchedule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                // Create a new JsonSerializerOptions with the RouteScheduleConverter that uses the context
                var legOptions = new JsonSerializerOptions(options);
                legOptions.Converters.Add(new RouteLegConverter(_owner));

                var root = doc.RootElement;

                var outbound = JsonSerializer.Deserialize<RouteLeg>(root.GetProperty("Outbound").GetRawText(), legOptions);
                var turnaroundTime = TimeSpan.Parse(root.GetProperty("TurnaroundTime").GetString());

                return new RouteSchedule(outbound.Origin, outbound.Destination, outbound.DepartureDay, outbound.DepartureTime, outbound.AssignedAirliner);
            }
        }

        public override void Write(Utf8JsonWriter writer, RouteSchedule value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            // Serialize RouteLegs using the RouteLegConverter
            writer.WritePropertyName("Outbound");
            JsonSerializer.Serialize(writer, value.Outbound, options);
            writer.WriteString("TurnaroundTime", value.TurnaroundTime.ToString());
            writer.WriteEndObject();
        }
    }

    public class RouteLegConverter : JsonConverter<RouteLeg>
    {
        private readonly Airline _owner;

        public RouteLegConverter() { }
        public RouteLegConverter(Airline owner)
        {
            _owner = owner;
        }

        public override RouteLeg Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                Airport origin = GameData.Instance.Airports.First(a => a.ICAO == root.GetProperty("Origin_ICAO").ToString());
                Airport destination = GameData.Instance.Airports.First(a => a.ICAO == root.GetProperty("Destination_ICAO").ToString());

                Airliner airliner = GameData.Instance.Airliners.First(air => air.Registration.ReturnValue == root.GetProperty("Assigned_Airliner").ToString());
                DayOfWeek departureDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), root.GetProperty("Departure_Day").GetString());
                var departureTime = TimeSpan.Parse(root.GetProperty("Departure_Time").GetString());

                return new RouteLeg(departureTime, departureDay, airliner, origin, destination);
            }
        }

        public override void Write(Utf8JsonWriter writer, RouteLeg value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Origin_ICAO", value.Origin.ICAO);
            writer.WriteString("Destination_ICAO", value.Destination.ICAO);
            writer.WriteString("Departure_Day", value.DepartureDay.ToString());
            writer.WriteString("Departure_Time", value.DepartureTime.ToString());
            writer.WriteString("Assigned_Airliner", value.AssignedAirliner.Registration.ReturnValue);
            writer.WriteEndObject();
        }
    }

    public class UserDataConverter : JsonConverter<UserData>
    {
        public override UserData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var airlineName = root.GetProperty("Airline").GetString();

                // Get the airline reference from the GameData singleton
                GameData.Instance.UserData.Airline = GameData.Instance.Airlines.FirstOrDefault(a => a.Name == airlineName);


                // Return the deserialized UserData object (as required by the JsonConverter signature)
                return GameData.Instance.UserData;
            }
        }

        public override void Write(Utf8JsonWriter writer, UserData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Airline", value.Airline.Name);
            writer.WriteEndObject();
        }
    }

    public class GlobalModifiersConverter : JsonConverter<GlobalModifiers>
    {
        public override GlobalModifiers Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                var currentFuelPrice = root.GetProperty("CurrentFuelPrice").GetDouble();

                // Set the currentfuelPrice
                GameData.Instance.GlobalModifiers.CurrentFuelPrice.Amount = currentFuelPrice;

                // Return the deserialized UserData object (as required by the JsonConverter signature)
                return GameData.Instance.GlobalModifiers;
            }
        }

        public override void Write(Utf8JsonWriter writer, GlobalModifiers value, JsonSerializerOptions options)
        {
            writer.WriteStartObject(); 
            writer.WriteNumber("CurrentFuelPrice", Math.Round(value.CurrentFuelPrice.Amount, 3));
            writer.WriteEndObject();
        }
    }

    #endregion
}
