using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Enums;

namespace AeroDynasty.Core.Models.Core
{
    public class Country
    {
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
    }
}
