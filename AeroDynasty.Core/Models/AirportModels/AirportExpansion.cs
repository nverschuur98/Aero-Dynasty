using AeroDynasty.Core.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.AirportModels
{
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

    public static class AirportExtensions
    {
        // Method to execute expansions for all airports based on the current date
        public static async Task CheckExpansionsForAllAsync(this ObservableCollection<Airport> airports, DateTime currentDate)
        {
            await Task.WhenAll(airports.Select(async airport =>
            {
                await airport.ExecuteExpansionsAsync(currentDate);
            }));
        }
    }

    public class NewRunwayExpansion : AirportExpansion
    {
        public Runway Runway { get; private set; }

        public NewRunwayExpansion(string name, int length, RunwaySurface surface , DateTime date) : base(date, AirportExpansionType.NewRunway)
        {
            Runway = new Runway(name, surface, length);
        }

        public override void Execute(Airport airport)
        {
            // Create and add the new runway
            airport.Runways.Add(Runway);
        }
    }
}
