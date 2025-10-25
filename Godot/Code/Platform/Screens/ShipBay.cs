using Deuteros.Code.Platform.Base;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Deuteros.Code.Platform.Helpers;
using System.Security.Cryptography.X509Certificates;
using Deuteros.Code.Objects.Interfaces;

namespace Deuteros.Code.Platform.Screens
{
    public partial class ShipBay : BaseSubScene
    {
        public const string ResearchSpriteBasePath = "res://Sprites//Items//Research//";
        public bool Ground { get; set; }
        public bool Earth { get; set; }
        public bool Shuttle { get; set; }
        public IPlanet CurrentPlanet { get; set; }
        Label FuelAvailable { get; set; }
        Label FuelFilled { get; set; }
        TextureRect SmallItemImageTextureRect { get; set; }
        TextureRect ItemProgressImageTextureRect { get; set; }

        public override void _Ready()
        {
            FuelAvailable = GetNode<Label>("Labels/FuelAvailableLabel");
            FuelFilled = GetNode<Label>("Labels/FuelFilledLabel");

            SmallItemImageTextureRect = GetNode<TextureRect>("Sprites/SmallItemImage");
            ItemProgressImageTextureRect = GetNode<TextureRect>("Sprites/ItemProgressImage");

            //Setup some flags to make our lives easier
            CurrentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();
            Earth = CurrentPlanet.PlanetId == Enums.Planetoids.earth;
            Ground = SceneVariables.Contains(Enums.SceneVariables.Ground);
            Shuttle = SceneVariables.Contains(Enums.SceneVariables.Shuttle);

            RefreshButtons();

            base._Ready();
        }

        private void RefreshButtons()
        {
            
        }

        //Triggered from gamecore
        protected override void DayTick(uint currentDay, uint nextDay)
        {
            DrawData();
        }

        // Called every update.
        public override void _Draw()
        {
            DrawData();
        }

        public void DrawData()
        {
        }
    }
}