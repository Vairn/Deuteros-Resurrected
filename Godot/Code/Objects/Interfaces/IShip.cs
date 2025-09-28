using Godot;
using System.Collections.Generic;
using System.Reflection;

namespace Deuteros.Code.Objects.Interfaces
{
    public interface IShip
    {
        //Also doubles as starting point for travel calculations
        public Enums.Planetoids PlanetLocation { get; set; }
        //Also doubles as starting point for travel calculations
        public Enums.Stars StarLocation { get; set; }
        public int StartTravelDay { get; set; }
        public Enums.Ship_Types ShipType { get; set; }
        public bool Docked { get; set; }
        public bool Engine { get; set; }
        public Staff Pilot { get; set; }
        public int Fuel { get; set; }
        public Enums.Fuel_Types FuelType { get; set; }
        public List<ShipModule> Modules { get; set; }
    }
}