using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform.Base;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Objects
{
    public class ACC
    {
        public Enums.StellarBodies Source { get; set; }
        public Enums.StellarBodies Destination { get; set; }

        public List<Enums.ItemTypes> SourceItems { get; set; }
        public List<Enums.ItemTypes> DestinationItems { get; set; }

        public Enums.ItemTypes CurrentSource { get; set; }
        public Enums.ItemTypes CurrentDestination { get; set; }

        public bool Active { get; set; }
        public bool CycleMode { get; set; }

        public IShip Ship { get; set; }

        public bool Refuel()
        {
            if (Ship.Fuel >= 50) return true;

            var stores = GameCore.SingletonInstance.GameData.Planets[Ship.PlanetLocation].Station.Resources.Stores;

            if (Ship.ShipType == Ship_Types.Shuttle && ((Shuttle)Ship).OnGround)
                stores = GameCore.SingletonInstance.GameData.Planets[Ship.PlanetLocation].PlanetResources.Stores;

            if (stores[Ship.FuelType] >= (100 - Ship.Fuel))
            {
                stores[Ship.FuelType] -= (100 - Ship.Fuel);
                Ship.Fuel = 100;
                return true;
            }
            else if (stores[Ship.FuelType] >= (50 - Ship.Fuel))
            {
                Ship.Fuel += stores[Ship.FuelType];
                stores[Ship.FuelType] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadSupply()
        {
            if (Ship.ShipState == Ship_States.Docked)
            {
                var stores = GameCore.SingletonInstance.GameData.Planets[Ship.PlanetLocation].Station.Resources.Stores;
                if (Ship.ShipType == Ship_Types.Shuttle && ((Shuttle)Ship).OnGround)
                {
                    stores = GameCore.SingletonInstance.GameData.Planets[Ship.PlanetLocation].PlanetResources.Stores;
                }

                //unload existing 
                if (Ship.Modules[0].ItemStored != ItemTypes.none && Ship.Modules[0].ItemCount != 0)
                {
                    stores[Ship.Modules[0].ItemStored] += Ship.Modules[0].ItemCount;
                }
                Ship.Modules[0].ItemStored = ItemTypes.none;
                Ship.Modules[0].ItemCount = 0;


                if (Ship.ShipType == Ship_Types.Shuttle && !((Shuttle)Ship).OnGround)
                {
                    var currentItemType = this.CurrentDestination;
                    do
                    {
                        if (this.DestinationItems.Contains(this.CurrentDestination) && stores[this.CurrentDestination] > 0)
                        {
                            Ship.Modules[0].ItemStored = this.CurrentDestination;
                            Ship.Modules[0].ItemCount = Math.Min(250,stores[this.CurrentDestination]);
                            stores[this.CurrentDestination] -= Ship.Modules[0].ItemCount;
                        }
                        this.CurrentDestination++;
                        if (this.CurrentDestination > ItemTypes.meh_fuel) this.CurrentDestination = ItemTypes.iron;
                    } while (this.CurrentDestination != currentItemType && Ship.Modules[0].ItemCount == 0);
                }
                else
                {
                    var currentItemType = this.CurrentSource;
                    do
                    {
                        if (this.SourceItems.Contains(this.CurrentSource) && stores[this.CurrentSource] > 0)
                        {
                            Ship.Modules[0].ItemStored = this.CurrentSource;
                            Ship.Modules[0].ItemCount = Math.Min(250, stores[this.CurrentSource]);
                            stores[this.CurrentSource] -= Ship.Modules[0].ItemCount;
                        }
                        this.CurrentSource++;
                        if (this.CurrentSource > ItemTypes.meh_fuel) this.CurrentSource = ItemTypes.iron;
                    } while (this.CurrentSource != currentItemType && Ship.Modules[0].ItemCount == 0);

                }

            }

        }

        public void Update(Ship_States oldState)
        {
            if (!this.Active) return;

            if (Ship.ShipType == Ship_Types.Shuttle)
            {
                if (Ship.ShipState == Ship_States.UnDocked && oldState == Ship_States.TakingOff)
                {
                    Ship.Dock();
                }
                else if (Ship.ShipState == Ship_States.UnDocked && oldState == Ship_States.Launching)
                {
                    Ship.Land();
                }
                else if (((((Shuttle)Ship).OnGround) && oldState == Ship_States.Landing) ||
                        (Ship.ShipState== Ship_States.Docked && oldState == Ship_States.Docking))
                {
                    if (Refuel())
                    {
                        LoadSupply();
                        Ship.TakeOff();
                    }
                }
            }
        }

        public void Activate()
        {
            if (Ship.ShipType == Ship_Types.Shuttle)
            {
                if ((Ship.ShipState == Ship_States.Docked) || (((Shuttle)Ship).OnGround))
                {
                    if (Refuel())
                    {
                        LoadSupply();
                        Ship.TakeOff();
                    }
                }
            }
        }
    }
}