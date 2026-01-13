using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class Training
    {
        public int AvailableTrainees { get; set; }

        public int ResearcherMaxCount { get; set; }
        public int ProductionMaxCount { get; set; }

        public int ResearcherTrainingMax { get; set; }
        public int ProductionTrainingMax { get; set; }
        public int MarinesTrainingMax { get; set; }

        public bool ResearcherLocked { get; set; }
        public bool ProductionLocked { get; set; }
        public bool MarinesLocked { get; set; }

        public int ResearcherTrainingCount { get; set; }
        public int ProductionTrainingCount { get; set; }
        public int MarinesTrainingCount { get; set; }
        public uint ResearcherDayStart { get; set; }
        public uint ProductionDayStart { get; set; }
        public uint MarinesDayStart { get; set; }
        public int ResearcherTrainingTime { get; set; }
        public int ProductionTrainingTime { get; set; }
        public int MarinesTrainingTime { get; set; }

        public int ReferenceTeamSize { get; set; }
        public double ReferenceLevelMultiplier { get; set; }
        public double ReferenceDuration { get; set; }

        public double ReferenceCost { get { return ReferenceTeamSize * ReferenceLevelMultiplier * ReferenceDuration; } }

        //Called from Earth
        public void ChildDayTick(uint currentDay, uint nextDay)
        {
            var earth = GameCore.SingletonInstance.GetPlanet<Earth>(Enums.Planetoids.earth);

            if (!GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked = true;
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherDayStart = currentDay;

            }
            else if (GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && (currentDay - GameCore.SingletonInstance.Earth.TrainingData.ResearcherDayStart) > GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingTime)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked = false;

                if (earth.ResearchStaff == null)
                {
                    var newResearcher = new Staff();
                    newResearcher.Leader = "Seth";
                    newResearcher.Count = 0;
                    newResearcher.Type = Enums.StaffType.Research;

                    earth.ResearchStaff = newResearcher;
                }

                earth.ResearchStaff.Count += GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount;
                GameCore.SingletonInstance.Earth.TrainingData.AvailableTrainees -= GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount;
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount = 0;

            }

            if (!GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked = true;
                GameCore.SingletonInstance.Earth.TrainingData.ProductionDayStart = currentDay;
            }
            else if (GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && (currentDay - GameCore.SingletonInstance.Earth.TrainingData.ProductionDayStart) > GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingTime)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked = false;

                if (earth.Factory.Builder == null)
                {
                    var newProduction = new Staff();
                    //TODO - Generate proper names
                    newProduction.Leader = "Roger";
                    newProduction.Count = GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount;
                    newProduction.Type = Enums.StaffType.Production;
                    earth.Factory.Builder = newProduction;
                }
                else
                {
                    earth.Factory.Builder.Count += GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount;
                }

                GameCore.SingletonInstance.Earth.TrainingData.AvailableTrainees -= GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount;
                GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount = 0;
            }

            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked = true;
                GameCore.SingletonInstance.Earth.TrainingData.MarinesDayStart = currentDay;
            }
            else if (GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && (currentDay - GameCore.SingletonInstance.Earth.TrainingData.MarinesDayStart) > GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingTime)
            {
                //Only produce marines if we have space for them
                if (GameCore.SingletonInstance.Earth.PlanetResources.Staff.Any(T => T == null))
                {
                    GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked = false;

                    var newMarine = new Staff();
                    //TODO - Generate proper names
                    newMarine.Leader = "Roger" + Random.Shared.Next(0, 100).ToString();
                    newMarine.Count = GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount;
                    newMarine.Type = Enums.StaffType.Marines;

                    earth.PlanetResources.AddStaff(newMarine);

                    GameCore.SingletonInstance.Earth.TrainingData.AvailableTrainees -= GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount;
                    GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount = 0;
                }
            }
        }
    }
}