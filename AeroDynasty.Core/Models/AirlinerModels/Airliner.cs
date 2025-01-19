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
        // Private vars
        private Registration _registration;
        private AircraftModel _model;
        private Airline _owner;
        private DateTime _productionDate;

        // Public vars
        public Registration Registration { get => _registration; }
        public AircraftModel Model { get => _model; }
        public Airline Owner { get => _owner; }
        public DateTime ProductionDate { get => _productionDate; }
        public String FormattedProductionDate { get => ProductionDate.ToString("dd-MMM-yyyy"); }

        /// <summary>
        /// Create a new airliner
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="model"></param>
        /// <param name="owner"></param>
        /// <param name="productionDate"></param>
        public Airliner(Registration registration, AircraftModel model, Airline owner, DateTime productionDate )
        {
            _registration = registration;
            _model = model;
            _owner = owner;
            _productionDate = productionDate;
        }

    }
}
