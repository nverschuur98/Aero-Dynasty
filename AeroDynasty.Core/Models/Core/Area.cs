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

        public Country GetCountry()
        {
            if (GameData.Instance.AreaCountryMap.ContainsKey(this))
            {
                return GameData.Instance.AreaCountryMap[this];
            }
            return null; // Or return a default country if preferred
        }
    }
}
