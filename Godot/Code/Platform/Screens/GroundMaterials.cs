using Deuteros.Code.Platform.Base;
using Godot;
using System;

namespace Deuteros.Code.Platform.Screens
{
    public partial class GroundMaterials : BaseSubScene
    {
        public Label DerrickCount { get; set; }
        public Button AddDerrick { get; set; }
        public Texture2D GreenArrow { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            DerrickCount = (Label)GetParent().GetNode("DerrickCount");
            AddDerrick = (Button)GetParent().GetNode("AddDerrick");
            AddDerrick.Connect("button_up", new Callable(this, nameof(AddDerrick_ButtonUp)));

            GreenArrow = (Texture2D)ResourceLoader.Load("res://Sprites/Buttons/GreenArrowRight.fw.png");

            base._Ready();
        }

        private void AddDerrick_ButtonUp()
        {
            var currentPlanet = Deuteros.Code.GameCore.SingletonInstance.GetCurrentPlanet();

            if (currentPlanet.PlanetResources.Derricks < 8 && currentPlanet.Stores[Deuteros.Code.Enums.ItemTypes.derrick] > 0)
            {
                currentPlanet.PlanetResources.Derricks++;
                currentPlanet.Stores[Deuteros.Code.Enums.ItemTypes.derrick]--;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            var currentPlanet = Deuteros.Code.GameCore.SingletonInstance.GetCurrentPlanet();
            DerrickCount.Text = currentPlanet.PlanetResources.Derricks.ToString();
        }

        // Called every update.
        public override void _Draw()
        {
            DrawMinerals(Deuteros.Code.GameCore.SingletonInstance.GetCurrentPlanet());
        }

        public void DrawMinerals(Deuteros.Code.Objects.Interfaces.IPlanet currentPlanet)
        {
            var startY = 227;
            var textureStartY = 206;
            var lineSizeY = 32;
            var mineralAmountX = 687;
            var greenArrowX = 891;
            var mineralNameX = 950;
            var minCount = 0;

            foreach(var mineral in currentPlanet.PlanetResources.Materials)
            {
                var mineralText = mineral.SurveyTicks > 0 ? "SURVEY" : mineral.GroundAmount.ToString();

                this.DrawString(DefaultFont, new Vector2(mineralNameX, startY + (lineSizeY * minCount)), mineral.MaterialType.ToString(), HorizontalAlignment.Left, -1, 16, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                this.DrawString(DefaultFont, new Vector2(mineralAmountX + Deuteros.Code.Utility.String.PadX(mineralText.Length, 6), startY + (lineSizeY * minCount)), mineralText, HorizontalAlignment.Left, -1, 16, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                this.DrawTexture(GreenArrow, new Vector2(greenArrowX, textureStartY + (lineSizeY * minCount)));
                minCount++;
            }
        }
    }
}