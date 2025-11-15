using Deuteros.Code.Platform.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Deuteros.Code.Platform.Base
{
    public partial class BaseSubScene : Node2D
    {
        public static Font DefaultFont { get; set; }
        private PackedScene _SettingsScreen;
        public bool SettingsShown { get; set; }
        public List<Enums.SceneVariables> SceneVariables { get; set; }
        public List<Objects.MenuButton> MenuButtons { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            DefaultFont = Deuteros.Code.GameCore.DefaultFont;

            Deuteros.Code.GameCore.SingletonInstance.DayPassed += DayTick;
            Deuteros.Code.GameCore.SingletonInstance.PlanetChanged += PlanetChange;
            Deuteros.Code.GameCore.SingletonInstance.ProductionFinished += ProductionFinished;
            Deuteros.Code.GameCore.SingletonInstance.ResearchFinished += ResearchFinished;

            _SettingsScreen = GD.Load<PackedScene>("res://Screens/Base/Settings.tscn");
            SettingsShown = false;
        }

        public override void _UnhandledInput(InputEvent e)
        {
            if (!OverlayManager.Instance.IsOpen && e.IsActionPressed("ui_cancel"))
            {
                GetViewport().SetInputAsHandled();
                OverlayManager.Instance.ShowOverlay(_SettingsScreen);
            }
        }

        public override void _ExitTree()
        {
            Deuteros.Code.GameCore.SingletonInstance.DayPassed -= DayTick;
            Deuteros.Code.GameCore.SingletonInstance.PlanetChanged -= PlanetChange;
            Deuteros.Code.GameCore.SingletonInstance.ProductionFinished -= ProductionFinished;
            Deuteros.Code.GameCore.SingletonInstance.ResearchFinished -= ResearchFinished;
        }

        protected virtual async void DayTick(uint currentDay, uint nextDay) { QueueRedraw(); }

        protected virtual async void PlanetChange(Deuteros.Code.Objects.Interfaces.IPlanet newPlanet) { QueueRedraw(); }

        protected virtual async void ProductionFinished(Deuteros.Code.Objects.Factory factory) { QueueRedraw(); }

        protected virtual async void ResearchFinished(Deuteros.Code.Objects.ResearchItem researchItem) { QueueRedraw(); }
    }
}