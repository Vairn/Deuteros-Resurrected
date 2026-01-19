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
    public partial class ShipBay : BaseSubScene
    {
        public const string NavSpriteBasePath = "res://Sprites//Buttons//Shipbay//";
        public Color SelectedCargoColor { get; set; } = new Color(255, 0, 0, 255);
        public bool Ground { get; set; }
        public bool Earth { get; set; }
        public bool Shuttle { get; set; }
        public bool ShipPresent { get; set; }
        public IShip Ship { get; set; }
        public IPlanet CurrentPlanet { get; set; }
        TextureButton Nav_Cockpit { get; set; }
        List<TextureButton> Nav_Torsos { get; set; }
        TextureButton Nav_Engine { get; set; }
        TextureButton Nav_Dismantle { get; set; }
        TextureButton Nav_Create_Shuttle { get; set; }
        TextureButton Nav_Create_IOS { get; set; }
        TextureButton Nav_Create_SCG { get; set; }

        Control CargoService { get; set; }
        Control EquipmentStock { get; set; }
        Control StaffList { get; set; }

        StaffList TorsoStaffList { get; set; }

        ShipBayScenes.Cockpit CockpitInstance { get; set; }
        List<ShipBayScenes.Torso> TorsoInstances { get; set; }
        ShipBayScenes.Engine EngineInstance { get; set; }

        public ScrollContainer ScrollContainer;
        public int ScreenWidth = 224;

        public Resource resourceList { get; set; }

        public int ScreenState { get; set; }

        public override void _Ready()
        {
            ScrollContainer = GetNode<ScrollContainer>("ShipContainer/ScrollContainer2");

            //Setup some flags to make our lives easier
            CurrentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();
            Earth = CurrentPlanet.PlanetId == Enums.Planetoids.earth;
            Ground = SceneVariables.Contains(Enums.SceneVariables.Ground);
            Shuttle = SceneVariables.Contains(Enums.SceneVariables.Shuttle);

            resourceList = (Resource)(Ground ? CurrentPlanet.PlanetResources : CurrentPlanet.Station.Resources);

            CockpitInstance = GetNode<ShipBayScenes.Cockpit>("ShipContainer/ScrollContainer2/HBoxContainer/Cockpit");

            TorsoStaffList = GetNode<StaffList>("StaffList");

            TorsoInstances = new List<ShipBayScenes.Torso>();
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso1"));
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso2"));
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso3"));
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso4"));
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso5"));
            TorsoInstances.Add(GetNode<ShipBayScenes.Torso>("ShipContainer/ScrollContainer2/HBoxContainer/Torso6"));

            EngineInstance = GetNode<ShipBayScenes.Engine>("ShipContainer/ScrollContainer2/HBoxContainer/Engine");

            CargoService = GetNode<Control>("CargoService");
            EquipmentStock = GetNode<Control>("EquipmentStock");
            StaffList = GetNode<Control>("StaffList");

            CockpitInstance.StaffList.PilotChanged += CockpitInstance_PilotChanged;
            CockpitInstance.StaffList.ProductionChanged += CockpitInstance_ProductionChanged;

            TorsoStaffList.StaffClicked += TorsoStaffList_StaffClicked;

            EngineInstance.EngineInstalled += EngineInstance_EngineInstalled;

            TorsoInstances[0].ModuleChanged += ShipBay_ModuleChanged;
            TorsoInstances[0].ModuleOpened += ShipBay_ModuleOpened;
            TorsoInstances[1].ModuleChanged += ShipBay_ModuleChanged;
            TorsoInstances[1].ModuleOpened += ShipBay_ModuleOpened;
            TorsoInstances[2].ModuleChanged += ShipBay_ModuleChanged;
            TorsoInstances[2].ModuleOpened += ShipBay_ModuleOpened;
            TorsoInstances[3].ModuleChanged += ShipBay_ModuleChanged;
            TorsoInstances[3].ModuleOpened += ShipBay_ModuleOpened;
            TorsoInstances[4].ModuleChanged += ShipBay_ModuleChanged;
            TorsoInstances[4].ModuleOpened += ShipBay_ModuleOpened;

            foreach (var mineral in GameCore.SingletonInstance.GameData.ItemList.Where(T => T.ItemCategory == ItemCategory.resource))
                GetNode<Button>("CargoService/Buttons/" + mineral.ItemType.ToScreenString()).Pressed += () => SelectMineral(mineral.ItemType);

            CargoService.Visible = false;
            EquipmentStock.Visible = false;
            StaffList.Visible = false;

            ScreenState = GetScreenState();

            ScrollToScreen();

            LoadButtons();

            UpdateState();

            RefreshButtons();

            base._Ready();
        }

        public void LoadButtons()
        {
            Nav_Cockpit = GetNode<TextureButton>("Buttons/ShipNav/Nav_Cockpit");
            Nav_Torsos = new List<TextureButton>();
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso1"));
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso2"));
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso3"));
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso4"));
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso5"));
            Nav_Torsos.Add(GetNode<TextureButton>("Buttons/ShipNav/Nav_Torso6"));
            Nav_Engine = GetNode<TextureButton>("Buttons/ShipNav/Nav_Engine");

            Nav_Dismantle = GetNode<TextureButton>("Buttons/Nav_Dismantle");
            Nav_Create_Shuttle = GetNode<TextureButton>("Buttons/Nav_Create_Shuttle");
            Nav_Create_IOS = GetNode<TextureButton>("Buttons/Nav_Create_IOS");
            Nav_Create_SCG = GetNode<TextureButton>("Buttons/Nav_Create_SCG");

            Nav_Cockpit.Pressed += NavCockpitPressed;

            Nav_Torsos[0].Pressed += () => NavTorsoPressed(1);
            Nav_Torsos[1].Pressed += () => NavTorsoPressed(2);
            Nav_Torsos[2].Pressed += () => NavTorsoPressed(3);
            Nav_Torsos[3].Pressed += () => NavTorsoPressed(4);
            Nav_Torsos[4].Pressed += () => NavTorsoPressed(5);
            Nav_Torsos[5].Pressed += () => NavTorsoPressed(6);

            Nav_Engine.Pressed += NavEnginePressed;

            Nav_Create_Shuttle.Pressed += CreateShuttle;
            Nav_Create_IOS.Pressed += CreateIOS;
            Nav_Create_SCG.Pressed += CreateSCG;
            Nav_Dismantle.Pressed += DismantleShip;
        }

        private void DismantleShip()
        {
            Ship = GameCore.SingletonInstance.GameData.Ships.Single(T => T.PlanetLocation == CurrentPlanet.PlanetId && T.Docked &&
                    ((T.ShipType == Enums.Ship_Types.Shuttle && Ground) || (T.ShipType != Enums.Ship_Types.Shuttle && !Ground)));

            GameCore.SingletonInstance.GameData.Ships.Remove(Ship);

            UpdateState();
            RefreshButtons();
        }

        private void CreateShuttle()
        {
            if ((Ground && CurrentPlanet.PlanetResources.Stores[Enums.ItemTypes.s_chassis] > 0) || (!Ground && CurrentPlanet.Station.Resources.Stores[Enums.ItemTypes.s_chassis] > 0))
            {
                var newShuttle = new Shuttle();
                newShuttle.StartTravelDay = 0;
                newShuttle.StarLocation = CurrentPlanet.ParentStar;
                newShuttle.Modules = new List<ShipModule>();
                newShuttle.Modules.Add(new ShipModule());
                newShuttle.Docked = true;
                newShuttle.Fuel = 0;
                newShuttle.Engine = false;
                newShuttle.FuelType = Enums.Fuel_Types.MEH_Fuel;
                newShuttle.OnGround = Earth;
                newShuttle.Pilot = null;
                newShuttle.PlanetLocation = CurrentPlanet.PlanetId;
                newShuttle.ShipType = Enums.Ship_Types.Shuttle;

                GameCore.SingletonInstance.GameData.Ships.Add(newShuttle);

                UpdateState();
                RefreshButtons();
            }
        }

        private void CreateIOS()
        {
            if ((Ground && CurrentPlanet.PlanetResources.Stores[Enums.ItemTypes.i_chassis] > 0) || (!Ground && CurrentPlanet.Station.Resources.Stores[Enums.ItemTypes.i_chassis] > 0))
            {
                var newIOS = new IOS();
                newIOS.StartTravelDay = 0;
                newIOS.StarLocation = CurrentPlanet.ParentStar;
                newIOS.Modules = new List<ShipModule>();
                newIOS.Modules.AddRange(Enumerable.Range(0, 3).Select(_ => new ShipModule()));
                newIOS.Docked = true;
                newIOS.Fuel = 0;
                newIOS.Engine = false;
                newIOS.FuelType = Enums.Fuel_Types.MEH_Fuel;
                newIOS.Pilot = null;
                newIOS.PlanetLocation = CurrentPlanet.PlanetId;
                newIOS.ShipType = Enums.Ship_Types.Shuttle;

                GameCore.SingletonInstance.GameData.Ships.Add(newIOS);

                UpdateState();
                RefreshButtons();
            }
        }

        private void CreateSCG()
        {
            if ((Ground && CurrentPlanet.PlanetResources.Stores[Enums.ItemTypes.g_chassis] > 0) || (!Ground && CurrentPlanet.Station.Resources.Stores[Enums.ItemTypes.g_chassis] > 0))
            {
                var newSCG = new SCG();
                newSCG.StartTravelDay = 0;
                newSCG.StarLocation = CurrentPlanet.ParentStar;
                newSCG.Modules = new List<ShipModule>();
                newSCG.Modules.AddRange(Enumerable.Range(0, 5).Select(_ => new ShipModule()));
                newSCG.Docked = true;
                newSCG.Fuel = 0;
                newSCG.Engine = false;
                newSCG.FuelType = Enums.Fuel_Types.MEH_Fuel;
                newSCG.Pilot = null;
                newSCG.PlanetLocation = CurrentPlanet.PlanetId;
                newSCG.ShipType = Enums.Ship_Types.Shuttle;

                GameCore.SingletonInstance.GameData.Ships.Add(newSCG);

                UpdateState();
                RefreshButtons();
            }
        }

        private int GetScreenState()
        {
            var returnState = 0;

            if (Ground && !Earth)
                returnState = CurrentPlanet.ShuttleState;
            else if (Ground && Earth)
                returnState = ((Earth)CurrentPlanet).ShuttleState;
            else if (!Ground && CurrentPlanet.Station != null && CurrentPlanet.Station.Built && Shuttle)
                returnState = CurrentPlanet.Station.ShuttleState;
            else if (!Ground && CurrentPlanet.Station != null && CurrentPlanet.Station.Built && !Shuttle)
                returnState = CurrentPlanet.Station.StarShipState;

            return returnState;
        }

        private void UpdateScreenState(int newScreenState)
        {
            if (Ground && Earth)
                ((Earth)CurrentPlanet).ShuttleState = newScreenState;
            if (Ground && !Earth)
                CurrentPlanet.ShuttleState = newScreenState;
            else if (!Ground && CurrentPlanet.Station != null && CurrentPlanet.Station.Built && Shuttle)
                CurrentPlanet.Station.ShuttleState = newScreenState;
            else if (!Ground && CurrentPlanet.Station != null && CurrentPlanet.Station.Built && !Shuttle)
                CurrentPlanet.Station.StarShipState = newScreenState;
        }

        private void NavCockpitPressed()
        {
            if (ScreenState != 0)
            {
                ScreenState = 0;
                ScrollToScreen();
                UpdateScreenState(ScreenState);
                RefreshButtons();
            }
        }

        private void NavTorsoPressed(int torsoId)
        {
            if (ScreenState != torsoId)
            {
                ScreenState = torsoId;
                ScrollToScreen();
                UpdateScreenState(ScreenState);

                RefreshButtons();
            }
        }

        private void NavEnginePressed()
        {
            if (ScreenState != 7)
            {
                ScreenState = 7;
                ScrollToScreen();
                UpdateScreenState(ScreenState);
                RefreshButtons();
            }
        }

        private void ScrollToScreen()
        {
            //TODO - Does not work backwards
            int targetScrollX = ScreenState * ScreenWidth;

            // Smooth scrolling
            var tween = GetTree().CreateTween();

            // Bounce distances
            float bounce1 = 20f;
            float bounce2 = 8f;

            // Scroll limits
            float minScroll = 0f;
            float maxScroll = (float)ScrollContainer.GetHScrollBar().MaxValue;

            // Clamp target within scroll bounds
            float target = Mathf.Clamp(targetScrollX, minScroll, maxScroll);

            // Get current scroll position
            float current = ScrollContainer.ScrollHorizontal;

            // Direction: +1 if scrolling forward (increasing), -1 if backward
            int direction = (target > current) ? 1 : -1;

            // Compute bounce positions (moving *away* from target)
            float bounceTarget1 = Mathf.Clamp(target - (bounce1 * direction), minScroll, maxScroll);
            float bounceTarget2 = Mathf.Clamp(target - (bounce2 * direction), minScroll, maxScroll);

            // Begin scroll
            tween.TweenProperty(ScrollContainer, "scroll_horizontal", target, 0.6)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.In);

            // First bounce
            if (!Mathf.IsEqualApprox(bounceTarget1, target))
            {
                tween.TweenProperty(ScrollContainer, "scroll_horizontal", bounceTarget1, 0.12)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.Out);

                tween.TweenProperty(ScrollContainer, "scroll_horizontal", target, 0.12)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);
            }

            // Second, smaller bounce
            if (!Mathf.IsEqualApprox(bounceTarget2, target))
            {
                tween.TweenProperty(ScrollContainer, "scroll_horizontal", bounceTarget2, 0.07)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.Out);

                tween.TweenProperty(ScrollContainer, "scroll_horizontal", target, 0.07)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);
            }
        }

        private void UpdateState()
        {
            //Detect if there is a ship present
            ShipPresent = GameCore.SingletonInstance.GameData.Ships.Any(T => T.PlanetLocation == CurrentPlanet.PlanetId && T.Docked &&
            ((T.ShipType == Enums.Ship_Types.Shuttle && Ground) || (T.ShipType != Enums.Ship_Types.Shuttle && !Ground)));

            CockpitInstance.UpdateStaff(Ground ? CurrentPlanet.PlanetResources.Staff : CurrentPlanet.Station.Resources.Staff);
            TorsoStaffList.UpdateStaff(Ground ? CurrentPlanet.PlanetResources.Staff : CurrentPlanet.Station.Resources.Staff);

            //There is no ship, reset buttons and reset state
            if (!ShipPresent)
            {
                Nav_Dismantle.Visible = false;
                Nav_Cockpit.Visible = false;
                Nav_Torsos.ForEach(T => T.Visible = false);
                Nav_Engine.Visible = false;

                CockpitInstance.LoadShip(null);
                TorsoStaffList.UpdateShip(false);

                TorsoInstances.ForEach(T => T.SpriteHolder.Visible = false);
                EngineInstance.SpriteHolder.Visible = false;

                ScrollContainer.ScrollHorizontal = 0;

                if (Shuttle)
                {
                    Nav_Create_Shuttle.Visible = true;
                    Nav_Create_IOS.Visible = false;
                    Nav_Create_SCG.Visible = false;
                }
                else
                {
                    Nav_Create_Shuttle.Visible = false;
                    Nav_Create_IOS.Visible = true;
                    //TODO - Need to be passed the point of having SVGs available
                    Nav_Create_SCG.Visible = true;
                }
            }
            else
            {
                Nav_Create_Shuttle.Visible = false;
                Nav_Create_IOS.Visible = false;
                Nav_Create_SCG.Visible = false;

                Nav_Dismantle.Visible = true;
                Nav_Cockpit.Visible = true;
                Nav_Engine.Visible = true;

                EngineInstance.SpriteHolder.Visible = true;

                Ship = GameCore.SingletonInstance.GameData.Ships.Single(T => T.PlanetLocation == CurrentPlanet.PlanetId && T.Docked &&
                ((T.ShipType == Enums.Ship_Types.Shuttle && Ground) || (T.ShipType != Enums.Ship_Types.Shuttle && !Ground)));

                CockpitInstance.LoadShip(Ship);
                TorsoStaffList.UpdateShip(Ship != null);

                Nav_Torsos.ForEach(T => T.Visible = false);
                TorsoInstances.ForEach(T => T.Visible = false);
                TorsoInstances.ForEach(T => T.SpriteHolder.Visible = false);

                EngineInstance.Installed = Ship.Engine;
                EngineInstance.UpdateState();

                if (Ship.ShipType == Enums.Ship_Types.Shuttle)
                {
                    Nav_Torsos[0].Visible = true;
                    TorsoInstances[0].Visible = true;
                    TorsoInstances[0].SpriteHolder.Visible = true;

                    TorsoInstances[0].ChangeModule(Ship.Modules[0]);
                    TorsoInstances[0].TorsoSection = 0;
                    TorsoInstances[0].UpdateState();
                }
                else if (Ship.ShipType == Enums.Ship_Types.IOS)
                {
                    Nav_Torsos[0].Visible = true;
                    Nav_Torsos[1].Visible = true;
                    Nav_Torsos[2].Visible = true;
                    TorsoInstances[0].Visible = true;
                    TorsoInstances[1].Visible = true;
                    TorsoInstances[2].Visible = true;
                    TorsoInstances[0].SpriteHolder.Visible = true;
                    TorsoInstances[1].SpriteHolder.Visible = true;
                    TorsoInstances[2].SpriteHolder.Visible = true;

                    TorsoInstances[0].ChangeModule(Ship.Modules[0]);
                    TorsoInstances[0].TorsoSection = 0;
                    TorsoInstances[0].UpdateState();

                    TorsoInstances[1].ChangeModule(Ship.Modules[1]);
                    TorsoInstances[1].TorsoSection = 1;
                    TorsoInstances[1].UpdateState();

                    TorsoInstances[2].ChangeModule(Ship.Modules[2]);
                    TorsoInstances[2].TorsoSection = 2;
                    TorsoInstances[2].UpdateState();
                }
                else if (Ship.ShipType == Enums.Ship_Types.SCG)
                {
                    Nav_Torsos[0].Visible = true;
                    Nav_Torsos[1].Visible = true;
                    Nav_Torsos[2].Visible = true;
                    Nav_Torsos[3].Visible = true;
                    Nav_Torsos[4].Visible = true;
                    Nav_Torsos[5].Visible = true;
                    TorsoInstances[0].Visible = true;
                    TorsoInstances[1].Visible = true;
                    TorsoInstances[2].Visible = true;
                    TorsoInstances[3].Visible = true;
                    TorsoInstances[4].Visible = true;
                    TorsoInstances[5].Visible = true;
                    TorsoInstances[0].SpriteHolder.Visible = true;
                    TorsoInstances[1].SpriteHolder.Visible = true;
                    TorsoInstances[2].SpriteHolder.Visible = true;
                    TorsoInstances[3].SpriteHolder.Visible = true;
                    TorsoInstances[4].SpriteHolder.Visible = true;
                    TorsoInstances[5].SpriteHolder.Visible = true;

                    TorsoInstances[0].ChangeModule(Ship.Modules[0]);
                    TorsoInstances[0].TorsoSection = 0;
                    TorsoInstances[0].UpdateState();

                    TorsoInstances[1].ChangeModule(Ship.Modules[1]);
                    TorsoInstances[1].TorsoSection = 1;
                    TorsoInstances[1].UpdateState();

                    TorsoInstances[2].ChangeModule(Ship.Modules[2]);
                    TorsoInstances[2].TorsoSection = 2;
                    TorsoInstances[2].UpdateState();

                    TorsoInstances[3].ChangeModule(Ship.Modules[3]);
                    TorsoInstances[3].TorsoSection = 3;
                    TorsoInstances[3].UpdateState();

                    TorsoInstances[4].ChangeModule(Ship.Modules[4]);
                    TorsoInstances[4].TorsoSection = 4;
                    TorsoInstances[4].UpdateState();
                }
            }
        }

        private Staff[] CockpitInstance_ProductionChanged(Staff staff)
        {
            if (Ground && CurrentPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)CurrentPlanet).Factory.Builder == null)
            {
                ((Earth)CurrentPlanet).Factory.Builder = staff;
                ((Earth)CurrentPlanet).PlanetResources.RemoveStaff(staff);
                return ((Earth)CurrentPlanet).PlanetResources.Staff;
            }
            else if (!Ground && !CurrentPlanet.Station.Factory.AOC && CurrentPlanet.Station.Factory.Builder == null)
            {
                CurrentPlanet.Station.Factory.Builder = staff;
                CurrentPlanet.Station.Resources.RemoveStaff(staff);
                return CurrentPlanet.Station.Resources.Staff;
            }
            else
            {
                return CurrentPlanet.Station.Resources.Staff;
            }
        }

        private Staff[] CockpitInstance_PilotChanged(Staff staff)
        {
            //Detect if there is a ship present
            if (Ship != null)
            {
                if (staff != null && Ship.Pilot != null)
                    Ship.Pilot = resourceList.SwapStaff(staff, Ship.Pilot);

                if (staff != null && Ship.Pilot == null)
                {
                    Ship.Pilot = staff;
                    resourceList.RemoveStaff(staff);
                }

                if (staff == null && Ship.Pilot != null)
                {
                    resourceList.AddStaff(Ship.Pilot);
                    Ship.Pilot = null;
                }

                CockpitInstance.UpdateState();
            }

            return resourceList.Staff;
        }

        private Staff[] TorsoStaffList_StaffClicked(Staff staff)
        {
            if (staff != null && Ship.Modules[ScreenState - 1].StaffStored != null)
            {
                Ship.Modules[ScreenState - 1].StaffStored = resourceList.SwapStaff(staff, Ship.Modules[ScreenState - 1].StaffStored);
            }
            else if (staff == null && Ship.Modules[ScreenState - 1].StaffStored != null)
            {
                resourceList.AddStaff(Ship.Modules[ScreenState - 1].StaffStored);
                Ship.Modules[ScreenState - 1].StaffStored = null;
            }
            else if (staff != null && Ship.Modules[ScreenState - 1].StaffStored == null)
            {
                Ship.Modules[ScreenState - 1].StaffStored = staff;
                resourceList.RemoveStaff(staff);
            }

            TorsoStaffList.UpdateState();
            CockpitInstance.UpdateStaff(resourceList.Staff);
            CockpitInstance.UpdateState();

            TorsoInstances[ScreenState - 1].UpdateState();

            return resourceList.Staff;
        }

        private bool ShipBay_ModuleChanged(Enums.Module_Types moduleType, int torsoSection)
        {
            var currentModule = Ship.Modules[torsoSection];
            if (currentModule.ModuleType == Enums.Module_Types.Supply && currentModule.ItemCount > 0)
                return false;
            else if (currentModule.ModuleType == Enums.Module_Types.Tool && currentModule.ItemStored != Enums.ItemTypes.none)
                return false;
            else if (currentModule.ModuleType == Enums.Module_Types.Cryo && currentModule.StaffStored != null)
                return false;
            else
                Ship.Modules[torsoSection].ModuleType = moduleType;

            return true;
        }

        private void ShipBay_ModuleOpened(int torsoSection)
        {
            var currentModule = Ship.Modules[torsoSection];

            var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");
            Rect2 rect = new Rect2();

            if (currentModule.ModuleType == Enums.Module_Types.Supply)
            {
                UpdateCargoService();

                CargoService.Visible = true;

                rect = CargoService.GetGlobalRect();
            }
            else if (currentModule.ModuleType == Enums.Module_Types.Tool)
            {
                EquipmentStock.Visible = true;

                rect = EquipmentStock.GetGlobalRect();
            }
            else if (currentModule.ModuleType == Enums.Module_Types.Cryo)
            {
                TorsoStaffList.UpdateState();

                StaffList.Visible = true;

                rect = StaffList.GetGlobalRect();
            }

            cursor.LockToRect(rect);
        }

        private void EngineInstance_EngineInstalled()
        {
            Ship.Engine = true;
        }

        #region CargoService

        private void UpdateCargoService()
        {
            if (Ship.Modules[ScreenState - 1].ItemCount > 0)
                GetNode<Label>("CargoService/Labels/MineralName" + Ship.Modules[ScreenState - 1].ItemStored.ToScreenString()).AddThemeColorOverride("font_color", SelectedCargoColor);

            foreach (var mineral in GameCore.SingletonInstance.GameData.ItemList.Where(T => T.ItemCategory == ItemCategory.resource))
            {
                GetNode<Label>("CargoService/Labels/MineralCount" + mineral.ItemType.ToScreenString()).Text = resourceList.Stores[mineral.ItemType].ToString();
            }
        }

        private void SelectMineral(ItemTypes itemType)
        {
            if (Ship.Modules[ScreenState - 1].ItemStored != ItemTypes.none && Ship.Modules[ScreenState - 1].ItemCount > 0)
            {
                resourceList.Stores[Ship.Modules[ScreenState - 1].ItemStored] += Ship.Modules[ScreenState - 1].ItemCount;
                GetNode<Label>("CargoService/Labels/MineralName" + Ship.Modules[ScreenState - 1].ItemStored.ToScreenString()).RemoveThemeColorOverride("font_color");
            }

            if (Ship.Modules[ScreenState - 1].ItemStored == itemType)
            {
                Ship.Modules[ScreenState - 1].ItemStored = ItemTypes.none;
                Ship.Modules[ScreenState - 1].ItemCount = 0;
            }
            else if (Ship.Modules[ScreenState - 1].ItemStored != itemType && resourceList.Stores[itemType] > 0)
            {
                Ship.Modules[ScreenState - 1].ItemStored = itemType;
                Ship.Modules[ScreenState - 1].ItemCount = resourceList.Stores[itemType] >= 250 ? 250 : resourceList.Stores[itemType];

                resourceList.Stores[itemType] = Math.Max(0, resourceList.Stores[itemType] - 250);
            }

            TorsoInstances[ScreenState - 1].UpdateState();

            UpdateCargoService();
        }

        #endregion

        private void RefreshButtons()
        {
            Nav_Cockpit.TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Cockpit_" + (ScreenState == 0 ? "On" : "Off") + ".png");
            Nav_Torsos[0].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 1 ? "On" : "Off") + ".png");
            Nav_Torsos[1].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 2 ? "On" : "Off") + ".png");
            Nav_Torsos[2].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 3 ? "On" : "Off") + ".png");
            Nav_Torsos[3].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 4 ? "On" : "Off") + ".png");
            Nav_Torsos[4].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 5 ? "On" : "Off") + ".png");
            Nav_Torsos[5].TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Torso_" + (ScreenState == 6 ? "On" : "Off") + ".png");
            Nav_Engine.TextureNormal = SpriteManager.LoadImage(NavSpriteBasePath + "Nav_Engine_" + (ScreenState == 7 ? "On" : "Off") + ".png");
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Right && mb.Pressed)
            {
                var cursor = GetTree().CurrentScene.GetNode<GlobalInput>("VirtualCursorView");

                if (cursor.IsLocked)
                {
                    if (CargoService.Visible == true)
                        CargoService.Visible = false;
                    else if (EquipmentStock.Visible == true)
                        EquipmentStock.Visible = false;
                    else if (StaffList.Visible == true)
                        StaffList.Visible = false;

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