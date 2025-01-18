using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Models.AircraftModels;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.Core;

namespace AeroDynasty.Core.Models.AirlinerModels
{
    public class Airliner
    {
        public Registration Registration { get; set; }
        public AircraftModel Model { get; set; }
        public Airline Owner { get; set; }

    }
}
