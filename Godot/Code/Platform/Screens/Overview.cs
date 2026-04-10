using Deuteros.Code;
using Deuteros.Code.Objects;
using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform.Base;
using Deuteros.Code.Platform.Screens;
using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

public partial class Overview : BaseSubScene
{
	List<TextureButton> StationButtons = new List<TextureButton>();
	private void Station_Pressed(int stationPressed)
	{
		var s = Deuteros.Code.GameCore.SingletonInstance.GameData.Planets.Values
			.Select(p => p.Station)
			.Where(s => s.BuildParts > 0).ToList();


		if (!s[stationPressed].Built) return;

		GameCore.SingletonInstance.GameData.CurrentPlanet = s[stationPressed].PlanetId;

		var sceneVariables = new List<SceneVariables>();
		sceneVariables.Add(Enums.SceneVariables.Orbit);

		//Double underscores in scene names represent a flag to pass to the scene
		var sceneNameSplit = Enums.Scenes.Overview.ToString().Split(new string[] { "__" }, System.StringSplitOptions.None);

		//Underscores in scene names represent a folder
		Deuteros.Code.GameCore.SingletonInstance.ChangeScene(sceneNameSplit[0].Replace("_", "/") + ".tscn", sceneVariables);

		//for (int i = 0; i < StationButtons.Count; i++)
		//{
		//    StationButtons[i].Hide();
		//}

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StationButtons.Add(GetNode<TextureButton>("TextureButton1"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton2"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton3"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton4"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton5"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton6"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton7"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton8"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton9"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton10"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton11"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton12"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton13"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton14"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton15"));
		StationButtons.Add(GetNode<TextureButton>("TextureButton16"));

		var s = Deuteros.Code.GameCore.SingletonInstance.GameData.Planets.Values
			.Select(p => p.Station)
			.Where(s => s.BuildParts>0);

		for (int i=0; i<StationButtons.Count; i++)
		{
			var stationPressed = i;
			StationButtons[i].Pressed += () => Station_Pressed(stationPressed);
			if (i >= s.Count())
			{
				StationButtons[i].Hide();
			}
			else
			{
				StationButtons[i].Show();
			}
		}

		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
