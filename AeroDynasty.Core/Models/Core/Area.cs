using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class Area
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Area(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    public class AreaCountryManager
    {

        // Adds an area to the dictionary with a country if it's not already in the dictionary
        public void AssignAreaToCountry(Area area, Country country)
        {
            if (!GameData.Instance.AreaCountryMap.ContainsKey(area))
            {
                // If the area is not already in the dictionary, add it.
                GameData.Instance.AreaCountryMap.Add(area, country);
            }
            else
            {
                // If the area is already in the dictionary, simply update the country.
                GameData.Instance.AreaCountryMap[area] = country;
            }
        }

        // Returns the country an area is currently assigned to
        public Country GetCountryByArea(Area area)
        {
            if (GameData.Instance.AreaCountryMap.ContainsKey(area))
            {
                return GameData.Instance.AreaCountryMap[area];
            }
            return null; // Or return a default country if preferred
        }
    }
}
