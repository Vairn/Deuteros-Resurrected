using Godot;
using System;

namespace Deuteros.Code.Platform
{
    public partial class TimerSwitchButton : Base.HoverButton
    {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            this.Connect("button_up", new Callable(this, nameof(TimerSwitch_ButtonUp)));

            base._Ready();
        }

        private void TimerSwitch_ButtonUp()
        {
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkip = !Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkip;
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkipStart = Time.GetTicksMsec();
        }
    }
}