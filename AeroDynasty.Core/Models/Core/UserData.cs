﻿using AeroDynasty.Core.Models.AirlineModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class UserData : _BaseModel
    {
        private Airline _airline { get; set; }
        public Airline Airline
        {
            get
            {
                return _airline;
            }
            set
            {
                _airline = value;
                OnPropertyChanged(nameof(Airline));
            }
        }

        /// <summary>
        /// Constructs a new user data class
        /// </summary>
        public UserData()
        {
        }
    }
}
