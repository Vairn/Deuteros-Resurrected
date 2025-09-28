using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class Shuttle : IShip
    {
        #region interface
        public Enums.Planetoids PlanetLocation { get; set; }
        public Enums.Stars StarLocation { get; set; }
        public int StartTravelDay { get; set; }
        public Enums.Ship_Types ShipType { get; set; }
        public bool Docked { get; set; }
        public bool Engine { get; set; }
        public Staff Pilot { get; set; }
        public int Fuel { get; set; }
        public Enums.Fuel_Types FuelType { get; set; }
        public List<ShipModule> Modules { get; set; }
        #endregion

        public bool OnGround { get; set; }
    }
}