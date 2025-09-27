using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class ResearchItem
    {
        public Enums.ItemTypes ItemType { get; set; }
        public int ResearchMultiplier { get; set; }
        public bool Researched { get; set; }
        public int Index { get; set; }
        public bool Locked { get { return ResearchPercentageComplete == 0; }  }
        public int TechLevel { get; set; }
        public int ResearchValue { get; set; }
        public int ResearchPercentageComplete { get; set; }
        public int ResearchOrder { get; set; }

        public ResearchItem()
        { }

        public ResearchItem(Enums.ItemTypes itemType, int index, int techLevel)
        { 
            ItemType = itemType;
            TechLevel = techLevel;
            Index = index;
            ResearchMultiplier = 64;
            Researched = false;
            ResearchValue = 64;
            ResearchPercentageComplete = 0;
        }

        public void UpdateResearch()
        {
            //If the item is researched, return 100
            if (Researched)
                return;

            var earth = GameCore.SingletonInstance.GetPlanet<Earth>(Enums.Planetoids.earth);

            //If the researchstaff is null, then the game has just started, do nothing
            if (earth.ResearchStaff == null)
                return;
            else
            {
                var level = earth.ResearchStaff.GetLevel();
                var teamSize = earth.ResearchStaff.Count;

                int v = (teamSize << level) * ResearchMultiplier / 801;

                if ((ResearchValue + v) > 255)
                {
                    ResearchValue = (ResearchValue + v) & 0xFF; // Overflow wraparound
                    if (ResearchPercentageComplete < 100)
                    {
                        ResearchPercentageComplete += 11;
                        if (ResearchPercentageComplete > 100)
                            ResearchPercentageComplete = 100;
                    }
                }
                else
                {
                    ResearchValue += v;
                }

                if (ResearchPercentageComplete == 100)
                {
                    Researched = true;
                    ResearchOrder = GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Research != null && T.Research.Researched).Count();
                    GameCore.SingletonInstance.GameData.GetItem(ItemType).Locked = false;
                    earth.ResearchStaff.ActionsTaken++;

                    GameCore.SingletonInstance.TriggerResearchFinished(GameCore.SingletonInstance.GameData.GetItem(ItemType).Research);
                }
            }
        }
    }
}