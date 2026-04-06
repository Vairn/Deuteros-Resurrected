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
using Deuteros.Code.Platform.Screens.ModuleScenes;
using System.Numerics;

namespace Deuteros.Code.Platform.Screens
{
    public partial class ShipInterior : BaseSubScene
    {
        public const string SpriteBasePath = "res://Sprites//SceneSprites//Ships//Interior//";

        public IShip Ship { get; set; }
        public IPlanet CurrentPlanet { get; set; }

        Control TextLayout { get; set; }
        Control StarMap { get; set; }
        Control Window { get; set; }
        Control ACC { get; set; }

        TextureRect LandingBlank { get; set; }
        TextureRect EngineControls { get; set; }
        TextureRect BigLocation { get; set; }

        TextureButton SmallLocation { get; set; }
        TextureButton OpenACC { get; set; }
        TextureButton SetCourse { get; set; }
        TextureButton[] Modules { get; set; } = new TextureButton[6];

        Button EngageEngine { get; set; }
        Button DisengageEngine { get; set; }
        Button Dock { get; set; }
        Button TakeOff { get; set; }
        Button Land { get; set; }

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

        StarMap DestinationStarMap { get; set; }
        OFFrameDeploy OfFrameDeployScene {get; set;}
        ACC ACCScreen { get; set; }

        public override void _Ready()
        {
            //Setup some flags to make our lives easier
            CurrentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();

            Ship = GameCore.SingletonInstance.GameData.Ships.Single(T => T.ShipID == GameCore.SingletonInstance.ShipSelected);

            TextLayout = GetNode<Control>("TextLayout");
            StarMap = GetNode<Control>("StarMap");
            Window = GetNode<Control>("Window");
            ACC = GetNode<Control>("ACCScreen");

            LandingBlank = GetNode<TextureRect>("LandingBlank");
            EngineControls = GetNode<TextureRect>("EngineControls");
            BigLocation = GetNode<TextureRect>("Location/BigLocation");

            OpenACC = GetNode<TextureButton>("OpenACC");
            SetCourse = GetNode<TextureButton>("SetCourse");
            SmallLocation = GetNode<TextureButton>("Location/SmallLocation");

            for (int i = 0; i < 6; i++)
            {
                Modules[i] = GetNode<TextureButton>("Modules/" + i.ToString().PadLeft(2, '0'));
                var modulePressed = i;
                Modules[i].Pressed += () => ShipInterior_Pressed(modulePressed);
                Modules[i].Visible = false;
            }

            EngageEngine = GetNode<Button>("EngineControls/EngageEngine");
            DisengageEngine = GetNode<Button>("EngineControls/DisengageEngine");
            Dock = GetNode<Button>("Dock");
            TakeOff = GetNode<Button>("TakeOff");
            Land = GetNode<Button>("Land");

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

            OpenACC.Pressed += ACC_Pressed;
            SetCourse.Pressed += SetCourse_Pressed;
            EngageEngine.Pressed += EngageEngine_Pressed;

            DisengageEngine.Pressed += DisengageEngine_Pressed;
            SmallLocation.Pressed += SmallLocation_Pressed;
            Dock.Pressed += Dock_Pressed;
            TakeOff.Pressed += TakeOff_Pressed;
            Land.Pressed += Land_Pressed;

            CargoValues[0].Text = "";
            CargoValues[1].Text = "";
            CargoValues[2].Text = "";

            UpdateState();

            base._Ready();
        }

