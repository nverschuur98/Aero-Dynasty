using AeroDynasty.Core.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.AirportModels
{
    /// <summary>
    /// Base airport expansion class
    /// </summary>
    public abstract class AirportExpansion
    {
        public DateTime Date { get; set; }
        public AirportExpansionType Type { get; set; }

        protected AirportExpansion(DateTime date, AirportExpansionType type)
        {
            Date = date;
            Type = type;
        }

        public abstract void Execute(Airport airport);

    }

    /// <summary>
    /// Static class for execution of airportExpansions
    /// </summary>
    public static class AirportExtensions
    {
        // Method to execute expansions for all airports based on the current date
        public static async Task CheckExpansionsForAllAsync(this ObservableCollection<Airport> airports, DateTime currentDate)
        {
            await Task.WhenAll(airports.Select(async airport =>
            {
                // Check if expansions list is set
                if(airport.Expansions != null)
                    await airport.ExecuteExpansionsAsync(currentDate);
            }));
        }
    }

    /// <summary>
    /// Change airport name
    /// </summary>
    public class ChangeAirportNameExpansion : AirportExpansion
    {
        public string NewName { get; private set; }

        public ChangeAirportNameExpansion(string newName, DateTime date) : base(date, AirportExpansionType.ChangeAirportName)
        {
            NewName = newName;
        }

        public override void Execute(Airport airport)
        {
            // Change the airport name
            airport.Name = NewName;
        }
    }

    /// <summary>
    /// Change airport type
    /// </summary>
    public class ChangeAirportTypeExpansion : AirportExpansion
    {
        public AirportType Type { get; private set; }

        public ChangeAirportTypeExpansion(AirportType type, DateTime date) : base(date, AirportExpansionType.ChangeAirportType)
        {
            Type = type;
        }

        public override void Execute(Airport airport)
        {
            // Change the airport type
            airport.Type = Type;
        }
    }

    /// <summary>
    /// Change airport name
    /// </summary>
    public class ChangeAirportTownExpansion : AirportExpansion
    {
        public string Town { get; private set; }

        public ChangeAirportTownExpansion(string town, DateTime date) : base(date, AirportExpansionType.ChangeAirportTown)
        {
            Town = town;
        }

        public override void Execute(Airport airport)
        {
            // Change the airport town
            airport.Town = Town;
        }
    }

    /// <summary>
    /// New runway expansion
    /// </summary>
    public class NewRunwayExpansion : AirportExpansion
    {
        public Runway Runway { get; private set; }

        public NewRunwayExpansion(string name, int length, RunwaySurface surface, DateTime date) : base(date, AirportExpansionType.NewRunway)
        {
            Runway = new Runway(name, surface, length);
        }

        public override void Execute(Airport airport)
        {
            // Create and add the new runway
            airport.Runways.Add(Runway);
        }
    }

    /// <summary>
    /// Change runway expansion
    /// </summary>
    public class ChangeRunwayExpansion : AirportExpansion
    {
        public string Name { get; private set; }
        public RunwaySurface Surface { get; private set; }
        public int Length { get; private set; }

        public ChangeRunwayExpansion(string name, int length, RunwaySurface surface, DateTime date) : base(date, AirportExpansionType.ChangeRunway)
        {
            Name = name;
            Surface = surface;
            Length = length;
        }

        public override void Execute(Airport airport)
        {
            Runway runway = airport.Runways.FirstOrDefault(r => r.Name == Name);

            // Check if runway exists
            if (runway == null)
                throw new ApplicationException($"Expansion Failed: Runway {Name} not found on airport {airport.ICAO_Name}");

            // Modify runway
            runway.Length = Length;
            runway.Surface = Surface;
        }
    }

    /// <summary>
    /// Change runway name expansion
    /// </summary>
    public class ChangeRunwayNameExpansion : AirportExpansion
    {
        public string Name { get; private set; }
        public string NewName { get; private set; }

        public ChangeRunwayNameExpansion(string name, string newName, DateTime date) : base(date, AirportExpansionType.NewRunway)
        {
            Name = name;
            NewName = newName;
        }

        public override void Execute(Airport airport)
        {
            Runway runway = airport.Runways.FirstOrDefault(r => r.Name == Name);

            // Check if runway exists
            if (runway == null)
                throw new ApplicationException($"Expansion Failed: Runway {Name} not found on airport {airport.ICAO_Name}");

            // Modify runway
            runway.Name = NewName;
        }
    }

    /// <summary>
    /// Close runway expansion
    /// </summary>
    public class CloseRunwayExpansion : AirportExpansion
    {
        public string Name { get; private set; }

        public CloseRunwayExpansion(string name, DateTime date) : base(date, AirportExpansionType.CloseRunway)
        {
            Name = name;
        }

        public override void Execute(Airport airport)
        {
            Runway runway = airport.Runways.FirstOrDefault(r => r.Name == Name);

            // Check if runway exists
            if (runway == null)
                throw new ApplicationException($"Expansion Failed: Runway {Name} not found on airport {airport.ICAO_Name}");

            airport.Runways.Remove(runway);
        }
    }
}
