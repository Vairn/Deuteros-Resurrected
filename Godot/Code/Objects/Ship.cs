using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class Ship : IShip
    {
        public Guid ShipID { get; set; }
        public Enums.StellarBodies PlanetLocation { get; set; }
        public Enums.StellarBodies StarLocation { get; set; }
        public Enums.StellarBodies DestinationPlanetLocation { get; set; }
        public Enums.StellarBodies DestinationStarLocation { get; set; }
        public uint StartTravelDay { get; set; }
        public Enums.Ship_Types ShipType { get; set; }
        public Enums.Ship_States ShipState { get; set; }
        public bool Engine { get; set; }
        public Staff Pilot { get; set; }
        public string Name { get; set; }
        public int Fuel { get; set; }
        public bool LocationView { get; set; }
        public Enums.ItemTypes FuelType { get; set; }
        public List<ShipModule> Modules { get; set; }

        public virtual int TravelTimeRemain()
        {
            return 0;
        }

        public Objects.ACC ACC { get; set; }
    }
}