        private async void ShipInterior_Pressed(int modulePressed)
        {
            if (Ship.ShipState == Ship_States.Docked)
            {
                var sceneVariables = new List<SceneVariables>();

                if (Ship.ShipType == Ship_Types.Shuttle && ((Shuttle)Ship).OnGround)
                {
                    CurrentPlanet.ShuttleState = modulePressed + 1;

                    sceneVariables.Add(Enums.SceneVariables.Ground);
                    sceneVariables.Add(Enums.SceneVariables.Shuttle);
                }
                else if (Ship.ShipType == Ship_Types.Shuttle)
                {
                    CurrentPlanet.Station.ShuttleState = modulePressed + 1;
                    sceneVariables.Add(Enums.SceneVariables.Orbit);
                    sceneVariables.Add(Enums.SceneVariables.Shuttle);
                }
                else if (Ship.ShipType != Ship_Types.Shuttle)
                { 
                    CurrentPlanet.Station.StarShipState = modulePressed + 1;
                    sceneVariables.Add(Enums.SceneVariables.Orbit);
                    sceneVariables.Add(Enums.SceneVariables.Ship);
                }

                //Double underscores in scene names represent a flag to pass to the scene
                var sceneNameSplit = Enums.Scenes.ShipBay.ToString().Split(new string[] { "__" }, System.StringSplitOptions.None);

                //Underscores in scene names represent a folder
                Deuteros.Code.GameCore.SingletonInstance.ChangeScene(sceneNameSplit[0].Replace("_", "/") + ".tscn", sceneVariables);
            }
            else if (Ship.ShipState != Ship_States.Docked)
            {
                if (Ship.Modules[modulePressed].ModuleType == Module_Types.Tool)
                {
                    if (Ship.Modules[modulePressed].ItemStored == ItemTypes.of_frame && CurrentPlanet.Station.Built == false && Ship.Pilot != null)
                    {
                        GameCore.LockScreen();

                        OfFrameDeployScene = GD.Load<PackedScene>("res://PreFabs/ShipModuleWindows/OFFrameDeploy.tscn").Instantiate<OFFrameDeploy>();
                        Window.AddChild(OfFrameDeployScene);
                        OfFrameDeployScene.WindowNumber.Text = (modulePressed + 1).ToString();
                        await OfFrameDeployScene.PlayThreeLabels(CurrentPlanet.Station.BuildParts + 1);
                        Window.RemoveChild(OfFrameDeployScene);
                        OfFrameDeployScene = null;

                        CurrentPlanet.Station.BuildParts++;

                        Ship.Modules[modulePressed].ItemStored = ItemTypes.none;
                        Ship.Modules[modulePressed].ItemCount = 0;

                        if (CurrentPlanet.Station.BuildParts == 8)
                        {
                            CurrentPlanet.Station.Built = true;
                        }

                        GameCore.SingletonInstance.TriggerStationPiecePlaced(CurrentPlanet.PlanetId);

                        GameCore.UnLockScreen();

                        UpdateState();
                    }
                }
            }
        }

        private void Land_Pressed()
        {
            if (Ship.ShipType == Ship_Types.Shuttle && Ship.ShipState == Ship_States.UnDocked)
            {
                Ship.ShipState = Ship_States.Landing;
                Ship.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                
                UpdateState();
            }
        }

        private void TakeOff_Pressed()
        {
            if (Ship.Engine && Ship.Fuel > 0)
                if (Ship.ShipType == Ship_Types.Shuttle && ((Shuttle)Ship).OnGround == true)
                {
                    ((Shuttle)Ship).OnGround = false;
                    Ship.ShipState = Ship_States.TakingOff;
                    Ship.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                }
                else if (Ship.ShipState == Ship_States.Docked)
                {
                    Ship.ShipState = Ship_States.Launching;
                    Ship.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                }

            UpdateState();
        }

        private void Dock_Pressed()
        {
            if (Ship.ShipState == Ship_States.UnDocked && CurrentPlanet.Station.Built)
            {
                Ship.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;
                Ship.ShipState = Ship_States.Docking;
                UpdateState();
            }
        }

        private void SmallLocation_Pressed()
        {
            Ship.LocationView = !Ship.LocationView;

            UpdateState();
        }

        private void DisengageEngine_Pressed()
        {
            //TODO - Actually not sure what to do with this, need help from data miner
        }

