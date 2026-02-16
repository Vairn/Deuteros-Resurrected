using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Deuteros.Code.Objects.Interfaces
{
    public interface IShip
    {
        public Guid ShipID { get; set; }
        public Enums.StellarBodies PlanetLocation { get; set; }
        public Enums.StellarBodies StarLocation { get; set; }
        public int StartTravelDay { get; set; }
        public Enums.Ship_Types ShipType { get; set; }
        public bool Docked { get; set; }
        public bool Docking { get; set; }
        public bool Launching { get; set; }
        public bool Engine { get; set; }
        public Staff Pilot { get; set; }
        public string Name { get; set; }
        public int Fuel { get; set; }
        public Enums.Fuel_Types FuelType { get; set; }
        public List<ShipModule> Modules { get; set; }

        #region ACC
        public bool ACC { get; set; }
        public bool ACCEnabled { get; set; }
        public Enums.StellarBodies ACCFrom { get; set; }
        public Enums.StellarBodies ACCTo { get; set; }
        public List<Enums.ItemTypes> ACCSend { get; set; }
        public List<Enums.ItemTypes> ACCReceive { get; set; }
        #endregion
    }
}