using Deuteros.Code.Platform.Base;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Deuteros.Code.Platform.Helpers;
using System.Security.Cryptography.X509Certificates;
using Deuteros.Code.Objects.Interfaces;
using Newtonsoft.Json.Linq;
using System.Reflection;
using static Deuteros.Code.Enums;
using Deuteros.Code.Utility;

namespace Deuteros.Code.Platform.Screens
{
    public partial class ShipInterior : BaseSubScene
    {
        public const string NavSpriteBasePath = "res://Sprites//Buttons//Shipbay//";

        public Color Red { get; set; } = new Color(255, 0, 0, 255);
        public Color Green { get; set; } = new Color(0, 136, 0, 255);
        public Color Bland { get; set; } = new Color(153, 170, 119, 255);
        public Color Yellow { get; set; } = new Color(255, 255, 0, 255);

        public IShip Ship { get; set; }
        public IPlanet CurrentPlanet { get; set; }
        TextureRect Location { get; set; }
        TextureButton SmallLocation { get; set; }
        TextureButton ACC { get; set; }
        TextureButton SetCourse { get; set; }
        TextureButton EngageEngine { get; set; }
        TextureButton DisengageEngine { get; set; }

        Label ShipName { get; set; }
        Label Status { get; set; }
        Label FuelValue { get; set; }
        Label PilotName { get; set; }
        Label PilotCount { get; set; }
        Label EngineStatusValue { get; set; }
        Label ACCStatus { get; set; }
        Label[] CargoValues { get; set; } = new Label[3];
        Label CourseText { get; set; }
        Label CourseValue { get; set; }
        Label ETA { get; set; }

        public override void _Ready()
        {
            //Setup some flags to make our lives easier
            CurrentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();

            Ship = GameCore.SingletonInstance.GameData.Ships.Single(T => T.ShipID == GameCore.SingletonInstance.ShipSelected);

            Location = GetNode<TextureRect>("Location");

            ACC = GetNode<TextureButton>("ACC");
            SetCourse = GetNode<TextureButton>("SetCourse");
            EngageEngine = GetNode<TextureButton>("EngineControls/EngageEngine");
            DisengageEngine = GetNode<TextureButton>("EngineControls/DisengageEngine");

            ShipName = GetNode<Label>("TextLayout/ShipName");
            Status = GetNode<Label>("TextLayout/Status");
            FuelValue = GetNode<Label>("TextLayout/FuelValue");
            PilotName = GetNode<Label>("TextLayout/PilotName");
            PilotCount = GetNode<Label>("TextLayout/PilotCount");
            EngineStatusValue = GetNode<Label>("TextLayout/EngineStatusValue");
            ACCStatus = GetNode<Label>("TextLayout/ACCStatus");
            CargoValues[0] = GetNode<Label>("TextLayout/CargoValue1");
            CargoValues[1] = GetNode<Label>("TextLayout/CargoValue2");
            CargoValues[2] = GetNode<Label>("TextLayout/CargoValue3");
            CourseText = GetNode<Label>("TextLayout/CourseText");
            CourseValue = GetNode<Label>("TextLayout/CourseValue");
            ETA = GetNode<Label>("TextLayout/ETA");

            ACC.Pressed += ACC_Pressed;
            SetCourse.Pressed += SetCourse_Pressed;
            EngageEngine.Pressed += EngageEngine_Pressed;
            DisengageEngine.Pressed += DisengageEngine_Pressed;

            UpdateState();

            base._Ready();
        }

        private void DisengageEngine_Pressed()
        {
            throw new NotImplementedException();
        }

        private void EngageEngine_Pressed()
        {
            throw new NotImplementedException();
        }

        private void SetCourse_Pressed()
        {
            throw new NotImplementedException();
        }

        private void ACC_Pressed()
        {
            throw new NotImplementedException();
        }

