using Deuteros.Code.Objects;
using Deuteros.Code.Platform;
using Deuteros.Code.Utility;
using Deuteros.Code;
using Godot;
using System;
using System.Threading;

public partial class Settings : Node2D
{
    public Button SoundToggle { get; set; }
    public Button SkipToShuttles { get; set; }
    public Button EarthStationTo7 { get; set; }
    public Label SoundToggleText { get; set; }
    public bool SoundOn { get; set; }
    public int BusMasterIndex { get; set; }

    public override void _Ready()
    {
        BusMasterIndex = AudioServer.GetBusIndex("Master");

        SkipToShuttles = (Button)GetNode("SkipToShuttles");
        EarthStationTo7 = (Button)GetNode("EarthStationTo7");
        SoundToggle = (Button)GetNode("SoundToggle");
        SoundToggle.Connect("button_up", new Callable(this, nameof(SoundToggle_ButtonUp)));

        SkipToShuttles.Pressed += SkipToShuttles_Pressed;
        EarthStationTo7.Pressed += EarthStationTo7_Pressed;

        SoundToggleText = (Label)GetNode("SoundToggle/SoundToggleText");

        SoundOn = !AudioServer.IsBusMute(BusMasterIndex);
        SoundToggleText.Text = SoundOn ? "ON" : "OFF";

        base._Ready();
    }

    private void EarthStationTo7_Pressed()
    {
        var gameData = GameCore.SingletonInstance.GameData;
        var earth = (Earth)gameData.Planets[Enums.StellarBodies.earth];
        earth.Station.BuildParts = 7;

        if (!GameCore.SingletonInstance.GameData.Unlocks.Contains(Enums.Game_Unlocks.First_Station_Segment))
        {
            SkipToShuttles_Pressed();
        }
    }

    private void SkipToShuttles_Pressed()
    {
        var gameData = GameCore.SingletonInstance.GameData;

        gameData.GetItem(Enums.ItemTypes.s_chassis).Research.Researched = true;
        gameData.GetItem(Enums.ItemTypes.s_drive).Research.Researched = true;
        gameData.GetItem(Enums.ItemTypes.meh_fuel).Research.Researched = true;
        gameData.GetItem(Enums.ItemTypes.of_frame).Research.Researched = true;

        gameData.GetItem(Enums.ItemTypes.s_chassis).Research.ResearchOrder = 2;
        gameData.GetItem(Enums.ItemTypes.s_drive).Research.ResearchOrder = 3;
        gameData.GetItem(Enums.ItemTypes.meh_fuel).Research.ResearchOrder = 4;
        gameData.GetItem(Enums.ItemTypes.of_frame).Research.ResearchOrder = 5;

        var earth = (Earth)gameData.Planets[Enums.StellarBodies.earth];

        if (earth.ResearchStaff == null || earth.ResearchStaff.Count == 0)
        {
            earth.ResearchStaff = new Staff();
            earth.ResearchStaff.Leader = "Von Braun";
            earth.ResearchStaff.Count = 200;
            earth.ResearchStaff.Type = Enums.StaffType.Research;
        }

        if (earth.Factory.Builder == null || earth.Factory.Builder.Count == 0)
        {
            earth.Factory.Builder = new Staff();
            earth.Factory.Builder.Leader = "Bob";
            earth.Factory.Builder.Count = 200;
            earth.Factory.Builder.Type = Enums.StaffType.Production;
        }

        earth.PlanetResources.Derricks = 8;
        earth.PlanetResources.Stores[Enums.ItemTypes.s_chassis] = 1;
        earth.PlanetResources.Stores[Enums.ItemTypes.s_drive] = 1;
        earth.PlanetResources.Stores[Enums.ItemTypes.of_frame] = 8;

        GameCore.SingletonInstance.TriggerUnlockAdded(Enums.Game_Unlocks.Shuttle_Unlock);
        GameCore.SingletonInstance.TriggerUnlockAdded(Enums.Game_Unlocks.First_Station_Segment);
    }

    private void SoundToggle_ButtonUp()
    {
        SoundOn = !SoundOn;

        SoundToggleText.Text = SoundOn ? "ON" : "OFF";
                
        AudioServer.SetBusMute(BusMasterIndex, !SoundOn);

        QueueRedraw();
    }

    // Called every update.
    public override void _Draw()
    {
    }
}