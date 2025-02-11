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
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string FlagPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Flags", $"{ISOCode}.png");
            }
        }
    }
}
