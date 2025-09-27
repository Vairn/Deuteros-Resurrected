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
    public Label SoundToggleText { get; set; }
    public bool SoundOn { get; set; }
    public int BusMasterIndex { get; set; }

    public override void _Ready()
    {
        BusMasterIndex = AudioServer.GetBusIndex("Master");

        SoundToggle = (Button)GetNode("SoundToggle");
        SoundToggle.Connect("button_up", new Callable(this, nameof(SoundToggle_ButtonUp)));

        SoundToggleText = (Label)GetNode("SoundToggle/SoundToggleText");

        SoundOn = !AudioServer.IsBusMute(BusMasterIndex);
        SoundToggleText.Text = SoundOn ? "ON" : "OFF";

        base._Ready();
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