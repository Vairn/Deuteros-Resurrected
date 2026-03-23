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
using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform;
using static Deuteros.Code.Enums;

namespace Deuteros.Code
{
	public partial class GameCore : BaseSubScene
	{
		#region DEBUGVARS
		
		//TODO DEBUG
		
		public bool InfiniteResources { get; set; }

		#endregion

		private List<Objects.MenuButton> EarthMenuButtons
		{
			get
			{
				return new List<Objects.MenuButton>()
				{
					new Objects.MenuButton(Enums.Menu_Buttons.Production, Enums.Scenes.Production, true, 
					new Godot.Collections.Array<Enums.SceneVariables>() { 
						Enums.SceneVariables.Ground 
					}, null),
					null,
					new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.ShipInterior, true,
						new Godot.Collections.Array<Enums.SceneVariables>() {
							Enums.SceneVariables.Ground
						}, new List<Action> { () => GameCore.SingletonInstance.ShipSelected = GameData.Ships.Single(T => T.ShipType == Enums.Ship_Types.Shuttle && T.PlanetLocation == Enums.StellarBodies.earth).ShipID },
						() => GameCore.SingletonInstance.GameData.Unlocks.Contains(Enums.Game_Unlocks.Shuttle_Unlock)),
					new Objects.MenuButton(Enums.Menu_Buttons.Training, Enums.Scenes.Earth_Training, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground,
						Enums.SceneVariables.Shuttle
					}, null),
					null,
					null,
					null,
					null,
					new Objects.MenuButton(Enums.Menu_Buttons.Research, Enums.Scenes.Earth_Research, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.GroundMaterials, Enums.Scenes.GroundMaterials, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Store, Enums.Scenes.Store, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null)
				};
			}
		}
		private List<Objects.MenuButton> EarthStationMenuButtons
		{
			get
			{
				return new List<Objects.MenuButton>()
				{
					new Objects.MenuButton(Enums.Menu_Buttons.Production, Enums.Scenes.Production, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit,
						Enums.SceneVariables.Shuttle
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.ShipInterior, true,
						new Godot.Collections.Array<Enums.SceneVariables>() {
							Enums.SceneVariables.Orbit
						}, new List<Action> { () => GameCore.SingletonInstance.ShipSelected = GameData.Ships.Single(T => T.ShipType == Enums.Ship_Types.Shuttle && T.PlanetLocation == GetCurrentPlanet().PlanetId).ShipID }, null),
					null,
					null,
					null,
					new Objects.MenuButton(Enums.Menu_Buttons.Store, Enums.Scenes.Store, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit,
						Enums.SceneVariables.Ship
					}, null),
					null,
					null,
					null,
					null
				};
			}
		}
		private List<Objects.MenuButton> StandardMenuButtons
		{
			get
			{
				return new List<Objects.MenuButton>()
				{
					new Objects.MenuButton(Enums.Menu_Buttons.Production, Enums.Scenes.Production, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Store, Enums.Scenes.Store, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Ground
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit,
						Enums.SceneVariables.Shuttle
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
					new Godot.Collections.Array<Enums.SceneVariables>() {
						Enums.SceneVariables.Orbit,
						Enums.SceneVariables.Ship
					}, null),
					new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.ShipInterior, true,
						new Godot.Collections.Array<Enums.SceneVariables>() {
							Enums.SceneVariables.Ground
						}, new List<Action> { () => GameCore.SingletonInstance.ShipSelected = GameData.Ships.Single(T => T.ShipType == Enums.Ship_Types.Shuttle && T.PlanetLocation == Enums.StellarBodies.earth).ShipID },
						() => GameCore.SingletonInstance.GameData.Unlocks.Contains(Enums.Game_Unlocks.Shuttle_Unlock)),
					null,

					null,
					null,
					null,
					null
				};
			}
		}

		private static GameCore _instance;
		private Node _currentScreen;
		private MainMenu _menuScreen;
		private Unlocker _unlocker;
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
		public delegate void DayPassedDelegate(uint previousDay, uint currentDay);
		public event DayPassedDelegate DayPassed;

		public delegate void PlanetChangedDelegate(Objects.Interfaces.IPlanet newPlanet);
		public event PlanetChangedDelegate PlanetChanged;

		public delegate void StationPiecePlacedDelegate(Enums.StellarBodies stellarBody);
		public event StationPiecePlacedDelegate StationPiecePlaced;

		public delegate void ProductionFinishedDelegate(Objects.Factory factory);
		public event ProductionFinishedDelegate ProductionFinished;

		public delegate void ResearchFinishedDelegate(Objects.ResearchItem researchItem);
		public event ResearchFinishedDelegate ResearchFinished;

		public delegate void ShipCreatedDelegate(IShip ship);
		public event ShipCreatedDelegate ShipCreated;

		public delegate void UnlockAddedDelegate(Enums.Game_Unlocks addedUnlock);
		public event UnlockAddedDelegate UnlockAdded;

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

			_unlocker = new Unlocker();

			SpriteManager.ImageCache = new Godot.Collections.Dictionary<string, Texture2D>();

			GameData = CoreData.CreateNewGameFile();

			_screenLocker = GetNode<InputBlocker>("/root/Master/InputBlocker");

			Deuteros.Code.GameCore.SingletonInstance.DayPassed += Code.Platform.Screens.Production.UpdateProduction;
			Deuteros.Code.GameCore.SingletonInstance.DayPassed += Code.Platform.Screens.ShipInterior.UpdateShips;
			Deuteros.Code.GameCore.SingletonInstance.DayPassed += Code.Platform.Screens.Research.UpdateResearch;

			Input.MouseMode = Input.MouseModeEnum.Hidden;

			ChangeScene("IntroScreen.tscn", new List<Enums.SceneVariables>());
		}

		private void TriggerDay(uint previousDay, uint currentDay)
		{
			DayPassed?.Invoke(previousDay, currentDay);
		}

		private void TriggerPlanetChange(Objects.Interfaces.IPlanet newPlanet)
		{
			PlanetChanged?.Invoke(newPlanet);
		}

		public void TriggerStationPiecePlaced(Enums.StellarBodies stellarBody)
		{
			StationPiecePlaced?.Invoke(stellarBody);
		}

		public void TriggerProductionFinished(Objects.Factory factory)
		{
			ProductionFinished?.Invoke(factory);
		}

		public void TriggerResearchFinished(Objects.ResearchItem researchItem)
		{
			ResearchFinished?.Invoke(researchItem);
		}

		public void TriggerShipCreated(IShip ship)
		{
			ShipCreated?.Invoke(ship);
		}

		public void TriggerUnlockAdded(Enums.Game_Unlocks addedUnlock)
		{
			UnlockAdded?.Invoke(addedUnlock);
		}

		public Deuteros.Code.Objects.Interfaces.IPlanet GetCurrentPlanet()
		{
			return GameData.Planets[GameData.CurrentPlanet];
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			UpdateTime();

			if (_menuScreen != null)
				_menuScreen.HoverInfo.Text = HoverText;
		}

		private void UpdateTime()
		{
			if (GameData.TimeSkip || GameData.TimeSkipDay)
			{
				if (GameData.TimeSkipDay || Time.GetTicksMsec() - GameData.TimeSkipStart >= 500)
				{
					GameData.CurrentDay++;
					TriggerDay(GameData.CurrentDay-1, GameData.CurrentDay);

					GameData.TimeSkipStart = Time.GetTicksMsec();
					GameData.TimeSkipDay = false;

					QueueRedraw();
				}
			}
		}

		public void ShowBulletin(BulletinTypes bulletin)
		{
			GameCore.SingletonInstance.ChangeScene("Bulletins.tscn", new List<SceneVariables>());
			_menuScreen.Location.Text = "News Bulletins";
			((Bulletins)_currentScreen).DisplayBulletin(bulletin);
		}

		public void ChangeScene(string sceneName, List<Enums.SceneVariables> sceneVariables)
		{
			//This is a special case for the loading screen, only happens once on a new game
			if (_currentScreen != null && _currentScreen.SceneFilePath.Contains("IntroScreen"))
			{
				var newMenuScene = GD.Load<PackedScene>("res://Screens/Base/MenuBase.tscn").Instantiate<MainMenu>();
				GetNode<Node>("/root/Master/MainScene").AddChild(newMenuScene);
				_menuScreen = newMenuScene;
			}

			if (_currentScreen != null)
				_currentScreen.QueueFree();

			var newScene = GD.Load<PackedScene>("res://Screens/" + sceneName).Instantiate<BaseSubScene>();
			newScene.SceneVariables = sceneVariables;
			GetNode<Node>("/root/Master/MainScene").AddChild(newScene);
			_currentScreen = newScene;

			if (_menuScreen != null && GetCurrentPlanet().PlanetId == Enums.StellarBodies.earth && sceneVariables.Contains(Enums.SceneVariables.Ground))
			{
				_menuScreen.MenuButtons = EarthMenuButtons;
				_menuScreen.Location.Text = "Earth City";
				_menuScreen.Star.Text = "The Sun";
				_menuScreen.SetupMenus();
			}
			else if (_menuScreen != null && GetCurrentPlanet().PlanetId == Enums.StellarBodies.earth && sceneVariables.Contains(Enums.SceneVariables.Orbit))
			{
				_menuScreen.MenuButtons = EarthStationMenuButtons;
				_menuScreen.Location.Text = "Earth Orbital";
				_menuScreen.Star.Text = "The Sun";
				_menuScreen.SetupMenus();
			}
			else if (_menuScreen != null && GetCurrentPlanet().PlanetId != Enums.StellarBodies.earth)
			{
				_menuScreen.MenuButtons = StandardMenuButtons;

				if (_currentScreen.GetType() == typeof(ShipBay) || _currentScreen.GetType() == typeof(GroundMaterials) || _currentScreen.GetType() == typeof(Deuteros.Code.Platform.Screens.Store))
				{
					_menuScreen.Location.Text = GetCurrentPlanet().PlanetId.ToString() + " colony";
				}
				else
				{
					_menuScreen.Location.Text = GetCurrentPlanet().PlanetId.ToString() + " orbital";
				}

				_menuScreen.Star.Text = GetCurrentPlanet().ParentStar.ToScreenString();
				_menuScreen.SetupMenus();
			}

			if (_currentScreen.GetType() == typeof(ShipInterior))
			{
				_menuScreen.Location.Text = ((ShipInterior)_currentScreen).Ship.Name;
			}

			if (_currentScreen.SceneFilePath.Contains("ResourceMap"))
			{
				_menuScreen.Location.Text = "Deposit Analysis";
			}

			if (_currentScreen.SceneFilePath.Contains("News.tscn"))
			{
				_menuScreen.Location.Text = "News Bulletins";
			}

			if (_currentScreen.SceneFilePath.Contains("SaveScreen.tscn"))
			{
				_menuScreen.Location.Text = "Disk Access";
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
