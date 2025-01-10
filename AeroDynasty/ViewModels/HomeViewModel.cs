﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroDynasty.Core.Models.AirlineModels;
using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;

namespace AeroDynasty.ViewModels
{
    public class HomeViewModel : _BaseViewModel
    {
        public ObservableCollection<Country> Countries { get; set; }
        public ObservableCollection<RegistrationTemplate> Registrations { get; set; }
        public ObservableCollection<Airline> Airlines { get; set; }
        public HomeViewModel()
        {
            Countries = new ObservableCollection<Country>();
            Registrations = new ObservableCollection<RegistrationTemplate>();
            Airlines = new ObservableCollection<Airline>();

            Countries = GameData.Instance.Countries;
            Registrations = GameData.Instance.RegistrationTemplates;
            Airlines = GameData.Instance.Airlines;
        }
    }
}
