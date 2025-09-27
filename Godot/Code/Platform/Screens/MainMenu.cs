using Godot;
using System;

namespace Deuteros.Code.Platform.Screens
{
	public partial class MainMenu : Node
	{
		public Label HoverInfo { get; set; }
		public Label Location { get; set; }
		public Label Time { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			HoverInfo = GetNode<Label>("HoverInfo");
			Location = GetNode<Label>("TimeAndLocation/Location");
			Time = GetNode<Label>("TimeAndLocation/Time");

			UpdateTime(Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay, Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay + 1);

			Deuteros.Code.GameCore.SingletonInstance.DayPassed += DayTick;
		}

		public override void _Process(double delta)
		{
			if (Deuteros.Code.GameCore.HoverText != HoverInfo.Text)
			{
				HoverInfo.Text = Deuteros.Code.GameCore.HoverText;
			}

			if (Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentPlanet.ToString().ToUpperInvariant() != Location.Text)
			{
				Location.Text = Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentPlanet.ToString().ToUpperInvariant();
			}
		}

		//Triggered from gamecore
		public void DayTick(uint currentDay, uint nextDay)
		{
			UpdateTime(currentDay, nextDay);
		}

		private void UpdateTime(uint currentDay, uint nextDay)
		{
			var curDay = (currentDay % 1000).ToString().PadLeft(3, '0');
			var outputYear = (3100 + Math.Floor((decimal)(currentDay / 1000))) + " " + curDay + ".00";

			Time.Text = outputYear;
		}
	}
}