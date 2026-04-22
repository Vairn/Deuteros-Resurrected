using Deuteros.Code.Objects;
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
                GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Add(Enums.Game_Unlocks.First_Station_Segment);

                GameCore.SingletonInstance.ShowBulletin(Enums.BulletinTypes.ios);

                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.i_chassis).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.i_drive).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.a__c__c).Research.Locked = false;
            }
            else if (stellarBody == Enums.StellarBodies.earth && GameCore.SingletonInstance.GetCurrentPlanet().Station.Built)
            {
                GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Add(Enums.Game_Unlocks.Space_Stations);
            }
        }

        private void SingletonInstance_ShipCreated(Objects.Interfaces.IShip ship)
        {
            if (ship.ShipType == Enums.Ship_Types.Shuttle && !GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Contains(Enums.Game_Unlocks.Shuttle_Unlock))
            {
                GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Add(Enums.Game_Unlocks.Shuttle_Unlock);
                
                GameCore.SingletonInstance.TriggerUnlockAdded(Enums.Game_Unlocks.Shuttle_Unlock);
            }
        }

        private void SingletonInstance_ResearchFinished(Objects.ResearchItem researchItem)
        {
            
        }

        private void SingletonInstance_ProductionFinished(Objects.Factory factory)
        {
            if (factory.CurrentProductionItem().Product.ItemType == Enums.ItemTypes.i_chassis && !GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Contains(Enums.Game_Unlocks.IOS_Attachments))
            {
                GameCore.SingletonInstance.GameData.ActiveSaveFile.Unlocks.Add(Enums.Game_Unlocks.IOS_Attachments);

                GameCore.SingletonInstance.ShowBulletin(Enums.BulletinTypes.ios_attachments);

                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.a__m__a).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.a__o__c).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.bandaid).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.grapple).Research.Locked = false;
                GameCore.SingletonInstance.GameData.GetItem(Enums.ItemTypes.r_frame).Research.Locked = false;
            }

        }
    }
}