        private void UpdateState()
        {
            ShipName.Text = Ship.Name;

            if (Ship.GetType() == typeof(Shuttle) && ((Shuttle)Ship).Landing)
                Status.Text = "Landing On/n" + Ship.PlanetLocation;
            if (Ship.GetType() == typeof(Shuttle) && ((Shuttle)Ship).Climbing)
                Status.Text = "Climbing From/n" + Ship.PlanetLocation;
            else if (Ship.GetType() == typeof(Shuttle) && ((Shuttle)Ship).OnGround)
                Status.Text = "In Ground Bay/n" + Ship.PlanetLocation;
            else if (Ship.Launching)
                Status.Text = "Launching From/n" + Ship.PlanetLocation;
            else if (Ship.Docking)
                Status.Text = "Docking With/n" + Ship.PlanetLocation;
            else if (Ship.Docked)
                Status.Text = "Docked Above/n" + Ship.PlanetLocation;
            else if (Ship.GetType() == typeof(InterStellarShip) && ((InterStellarShip)Ship).InTransit)
                Status.Text = "In Transit To/n" + Ship.PlanetLocation;
            else
                Status.Text = "Orbitting/n" + Ship.PlanetLocation;


            FuelValue.Text = Ship.Fuel.ToString();
            PilotName.Text = Ship.Pilot.Leader;
            PilotCount.Text = Ship.Pilot.Count.ToString();

            EngineStatusValue.RemoveThemeColorOverride("font_color");

            if (!Ship.Engine)
            {
                EngineStatusValue.Text = "Not Installed";
                EngineStatusValue.AddThemeColorOverride("font_color", Red);
            }
            else if (Ship.Docked)
            {
                EngineStatusValue.Text = "Disengaged";
                EngineStatusValue.AddThemeColorOverride("font_color", Red);
            } 
            else if (Ship.Docking || Ship.Launching || 
                (Ship.GetType() == typeof(InterStellarShip) && ((InterStellarShip)Ship).InTransit)
                )
            {
                EngineStatusValue.Text = "Engaged";
                EngineStatusValue.AddThemeColorOverride("font_color", Red);
            }
            else if (Ship.GetType() == typeof(Shuttle) && 
                (((Shuttle)Ship).Climbing || ((Shuttle)Ship).Landing))
            {
                EngineStatusValue.Text = "Engaged";
                EngineStatusValue.AddThemeColorOverride("font_color", Red);
            }

            if (Ship.ACC && Ship.ACCEnabled)
                ACCStatus.Text = "A.C.C Is Engaged";
            else if (Ship.ACC && !Ship.ACCEnabled)
                ACCStatus.Text = "A.C.C Is Disengaged";
            else
                ACCStatus.Text = "";

            for (int i = 0; i < 3; i++)
            {
                if (Ship.Modules[i].ModuleType == Module_Types.None)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", Green);
                    CargoValues[i].Text = "free";
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Supply)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", Bland);
                    if (Ship.Modules[i].ItemCount > 0)
                        CargoValues[i].Text = Ship.Modules[i].ItemCount + " " + Ship.Modules[i].ItemStored.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Tool)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", Green);
                    
                    if (Ship.Modules[i].ItemStored != ItemTypes.none)
                        CargoValues[i].Text = Ship.Modules[i].ItemStored.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Cryo)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", Yellow);
                    
                    if (Ship.Modules[i].StaffStored != null)
                        CargoValues[i].Text = Ship.Modules[i].StaffStored.Type.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";
                }
            }

            CourseText.Text = Ship.PlanetLocation.ToScreenString() + " To/n" + ((InterStellarShip)Ship).PlanetDestination.ToScreenString();

            var currentDay = GameCore.SingletonInstance.GameData.CurrentDay + ((InterStellarShip)Ship).TravelTimeRemain();
            var curDay = (currentDay % 1000).ToString().PadLeft(3, '0');
            var outputYear = (3100 + Math.Floor((decimal)(currentDay / 1000))) + " " + curDay + ".00";

            ETA.Text = "ETA/n" + outputYear;

            //TODO - Set images
            //Click events for images
            //Click events for set destination
            //Click events for ACC
            //Click events for engines
            //Click events for modules
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Right && mb.Pressed)
            {
                var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");

                if (cursor.IsLocked)
                {
                    //if (CargoService.Visible == true)
                    //    CargoService.Visible = false;
                    //else if (EquipmentStock.Visible == true)
                    //    EquipmentStock.Visible = false;
                    //else if (StaffList.Visible == true)
                    //    StaffList.Visible = false;

                    cursor.Unlock();

                    GetViewport().SetInputAsHandled();
                }
            }
        }

        //Triggered from gamecore
        protected override void DayTick(uint currentDay, uint nextDay)
        {
            UpdateState();
            DrawData();
        }

        // Called every update.
        public override void _Draw()
        {
            DrawData();
        }

        public void DrawData()
        {

        }
    }
}