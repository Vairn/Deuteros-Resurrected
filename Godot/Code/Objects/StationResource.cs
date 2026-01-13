using Deuteros.Code.Platform;
using Godot;
using System;
using System.Collections.Generic;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class StationResource : Deuteros.Code.Platform.Resource
    {
        public Enums.Planetoids PlanetId { get; set; }
        public Store Stores { get; set; }

        public StationResource() 
        {
            Stores = new Store();
        }
    }
}