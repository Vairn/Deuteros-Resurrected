using Deuteros.Code.Platform.Helpers;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.IO;

namespace Deuteros.Code
{
	public partial class GameCore : Node2D
	{
        private static GameCore _instance;
        private Node _currentScreen;
        private Node _menuScreen;

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
		public static Font DefaultFont { get; set; }

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

            ChangeScene("IntroScreen.tscn");
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
		public void ChangeScene(string sceneName)
		{
			if (_currentScreen != null && _currentScreen.SceneFilePath.Contains("IntroScreen"))
			{
                var newMenuScene = GD.Load<PackedScene>("res://Screens/Base/MenuBase.tscn").Instantiate<Node>();
                GetNode<Node>("/root/Master/MainScene").AddChild(newMenuScene);
                _menuScreen = newMenuScene;
            }

            if (_currentScreen != null)
            {
                _currentScreen.QueueFree();
            }

            var newScene = GD.Load<PackedScene>("res://Screens/" + sceneName).Instantiate<Node>();
            GetNode<Node>("/root/Master/MainScene").AddChild(newScene);
            _currentScreen = newScene;
        }

		public PlanetType GetPlanet<PlanetType>(Enums.Planetoids planet)
		{
            return (PlanetType)GameData.Planets[planet];
        }

        public Earth Earth { 
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
