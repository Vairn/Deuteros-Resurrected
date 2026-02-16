using Godot;
using System;
using System.Collections.Generic;

namespace Deuteros.Code.Platform.Screens
{
	public partial class MainMenu : Node2D
	{
		public Label HoverInfo { get; set; }
		public Label Location { get; set; }
		public Label Time { get; set; }
        public List<Objects.MenuButton> MenuButtons { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			MenuButtons = new List<Objects.MenuButton>();
			HoverInfo = GetNode<Label>("HoverInfo");
			Location = GetNode<Label>("Location/LocationBox/Location");
			Time = GetNode<Label>("Time/TimeBox/Time");

			UpdateTime(Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay, Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay);

			Deuteros.Code.GameCore.SingletonInstance.DayPassed += DayTick;

            if (Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentPlanet.ToString().ToUpperInvariant() != Location.Text)
            {
                Location.Text = Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentPlanet.ToString().ToUpperInvariant();
            }

            base._Ready();
        }

        public override void _Process(double delta)
		{
			if (Deuteros.Code.GameCore.HoverText != HoverInfo.Text)
			{
				HoverInfo.Text = Deuteros.Code.GameCore.HoverText;
			}
		}

		//Triggered from gamecore
		public void DayTick(uint currentDay, uint nextDay)
		{
			UpdateTime(currentDay, nextDay);
		}

		private void UpdateTime(uint currentDay, uint nextDay)
		{
			var curDay = (nextDay % 1000).ToString().PadLeft(3, '0');
			var outputYear = (3100 + Math.Floor((decimal)(nextDay / 1000))) + " " + curDay + ".00";

			Time.Text = outputYear;
		}

		public void SetupMenus()
		{
			var column = "A";
			var row = 1;

			for (var i = 0; i < 12; i++)
			{
				var menuButton = MenuButtons[i];

				var currentButton = GetNode<MenuButton>("MainButtons/" + column + row.ToString() + "/");

                if (menuButton == null)
				{
					currentButton.SetButtonType(Enums.Menu_Buttons.Empty);
					currentButton.SceneVariables = null;
                    currentButton.TargetScene = Enums.Scenes.None;
                }
				else
				{
                    currentButton.SetButtonType(menuButton.ButtonType);
                    currentButton.SceneVariables = menuButton.SceneVariables;
                    currentButton.TargetScene = menuButton.SceneToLoad;
                }

                row++;

				if (row == 7)
				{
					row = 1;
					column = "B";
				}
			}
		}
	}
}