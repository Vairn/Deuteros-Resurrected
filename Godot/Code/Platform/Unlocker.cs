using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Platform
{
    internal class Unlocker
    {
        public Unlocker()
        {
            GameCore.SingletonInstance.ProductionFinished += SingletonInstance_ProductionFinished;
            GameCore.SingletonInstance.ResearchFinished += SingletonInstance_ResearchFinished;
            GameCore.SingletonInstance.ShipCreated += SingletonInstance_ShipCreated;
            GameCore.SingletonInstance.StationPiecePlaced += SingletonInstance_StationPiecePlaced;
        }

        private void SingletonInstance_StationPiecePlaced(Enums.StellarBodies stellarBody)
        {
            if (stellarBody == Enums.StellarBodies.earth && GameCore.SingletonInstance.GetCurrentPlanet().Station.BuildParts == 1)
            {
                GameCore.SingletonInstance.ShowBulletin(Enums.BulletinTypes.ios);

                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.i_chassis).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.i_drive).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.a__c__c).Research.Locked = false;
            }
        }

        private void SingletonInstance_ShipCreated(Objects.Interfaces.IShip ship)
        {
            if (ship.ShipType == Enums.Ship_Types.Shuttle && !GameCore.SingletonInstance.GameData.Unlocks.Contains(Enums.Game_Unlocks.Shuttle_Unlock))
            {
                GameCore.SingletonInstance.GameData.Unlocks.Add(Enums.Game_Unlocks.Shuttle_Unlock);
                
                GameCore.SingletonInstance.TriggerUnlockAdded(Enums.Game_Unlocks.Shuttle_Unlock);
            }
        }

        private void SingletonInstance_ResearchFinished(Objects.ResearchItem researchItem)
        {
            
        }

        private void SingletonInstance_ProductionFinished(Objects.Factory factory)
        {
            
        }
    }
}
