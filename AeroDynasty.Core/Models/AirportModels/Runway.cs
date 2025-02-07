using AeroDynasty.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.AirportModels
{
    public class Runway
    {
        public string Name { get; set; }
        public RunwaySurface Surface { get; set; }
        public int Length { get; set; }

        // Constructor
        public Runway(string name, RunwaySurface surface, int length)
        {
            Name = name;
            Surface = surface;
            Length = length;
        }
    }
}