        private void EngageEngine_Pressed()
        {
            if (Ship.ShipState == Ship_States.UnDocked && Ship.Engine && Ship.DestinationPlanetLocation != Ship.PlanetLocation)
            {
                Ship.ShipState = Ship_States.InTransit;
                Ship.StartTravelDay = GameCore.SingletonInstance.GameData.CurrentDay;

                CurrentPlanet = null;

                UpdateState();
            }
        }

        private void SetCourse_Pressed()
        {
            DestinationStarMap = GD.Load<PackedScene>("res://PreFabs/StarMap.tscn").Instantiate<StarMap>();
            DestinationStarMap.ShowResources = false;

            StarMap.AddChild(DestinationStarMap);

            DestinationStarMap.LoadMap(Ship.DestinationPlanetLocation);

            var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");
            cursor.LockToRect(StarMap.GetGlobalRect());

            UpdateState();
        }

        private void ACC_Pressed()
        {
            ACCScreen = GD.Load<PackedScene>("res://PreFabs/ACC.tscn").Instantiate<ACC>();
            ACCScreen.SetACC(Ship.ACC);

            ACC.AddChild(ACCScreen);

            ACCScreen.UpdateState();

            var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");
            cursor.LockToRect(ACC.GetGlobalRect());

            UpdateState();
        }

        private void UpdateState()
        {
            if (CurrentPlanet == null && Ship.ShipState != Ship_States.InTransit)
                CurrentPlanet = GameCore.SingletonInstance.GameData.Planets[Ship.PlanetLocation];

            ShipName.Text = Ship.Name;

            if (Ship.GetType() == typeof(Shuttle) && Ship.ShipState == Ship_States.Landing)
                Status.Text = "Landing On\n" + Ship.PlanetLocation;
            if (Ship.GetType() == typeof(Shuttle) && Ship.ShipState == Ship_States.TakingOff)
                Status.Text = "Climbing From\n" + Ship.PlanetLocation;
            else if (Ship.GetType() == typeof(Shuttle) && ((Shuttle)Ship).OnGround)
                Status.Text = "In Ground Bay\n" + Ship.PlanetLocation;
            else if (Ship.ShipState == Ship_States.Launching)
                Status.Text = "Launching From\n" + Ship.PlanetLocation;
            else if (Ship.ShipState == Ship_States.Launching)
                Status.Text = "Docking With\n" + Ship.PlanetLocation;
            else if (Ship.ShipState == Ship_States.Docked)
                Status.Text = "Docked Above\n" + Ship.PlanetLocation;
            else if (Ship.ShipState == Ship_States.InTransit)
                Status.Text = "In Transit To\n" + Ship.DestinationPlanetLocation;
            else if (Ship.ShipState == Ship_States.Docking)
                Status.Text = "Docking With\n" + Ship.PlanetLocation;
            else
                Status.Text = "Orbitting\n" + Ship.PlanetLocation;

            FuelValue.Text = Ship.Fuel.ToString();
            PilotName.Text = Ship.Pilot == null ? "None" : Ship.Pilot.GetLevelString() + "\n" + Ship.Pilot.Leader;
            PilotCount.Text = Ship.Pilot == null ? "" : Ship.Pilot.Count.ToString();

            EngineStatusValue.RemoveThemeColorOverride("font_color");

            if (!Ship.Engine)
            {
                EngineStatusValue.Text = "Not Installed";
                EngineStatusValue.AddThemeColorOverride("font_color", CoreData.Red);
                EngageEngine.Visible = false;
                DisengageEngine.Visible = false;
            }
            else if (Ship.Engine)
            {
                EngageEngine.Visible = true;
                DisengageEngine.Visible = true;

                if (new List<Enums.Ship_States>() { Ship_States.Docking, Ship_States.Launching, Ship_States.Landing, Ship_States.TakingOff, Ship_States.InTransit }.Contains(Ship.ShipState))
                {
                    EngineStatusValue.Text = "Engaged";
                    EngineStatusValue.AddThemeColorOverride("font_color", CoreData.Green);
                }
                else
                {
                    EngineStatusValue.Text = "Disengaged";
                    EngineStatusValue.AddThemeColorOverride("font_color", CoreData.Red);
                }
            }

            if (Ship.ACC != null)
            {
                OpenACC.Visible = true;

                if (Ship.ACC.Active)
                    ACCStatus.Text = "A.C.C Is \nEngaged";
                else if (Ship.ACC.CycleMode)
                    ACCStatus.Text = "A.C.C Is \nFinishing";
                else if (!Ship.ACC.Active)
                    ACCStatus.Text = "A.C.C Is \nDisengaged";
            }
            else
            {
                OpenACC.Visible = false;

                ACCStatus.Text = "";
            }

            for (int i = 0; i < Ship.Modules.Count(); i++)
            {
                if (Ship.Modules[i].ModuleType == Module_Types.None)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", CoreData.Green);
                    CargoValues[i].Text = "free";
                    Modules[i].Visible = false;
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Supply)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", CoreData.Beige);
                    if (Ship.Modules[i].ItemCount > 0)
                        CargoValues[i].Text = Ship.Modules[i].ItemCount + " " + Ship.Modules[i].ItemStored.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";

                    Modules[i].Visible = true;
                    Modules[i].TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "Ship_Module_Supply.png"); 
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Tool)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", CoreData.Green);

