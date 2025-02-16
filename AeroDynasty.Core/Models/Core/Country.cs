using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AeroDynasty.Core.Enums;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.Core.Models.Core
{
    public class Country
    {
        public bool IsActive
        {
            get
            {
                return GameData.Instance.AreaCountryMap.Values.Contains(this);
            }
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string ISO2Code { get; set; }
        public string ISO3Code { get; set; }
        public int Region { get; set; }
         
        public string FlagPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Flags", $"{ISO2Code}.png");
            }
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <returns></returns>
        public static ICollectionView GetCountries()
        {
            return GetCountries(_ => true); // Calls the overloaded method with no extra filter
        }

        /// <summary>
        /// Return all active airports in the game as an ICollectionView
        /// </summary>
        /// <param name="additionalFilter">Selecting criteria</param>
        /// <returns></returns>
        public static ICollectionView GetCountries(Func<Country, bool> additionalFilter)
        {
            var filteredCountries = GameData.Instance.Countries.Where(a => a.IsActive).Where(additionalFilter);
            var countryView = CollectionViewSource.GetDefaultView(filteredCountries);
            countryView.SortDescriptions.Add(new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            return countryView;
        }
    }
}
