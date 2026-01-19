using Deuteros.Code.Platform.Base;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Deuteros.Code.Platform.Helpers;
using Deuteros.Code.Objects.Interfaces;
using System.Resources;

namespace Deuteros.Code.Platform.Screens
{
    public partial class Store : BaseSubScene
    {
        public List<StoreButton> Buttons { get; set; }
        public StoreButton SelectedButton { get; set; }
        public Button SwitchStoreType { get; set; }
        public bool ViewTypeToggle { get; set; }
        public Texture2D BlueArrow { get; set; }
        public Label ResourceListLabel { get; set; }
        public Label BuildAmountLabel { get; set; }
        public GridContainer StoreButtonsNode { get; set; }

        public override void _Ready()
        {
            SwitchStoreType = (Button)GetNode("SwitchStoreImage/SwitchStoreType");
            SwitchStoreType.Connect("button_up", new Callable(this, nameof(SwitchStoreType_ButtonUp)));

            ResourceListLabel = (Label)GetNode("ResourceList");
            BuildAmountLabel = (Label)GetNode("Recipe");

            BlueArrow = (Texture2D)ResourceLoader.Load("res://Sprites/Buttons/BlueArrowRight.fw.png");

            SelectedButton = null;

            StoreButtonsNode = GetNode<GridContainer>("ButtonsImage/StoreButtons");

            Buttons = Utility.Buttons.CreateButtons<StoreButton, Item>(StoreButtonsNode,
                GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Research != null && T.Research.Researched).Select(T => T).OrderBy(T => T.Research.ResearchOrder).ToDictionary(obj => obj.Research.ResearchOrder),
                this,
                nameof(StoreButton_Clicked),
                "/Code/Platform/StoreButton.cs",
                "StoreButton");

            DrawData();

            base._Ready();
        }

        private void StoreButton_Clicked(int index)
        {
            var clickedButton = Buttons.Single(T => T.ObjectData != null && T.ObjectData.Research.ResearchOrder == index);

            if (!clickedButton.ObjectData.Locked)
            {
                if (SelectedButton != null)
                    SelectedButton.Selected = false;

                SelectedButton = clickedButton;
                SelectedButton.Selected = true;

                DrawData();
            }
        }

        private void SwitchStoreType_ButtonUp()
        {
            ViewTypeToggle = !ViewTypeToggle;
            QueueRedraw();
        }

        // Called every update.
        public override void _Draw()
        {
            DrawData();
        }

        public void DrawData()
        {
            if (GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Research != null && T.Research.Researched).Count() > Buttons.Count())
            {
                Buttons = Utility.Buttons.CreateButtons<StoreButton, Item>(StoreButtonsNode,
                    GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Research != null && T.Research.Researched).Select(T => T).OrderBy(T => T.Research.ResearchOrder).ToDictionary(obj => obj.Research.ResearchOrder),
                    this,
                    nameof(StoreButton_Clicked),
                    "/Platform/StoreButton.cs",
                    "StoreButton");
            }

            var currentPlanet = Deuteros.Code.GameCore.SingletonInstance.GetCurrentPlanet();

            ResourceListLabel.Text = "";

            foreach (var item in GameCore.SingletonInstance.GameData.ItemList.Where(T => !T.Locked && (T.ItemCategory == Enums.ItemCategory.item) == ViewTypeToggle))
                ResourceListLabel.Text += item.ItemType.ToScreenString() + " " + currentPlanet.PlanetResources.Stores[item.ItemType] + "\n";

            if (SelectedButton != null)
            {
                var recipeText = "Enough supplies for {0} {1}s";
                var maxCount = 200;
                var recipeItem = SelectedButton.ObjectData as Item;

                foreach (var material in recipeItem.BuildRequirements)
                {
                    var maxProd = currentPlanet.PlanetResources.Stores[material.ItemType] / material.ItemCount;
                    if (maxProd < maxCount)
                        maxCount = maxProd;
                }

                BuildAmountLabel.Text = string.Format(recipeText, maxCount, recipeItem.FullName);
            }
        }
    }
}