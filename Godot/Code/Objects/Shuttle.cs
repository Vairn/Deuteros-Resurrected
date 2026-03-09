using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class Shuttle : Ship, IShip
    {
        public bool OnGround { get; set; }
        
        public Shuttle()
        {
            ShipID = Guid.NewGuid();
            Name = "Shuttle Craft";
        }

        public override int TravelTimeRemain()
        {
            if (ShipState == Enums.Ship_States.Landing)
                return 2 - (int)(GameCore.SingletonInstance.GameData.CurrentDay - StartTravelDay);
            else if (ShipState == Enums.Ship_States.TakingOff)
                return 5 - (int)(GameCore.SingletonInstance.GameData.CurrentDay - StartTravelDay);
            else
                return 0;
        }
    }
}