using Godot;
using System;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class Ship
    {
        public Enums.Planetoids PlanetId { get; set; }
        public bool Docked { get; set; }
        public bool InOrbit { get; set; }
        public Enums.Planetoids Destination { get; set; }
        public Enums.StaffType Staff { get; set; }
    }
}