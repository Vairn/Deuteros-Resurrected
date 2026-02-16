using Deuteros.Code.Platform.Helpers;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.IO;
using Deuteros.Code.Platform.Base;
using System.Linq;
using Deuteros.Code.Platform.Screens;
using Deuteros.Code.Utility;
using System.Diagnostics;

namespace Deuteros.Code
{
    public partial class GameCore : BaseSubScene
    {
        private List<Objects.MenuButton> earthMenuButtons
        {
            get
            {
                return new List<Objects.MenuButton>()
                {
                    new Objects.MenuButton(Enums.Menu_Buttons.Production, Enums.Scenes.Production, true, new List<Enums.SceneVariables>() { Enums.SceneVariables.Ground }),
                    null,
                    GameCore.SingletonInstance.GameData.Unlocks.Contains(Enums.Game_Unlocks.Shuttle_Unlock) ?
                    new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.Shuttle, true,
                        new List<Enums.SceneVariables>() { 
                            Enums.SceneVariables.Ground 
                        })
                        : null,
                    new Objects.MenuButton(Enums.Menu_Buttons.Training, Enums.Scenes.Earth_Training, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    }),
                    new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground,
                        Enums.SceneVariables.Shuttle
                    }),
                    null,
                    null,
                    null,
                    null,
                    new Objects.MenuButton(Enums.Menu_Buttons.Research, Enums.Scenes.Earth_Research, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    }),
                    new Objects.MenuButton(Enums.Menu_Buttons.GroundMaterials, Enums.Scenes.GroundMaterials, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    }),
                    new Objects.MenuButton(Enums.Menu_Buttons.Store, Enums.Scenes.Store, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    })
                };
            }
        }

        private static GameCore _instance;
        private Node _currentScreen;
        private MainMenu _menuScreen;
        private InputBlocker _screenLocker;
        private int _lockCount;

        public static GameCore SingletonInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GD.Load<Node>("/root/GameCore") as GameCore;

                    if (_instance == null)
                    {
                        GD.PushError("GameCore not found! Did you forget to AutoLoad it?");
                    }
                }

                return _instance;
            }
        }

        public CoreData GameData { get; set; }

        public static string HoverText { get; set; }

        public Guid ShipSelected { get; set; }

        //Data stored in the GameCore is temporary
        public delegate void DayPassedDelegate(uint currentDay, uint nextDay);
        public event DayPassedDelegate DayPassed;

        public delegate void PlanetChangedDelegate(Objects.Interfaces.IPlanet newPlanet);
        public event PlanetChangedDelegate PlanetChanged;

        public delegate void ProductionFinishedDelegate(Objects.Factory factory);
        public event ProductionFinishedDelegate ProductionFinished;

        public delegate void ResearchFinishedDelegate(Objects.ResearchItem researchItem);
        public event ResearchFinishedDelegate ResearchFinished;

        public GameCore()
        {
            var fontLoadLabel = new Label();
            DefaultFont = fontLoadLabel.GetThemeFont("");
            fontLoadLabel.QueueFree();
            HoverText = "";
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            // Assign instance when the node is ready
            if (_instance == null)
                _instance = this;

            SpriteManager.ImageCache = new Godot.Collections.Dictionary<string, Texture2D>();

            GameData = CoreData.CreateNewGameFile();

            _screenLocker = GetNode<InputBlocker>("/root/Master/InputBlocker");

            Deuteros.Code.GameCore.SingletonInstance.DayPassed += Code.Platform.Screens.Production.UpdateProduction;

            Input.MouseMode = Input.MouseModeEnum.Hidden;

            ChangeScene("IntroScreen.tscn", new List<Enums.SceneVariables>());
        }

        private void TriggerDay(uint currentDay, uint nextDay)
        {
            DayPassed?.Invoke(currentDay, nextDay);
        }

        private void TriggerPlanetChange(Objects.Interfaces.IPlanet newPlanet)
        {
            PlanetChanged?.Invoke(newPlanet);
        }

        public void TriggerProductionFinished(Objects.Factory factory)
        {
            ProductionFinished?.Invoke(factory);
        }

        public void TriggerResearchFinished(Objects.ResearchItem researchItem)
        {
            ResearchFinished?.Invoke(researchItem);
        }

        public Deuteros.Code.Objects.Interfaces.IPlanet GetCurrentPlanet()
        {
            return GameData.Planets[GameData.CurrentPlanet];
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            if (GameData.TimeSkip || GameData.TimeSkipDay)
            {
                if (GameData.TimeSkipDay || Time.GetTicksMsec() - GameData.TimeSkipStart > 999)
                {
                    TriggerDay(GameData.CurrentDay, GameData.CurrentDay + 1);

                    GameData.CurrentDay++;
                    GameData.TimeSkipStart = Time.GetTicksMsec();
                    GameData.TimeSkipDay = false;

                    QueueRedraw();
                }
            }
        }
        public void ChangeScene(string sceneName, Guid shipRef, List<Enums.SceneVariables> sceneVariables)
        {
            ShipSelected = shipRef;
            ChangeScene(sceneName, sceneVariables);
        }

        public void ChangeScene(string sceneName, List<Enums.SceneVariables> sceneVariables)
        {
            if (_currentScreen != null && _currentScreen.SceneFilePath.Contains("IntroScreen"))
            {
                var newMenuScene = GD.Load<PackedScene>("res://Screens/Base/MenuBase.tscn").Instantiate<MainMenu>();
                GetNode<Node>("/root/Master/MainScene").AddChild(newMenuScene);
                _menuScreen = newMenuScene;
            }

            if (_currentScreen != null)
            {
                _currentScreen.QueueFree();
            }

            var newScene = GD.Load<PackedScene>("res://Screens/" + sceneName).Instantiate<BaseSubScene>();
            newScene.SceneVariables = sceneVariables;
            GetNode<Node>("/root/Master/MainScene").AddChild(newScene);
            _currentScreen = newScene;

            if (_menuScreen != null && GetCurrentPlanet().PlanetId == Enums.StellarBodies.earth)
            {
                _menuScreen.MenuButtons = earthMenuButtons;
                _menuScreen.SetupMenus();
            }
            else if (newScene.MenuButtons != null)
            {
                _menuScreen.MenuButtons = newScene.MenuButtons;
                _menuScreen.SetupMenus();
            }

            ShipSelected = Guid.Empty;
        }

        public static void LockScreen(string lockMessage = "")
        {
            lock (SingletonInstance._screenLocker)
            {
                SingletonInstance._lockCount++;
                SingletonInstance._screenLocker.SetBlocked(true);

                GD.PushWarning("Screen locked (" + lockMessage + ") Count " + SingletonInstance._lockCount.ToString());
            }
        }

        public static void UnLockScreen()
        {
            lock(SingletonInstance._screenLocker)
            {
                SingletonInstance._lockCount = Math.Max(0, SingletonInstance._lockCount - 1);

                if (SingletonInstance._lockCount == 0)
                {
                    SingletonInstance._screenLocker.SetBlocked(false);
                    GD.PushWarning("Screen unlocked");
                }
                else
                {
                    GD.PushWarning("Screen unlock denied Count " + SingletonInstance._lockCount.ToString());
                }
            }
        }

        public static PlanetType GetPlanet<PlanetType>(Enums.StellarBodies planet)
        {
            return (PlanetType)SingletonInstance.GameData.Planets[planet];
        }


        public static Earth Earth
        {
            get
            {
                return (Earth)SingletonInstance.GameData.Planets[Enums.StellarBodies.earth];
            }

            set
            {
                SingletonInstance.GameData.Planets[Enums.StellarBodies.earth] = value;
            }
        }
    }
}
