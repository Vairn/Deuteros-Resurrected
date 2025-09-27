using Godot;
using System;

namespace Deuteros.Code.Platform
{
    public partial class TimerHoldButton : Base.HoverButton
    {
        public uint DaysAtStart { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            this.Connect("button_up", new Callable(this, nameof(TimerHold_ButtonUp)));
            this.Connect("button_down", new Callable(this, nameof(TimerHold_ButtonDown)));

            base._Ready();
        }

        private void TimerHold_ButtonUp()
        {
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkip = false;
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkipStart = Time.GetTicksMsec();
            if (Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay - DaysAtStart == 0 && Time.GetTicksMsec() - Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkipStart < 999)
            {
                Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkipDay = true;
            }
        }

        private void TimerHold_ButtonDown()
        {
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkip = true;
            Deuteros.Code.GameCore.SingletonInstance.GameData.TimeSkipStart = Time.GetTicksMsec();
            DaysAtStart = Deuteros.Code.GameCore.SingletonInstance.GameData.CurrentDay;
        }
    }
}