using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class Planet : Interfaces.IPlanet
    {
        public PlanetResource PlanetResources { get; set; }
        public Store Stores { get; set; }
        public bool ActivePlayer { get; set; }
        public bool IsMoon { get; set; }
        public bool ActiveMethanoid { get; set; }
        public SpaceStation Station { get; set; }
        public Enums.Planetoids PlanetId { get; set; }
        public Enums.Planetoids MoonParentPlanetId { get; set; }
        public Enums.Stars ParentStar { get; set; }
        public Shuttle Shuttle { get; set; }

        public Planet()
        {
            Deuteros.Code.GameCore.SingletonInstance.DayPassed += DayTick;
        }

        public virtual void AddItems(Enums.ItemTypes itemToAdd, int count)
        {
            Station.Resources.Stores[itemToAdd] += count;
        }

        //Triggered from gamecore
        public void DayTick(uint currentDay, uint nextDay)
        {
            int daysDifference = (int)Math.Floor((decimal)(nextDay - currentDay));

            //TODO This isn't right - We need to mine every other day, but mining can start on any day - Or can it?
            if (PlanetId == Enums.Planetoids.earth && nextDay % 2 != 0)
            {
                return;
            }

            var randomGen = new Random((int)Time.GetTicksMsec());

            if (PlanetResources.Derricks > 0)
            {
                foreach (var material in PlanetResources.Materials)
                {
                    if (material.GroundAmount < 1 && material.SurveyTicks == 0)
                    {
                        material.SurveyTicks = randomGen.Next(0, 8) * GameCore.SingletonInstance.GameData.ResourceLevels_Survey_Multiplier[material.MaterialType];
                    }
                    else if (material.GroundAmount < 1 && material.SurveyTicks > 0)
                    {
                        material.SurveyTicks--;

                        if (material.SurveyTicks == 0)
                            material.GroundAmount = (randomGen.Next(0, 32768) * GameCore.SingletonInstance.GameData.ResourceLevels_Survey_Multiplier[material.MaterialType]) & 0x7FFF;
                    }
                    else
                    {
                        int amountRemoved = (PlanetResources.Derricks * GameCore.SingletonInstance.GameData.ResourceRate_Per_Derrick[material.MaterialType]) * daysDifference;
                        material.GroundAmount -= amountRemoved;

                        Stores[material.MaterialType] += amountRemoved;
                    }
                }
            }

        }
    }
}