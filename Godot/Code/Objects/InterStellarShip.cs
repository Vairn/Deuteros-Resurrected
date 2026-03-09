using Deuteros.Code.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class InterStellarShip : Ship, IShip
    {
        public Enums.StellarBodies PlanetDestination { get; set; }
        public Enums.StellarBodies StarDestination { get; set; }
        public bool InTransit { get; set; }

        public override int TravelTimeRemain()
        {
            int totalJourneyTime = 0;

            var startPlanet = GameCore.SingletonInstance.GameData.Planets[PlanetLocation];
            var destinationPlanet = GameCore.SingletonInstance.GameData.Planets[PlanetDestination];

            if (startPlanet.MoonParentPlanetId != Enums.StellarBodies.none)
                startPlanet = GameCore.SingletonInstance.GameData.Planets[startPlanet.MoonParentPlanetId];

            if (destinationPlanet.MoonParentPlanetId != Enums.StellarBodies.none)
                destinationPlanet = GameCore.SingletonInstance.GameData.Planets[destinationPlanet.MoonParentPlanetId];

            //Travelling within the same planetary system
            if (startPlanet == destinationPlanet)
            {
                totalJourneyTime = Math.Max(Math.Abs(GameCore.SingletonInstance.GameData.Planets[PlanetLocation].Order - GameCore.SingletonInstance.GameData.Planets[PlanetDestination].Order), 1);
            }
            //We're going to a different planet
            else if (startPlanet != destinationPlanet)
            {
                totalJourneyTime = Math.Abs(destinationPlanet.Order - startPlanet.Order) * 4;
            }

            return totalJourneyTime - (int)(GameCore.SingletonInstance.GameData.CurrentDay - StartTravelDay);
        }
    }
}
