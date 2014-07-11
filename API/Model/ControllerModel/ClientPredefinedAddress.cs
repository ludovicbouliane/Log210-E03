﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DomainModel;

namespace Model.ControllerModel
{
    public class ClientPredefinedAddress
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; } 
    }
}
