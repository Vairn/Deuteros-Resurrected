using Godot;
using System;
using System.Collections.Generic;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class StationResource
    {
        public Enums.Planetoids PlanetId { get; set; }
        public Store Stores { get; set; }
        public List<Staff> Staff { get; set; }

        public StationResource() 
        {
            Stores = new Store();
            Staff = new List<Staff>();
        }
    }
}