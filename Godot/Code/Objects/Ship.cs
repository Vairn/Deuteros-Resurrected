using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

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

        public virtual void Dock()
        {
            if (this.ShipState == Ship_States.UnDocked && GameCore.SingletonInstance.GameData.Planets[this.PlanetLocation].Station.Built)
            {
                this.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                this.ShipState = Ship_States.Docking;
            }
        }

        public virtual void Land()
        {
            if (this.ShipType == Ship_Types.Shuttle && this.ShipState == Ship_States.UnDocked)
            {
                this.ShipState = Ship_States.Landing;
                this.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;

            }
        }


        public virtual void TakeOff()
        {
            if (this.Engine && this.Fuel > 0)
                if (this.ShipType == Ship_Types.Shuttle && ((Shuttle)this).OnGround == true)
                {
                    ((Shuttle)this).OnGround = false;
                    this.ShipState = Ship_States.TakingOff;
                    this.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                }
                else if (this.ShipState == Ship_States.Docked)
                {
                    this.ShipState = Ship_States.Launching;
                    this.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                }
        }

        public virtual int TravelTimeRemain()
        {
            return 0;
        }

        public Objects.ACC ACC { get; set; }
    }
}