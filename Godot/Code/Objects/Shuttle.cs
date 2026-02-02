using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class Shuttle : Ship, IShip
    {
        public bool OnGround { get; set; }
        public bool Landing { get; set; }
        public bool Climbing { get; set; }

        public Shuttle()
        {
            ShipID = Guid.NewGuid();
            Name = "Shuttle Craft";
        }
    }
}