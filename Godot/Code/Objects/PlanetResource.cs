using Godot;
using System;
using System.Collections.Generic;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class PlanetResource
    {
        public Enums.Planetoids PlanetId { get; set; }
        public int Derricks { get; set; }
        public List<Staff> Staff { get; set; }
        public List<Objects.Material> Materials { get; set; }

        public PlanetResource(List<Objects.Material> materials)
        {
            Materials = materials;
            Staff = new List<Staff>();
        }
    }
}