                    if (Ship.Modules[i].ItemStored != ItemTypes.none)
                        CargoValues[i].Text = Ship.Modules[i].ItemStored.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";

                    Modules[i].Visible = true;
                    Modules[i].TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "Ship_Module_Tool.png");
                }
                else if (Ship.Modules[i].ModuleType == Module_Types.Cryo)
                {
                    CargoValues[i].AddThemeColorOverride("font_color", CoreData.Yellow);

                    if (Ship.Modules[i].StaffStored != null)
                        CargoValues[i].Text = Ship.Modules[i].StaffStored.Type.ToScreenString();
                    else
                        CargoValues[i].Text = "Empty";

                    Modules[i].Visible = true;
                    Modules[i].TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "Ship_Module_Cryo.png");
                }
            }

            if (Ship.ShipType == Ship_Types.Shuttle)
            {
                CourseText.Text = "";
                CourseValue.Text = "";
            }
            else
            {
                CourseValue.Text = Ship.PlanetLocation.ToScreenString() + " To\n" + Ship.DestinationPlanetLocation.ToScreenString();
            }

            var currentDay = GameCore.SingletonInstance.GameData.CurrentDay + Ship.TravelTimeRemain();
            var curDay = (currentDay % 1000).ToString().PadLeft(3, '0');
            var outputYear = (3100 + Math.Floor((decimal)(currentDay / 1000))) + " " + curDay + ".00";

            ETA.Text = "ETA\n" + outputYear;

            if (Ship.ShipType == Ship_Types.Shuttle)
            {
                LandingBlank.Visible = false;
                EngineControls.Visible = false;
            }
            else
            {
                LandingBlank.Visible = true;
                EngineControls.Visible = true;
            }

            if (Ship.LocationView)
            {
                BigLocation.Visible = true;
                TextLayout.Visible = false;

                if (Ship.ShipState == Ship_States.Docked)
                    BigLocation.Texture = SpriteManager.LoadImage(SpriteBasePath + "BigLocation_Docked.png");
                else if (Ship.ShipState == Ship_States.InTransit)
                    BigLocation.Texture = SpriteManager.LoadImage(SpriteBasePath + "BigLocation_Travel.png");
                else if (Ship.ShipState == Ship_States.Launching)
                    BigLocation.Texture = SpriteManager.LoadImage(SpriteBasePath + "SmallLocation_StormDoors.png");
                else if (Ship.ShipState == Ship_States.TakingOff)
                    BigLocation.Texture = null;
                else if (Ship.ShipState == Ship_States.Landing)
                    BigLocation.Texture = null;
                else
                    BigLocation.Texture = null;
            }
            else
            {
                BigLocation.Visible = false;
                TextLayout.Visible = true;

                if (Ship.ShipState == Ship_States.Docked)
                    SmallLocation.TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "SmallLocation_Docked.png");
                else if (Ship.ShipState == Ship_States.InTransit)
                    SmallLocation.TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "SmallLocation_Travel.png");
                else if (Ship.ShipState == Ship_States.Launching)
                    SmallLocation.TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "SmallLocation_StormDoors.png");
                else if (Ship.ShipState == Ship_States.TakingOff)
                    SmallLocation.TextureNormal = null;
                else if (Ship.ShipState == Ship_States.Landing)
                    SmallLocation.TextureNormal = null;
                else if (Ship.ShipState == Ship_States.UnDocked || Ship.ShipState == Ship_States.Docking)
                    SmallLocation.TextureNormal = SpriteManager.LoadImage(SpriteBasePath + "SmallLocation_Planet_" + CurrentPlanet.PlanetColor.ToString() + ((CurrentPlanet != null && CurrentPlanet.Station.BuildParts > 0) ? "_Station" : "") + ".png");
                else
                    SmallLocation.TextureNormal = null;
            }

            SetCourse.Visible = Ship.ShipType != Ship_Types.Shuttle; 

            //TODO - Set images
            //Click events for modules
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Right && mb.Pressed)
            {
                var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");

                if (cursor.IsLocked)
                {
                    //We were locked into the star map, let's read the new destination
                    if (DestinationStarMap != null && DestinationStarMap.Visible == true)
                    {
                        DestinationStarMap.Visible = false;

                        var newDestination = GameCore.SingletonInstance.GameData.Planets[DestinationStarMap.CurrentLocation];

                        Ship.DestinationPlanetLocation = newDestination.PlanetId;
                        Ship.DestinationStarLocation = newDestination.ParentStar;

                        StarMap.RemoveChild(DestinationStarMap);

                        UpdateState();
                    }

                    //We were locked into the star map, let's read the new destination
                    if (ACCScreen != null && ACCScreen.Visible == true)
                    {
                        ACCScreen.Visible = false;

                        ACC.RemoveChild(ACCScreen);

                        UpdateState();
                    }

                    cursor.Unlock();

                    GetViewport().SetInputAsHandled();

                    UpdateState();
                }
            }
        }

        //Triggered from gamecore
        protected override void DayTick(uint previousDay, uint currentDay)
        {
            UpdateState();
        }

        #region Statics

        public static void UpdateShips(uint previousDay, uint currentDay)
        {
            foreach (var ship in GameCore.SingletonInstance.GameData.Ships)
            {
                if (ship.ShipState != Ship_States.Docked || ship.ShipState != Ship_States.UnDocked)
                {
                    if (ship.ShipState == Ship_States.Launching)
                    {
                        ship.ShipState = Ship_States.UnDocked;
                    }
                    else if (ship.ShipState == Ship_States.TakingOff)
                    {
                        if (ship.TravelTimeRemain() == 0)
                            ship.ShipState = Ship_States.UnDocked;
                    }
                    else if (ship.ShipState == Ship_States.Landing)
                    {
                        if (ship.TravelTimeRemain() == 0)
                        {
                            ship.ShipState = Ship_States.Docked;
                            ((Shuttle)ship).OnGround = true;
                        }
                    }
                    else if (ship.ShipState == Ship_States.InTransit)
                    {
                        if (ship.TravelTimeRemain() == 0)
                        {
                            ship.ShipState = Ship_States.UnDocked;
                            ship.PlanetLocation = ship.DestinationPlanetLocation;
                            ship.StarLocation = ship.DestinationStarLocation;
                        }
                            
                    }
                    else if (ship.ShipState == Ship_States.Docking)
                    {
                        if (ship.ShipType == Ship_Types.Shuttle || !GameCore.SingletonInstance.GameData.Ships.Any(T => T.PlanetLocation == ship.PlanetLocation && T.ShipType != Ship_Types.Shuttle && T.ShipState == Ship_States.Docked))
                            ship.ShipState = Ship_States.Docked;
                    }
                }
            }
        }

        #endregion
    }
}