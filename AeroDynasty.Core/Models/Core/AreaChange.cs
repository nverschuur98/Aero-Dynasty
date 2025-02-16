using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class AreaChange
    {
        public Area Area { get; private set; }
        public Country Country { get; private set; }
        public DateTime ChangeDate { get; private set; }
        public bool IgnoreWar { get; private set; }

        public AreaChange(Area area, Country country, DateTime changedate, bool ignoreWar)
        {
            Area = area;
            Country = country;
            ChangeDate = changedate;
            IgnoreWar = ignoreWar;
        }
    }

    public static class AreaChangeManager
    {

        // Adds an area to the dictionary with a country if it's not already in the dictionary
        public static void AssignAreaToCountry(Area area, Country country)
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

        
    }
}
