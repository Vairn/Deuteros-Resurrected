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
                    GameCore.SingletonInstance.GameData.CompleteStages.Contains(Enums.Game_Stages.Earth_Shuttle_Built) ?
                    new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.Shuttle, true,
                        new List<Enums.SceneVariables>() { Enums.SceneVariables.Ground })
                        : null,
                    new Objects.MenuButton(Enums.Menu_Buttons.Training, Enums.Scenes.Earth_Training, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    }),
                    new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
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

            Deuteros.Code.GameCore.SingletonInstance.DayPassed += Code.Platform.Screens.Production.UpdateProduction;

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

        //When we change scene we must re-load the menus and perform some house keeping
        //SceneFlags can be passed in to give the scene some hints on its setup
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

            //If this is earth, then the menus are fairly 
            if (_menuScreen != null && GetCurrentPlanet().PlanetId == Enums.Planetoids.earth)
            {
                _menuScreen.MenuButtons = earthMenuButtons;
                _menuScreen.SetupMenus();
            }
            else if (newScene.MenuButtons != null)
            {
                _menuScreen.MenuButtons = newScene.MenuButtons;
                _menuScreen.SetupMenus();
            }
        }

        public PlanetType GetPlanet<PlanetType>(Enums.Planetoids planet)
        {
            return (PlanetType)GameData.Planets[planet];
        }

        public Earth Earth
        {
            get
            {
                return (Earth)GameData.Planets[Enums.Planetoids.earth];
            }

            set
            {
                GameData.Planets[Enums.Planetoids.earth] = value;
            }
        }
    }
}
