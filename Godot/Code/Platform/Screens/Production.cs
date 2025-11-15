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
    public partial class Production : BaseSubScene
    {
        public const string ResearchSpriteBasePath = "res://Sprites//Items//Research//";
        public const string ProductionProgressSpriteBasePath = "res://Sprites//Items//Production//";
        public List<ProductionButton> Buttons { get; set; }
        public ProductionButton SelectedButton { get; set; }
        Label ProductionNameLabel { get; set; }
        Label StaffNameLabel { get; set; }
        Label StaffRankLabel { get; set; }
        Label StaffCountLabel { get; set; }
        TextureRect SmallItemImageTextureRect { get; set; }
        TextureRect ItemProgressImageTextureRect { get; set; }

        public override void _Ready()
        {
            ProductionNameLabel = GetNode<Label>("Labels/ProductionNameLabel");
            StaffNameLabel = GetNode<Label>("Labels/StaffNameLabel");
            StaffRankLabel = GetNode<Label>("Labels/StaffRankLabel");
            StaffCountLabel = GetNode<Label>("Labels/StaffCountLabel");

            SmallItemImageTextureRect = GetNode<TextureRect>("Sprites/SmallItemImage");
            ItemProgressImageTextureRect = GetNode<TextureRect>("Sprites/ItemProgressImage");

            RefreshButtons();

            base._Ready();
        }

        private void ProductionButton_Clicked(int index)
        {
            var clickedButton = Buttons.Single(T => T.ObjectData != null && T.ObjectData.Research.ResearchOrder == index);

            if (SelectedButton != null)
                SelectedButton.Selected = false;

            SelectedButton = clickedButton;
            SelectedButton.Selected = true;

            CheckProductionStart();

            DrawData();
        }

        private void CheckProductionStart()
        {
            var currentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();

            if (SelectedButton != null)
            {
                var addedItem = (Item)SelectedButton.ObjectData;
                Code.Objects.Factory currentFactory;

                if (currentPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)currentPlanet).GroundSelected)
                    currentFactory = ((Earth)currentPlanet).Factory;
                else
                    currentFactory = currentPlanet.Station.Factory;

                //There is no staff
                if (!currentFactory.AOC && (currentFactory.Builder == null || currentFactory.Builder.Count == 0))
                    return;

                if (!currentFactory.AOC)
                {
                    if (currentFactory.CurrentProductionItem() == null || currentFactory.CurrentProductionItem().Product.ItemType != addedItem.ItemType)
                    {
                        if (CheckResourceAvailable(currentPlanet, addedItem))
                        {
                            if (currentFactory.CurrentProductionItem() != null)
                            {
                                currentFactory.CurrentProductionItem().Production_Value = 1;
                                currentFactory.CurrentProductionItem().Active = false;
                            }

                            if (currentFactory.ProductionQueue.Any(T => T.Product.ItemType == addedItem.ItemType))
                            {
                                currentFactory.ProductionQueue.Single(T => T.Product.ItemType == addedItem.ItemType).Active = true;
                            }
                            else
                            {
                                var newProdItem = new ProductionItem(addedItem);
                                newProdItem.AOCOneTime = false;
                                newProdItem.AOCRepeat = false;
                                newProdItem.Active = true;
                                currentFactory.ProductionQueue.Add(newProdItem);

                                RemoveResourceByItem(currentPlanet, addedItem);
                            }
                        }
                    }
                }
                else
                {
                    var production = currentFactory.ProductionQueue.SingleOrDefault(T => T.Product.ItemType == addedItem.ItemType);

                    if (production == null)
                    {
                        var newProdItem = new ProductionItem(addedItem);
                        newProdItem.AOCOneTime = true;
                        newProdItem.AOCRepeat = false;

                        currentFactory.ProductionQueue.Add(newProdItem);
                    }
                    else if (production.AOCRepeat)
                    {
                        production.AOCRepeat = false;
                        production.AOCOneTime = false;
                    }
                    else if (production.AOCOneTime)
                    {
                        production.AOCRepeat = true;
                        production.AOCOneTime = false;
                    }
                }
            }
        }

        protected override void ResearchFinished(Objects.ResearchItem researchItem)
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var currentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();

            Buttons = Utility.Buttons.CreateButtons<ProductionButton, Item>(GetNode<GridContainer>("ProductionButtonGrid"),
            GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Production && T.Research != null && T.Research.Researched
            && (currentPlanet.PlanetId != Enums.Planetoids.earth || !T.OrbitOnly)
            && !T.AutoProduce
            ).Select(T => T).OrderBy(T => T.Research.ResearchOrder).ToDictionary(obj => obj.Research.ResearchOrder),
            this,
            nameof(ProductionButton_Clicked),
            "/Code/Platform/ProductionButton.cs",
            "ProductionButton");
        }

        protected override void ProductionFinished(Objects.Factory factory)
        {
            var currentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();

            Code.Objects.Factory currentFactory;

            if (currentPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)currentPlanet).GroundSelected)
                currentFactory = ((Earth)currentPlanet).Factory;
            else
                currentFactory = currentPlanet.Station.Factory;

            if (factory == currentFactory)
                SelectedButton = null;
        }

        //Triggered from gamecore
        protected override void DayTick(uint currentDay, uint nextDay)
        {
            CheckProductionStart();

            DrawData();
        }

        // Called every update.
        public override void _Draw()
        {
            DrawData();
        }

        public void DrawData()
        {
            var currentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();
            Code.Objects.Factory currentFactory;

            if (currentPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)currentPlanet).GroundSelected)
                currentFactory = ((Earth)currentPlanet).Factory;
            else
                currentFactory = currentPlanet.Station.Factory;

            if (currentFactory.Builder != null)
            {
                StaffCountLabel.Text = currentFactory.Builder.Count.ToString();
                StaffNameLabel.Text = currentFactory.Builder.Leader;
                StaffRankLabel.Text = currentFactory.Builder.ActionsTaken.ToString();
            }
            else
            {
                StaffCountLabel.Text = "";
                StaffNameLabel.Text = "None";
                StaffRankLabel.Text = "";
            }

            if (currentFactory.CurrentProductionItem() != null)
            {
                var currentProductionItem = currentFactory.CurrentProductionItem();
                ProductionNameLabel.Text = currentProductionItem.Product.FullName;
                SmallItemImageTextureRect = SpriteManager.LoadImageToTextureRect(ResearchSpriteBasePath + currentProductionItem.Product.ItemType.ToString() + ".png", SmallItemImageTextureRect);
                ItemProgressImageTextureRect = SpriteManager.LoadImageToTextureRect(ProductionProgressSpriteBasePath + currentProductionItem.Product.ItemType.ToString() + "_" + currentProductionItem.Production_Complete + ".png", ItemProgressImageTextureRect);
            }
            else
            {
                ProductionNameLabel.Text = "";
                SmallItemImageTextureRect.Texture = null;
                ItemProgressImageTextureRect = SpriteManager.LoadImageToTextureRect(ProductionProgressSpriteBasePath + "idle.png", ItemProgressImageTextureRect);
            }
        }

        #region Statics

        public static void UpdateProduction(uint currentDay, uint nextDay)
        {
            foreach (var planet in GameCore.SingletonInstance.GameData.Planets)
            {
                var currentPlanet = (Planet)planet.Value;
                Code.Objects.Factory currentFactory = null;

                if (currentPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)currentPlanet).GroundSelected)
                    currentFactory = ((Earth)currentPlanet).Factory;
                else if (currentPlanet.Station.Built)
                    currentFactory = currentPlanet.Station.Factory;

                if (currentFactory != null)
                {
                    currentFactory.IncrementCurrentProd();

                    if (currentFactory.CurrentProductionItem() != null)
                    {
                        //Production complete
                        if (currentFactory.CurrentProductionItem().Complete)
                        {
                            currentPlanet.AddItems(currentFactory.CurrentProductionItem().Product.ItemType, 1);

                            currentFactory.Builder.ActionsTaken++;

                            GameCore.SingletonInstance.TriggerProductionFinished(currentFactory);

                            if (!currentFactory.AOC)
                            {
                                currentFactory.ProductionQueue.Remove(currentFactory.CurrentProductionItem());
                            }
                            else if (currentFactory.ProductionQueue.Any(T => T.AOCRepeat))
                            {
                                var currentResearchOrder = currentFactory.CurrentProductionItem().Product.Research.ResearchOrder;

                                if (currentFactory.CurrentProductionItem().AOCOneTime)
                                    currentFactory.ProductionQueue.Remove(currentFactory.CurrentProductionItem());

                                if (currentFactory.ProductionQueue.Where(T => CheckResourceAvailable(currentPlanet, T.Product)).Count() > 0)
                                {
                                    if (currentFactory.ProductionQueue.Any(T => T.Product.Research.ResearchOrder > currentResearchOrder))
                                        currentFactory.ProductionQueue.OrderBy(T => T.Product.Research.ResearchOrder).Where(T => CheckResourceAvailable(currentPlanet, T.Product) && T.Product.Research.ResearchOrder > currentResearchOrder).First().Active = true;
                                    else
                                        currentFactory.ProductionQueue.OrderBy(T => T.Product.Research.ResearchOrder).Where(T => CheckResourceAvailable(currentPlanet, T.Product)).First().Active = true;

                                    RemoveResourceByItem(currentPlanet, currentFactory.CurrentProductionItem().Product);
                                }
                            }
                        }
                    }

                    foreach (var autoProduced in GameCore.SingletonInstance.GameData.GetAllActiveItems().Where(T => T.AutoProduce))
                    {
                        if (CheckResourceAvailable(currentPlanet, autoProduced))
                        {
                            if (autoProduced.AutoProduceFlip)
                            {
                                autoProduced.AutoProduceFlip = false;
                            }
                            else
                            {
                                autoProduced.AutoProduceFlip = true;
                                RemoveResourceByItem(currentPlanet, autoProduced);
                                currentPlanet.AddItems(autoProduced.ItemType, 3);
                            }
                        }
                    }
                }
            }
        }

        public static bool CheckResourceAvailable(IPlanet productionPlanet, Item productionItem)
        {
            Objects.Store currentStore;

            if (productionPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)productionPlanet).GroundSelected)
                currentStore = ((Earth)productionPlanet).Stores;
            else
                currentStore = productionPlanet.Station.Resources.Stores;

            var prodPossible = true;

            foreach (var material in productionItem.BuildRequirements)
                if (material.ItemCount > currentStore[material.ItemType])
                    prodPossible = false;

            return prodPossible;
        }

        public static void RemoveResourceByItem(IPlanet productionPlanet, Item productionItem)
        {
            Objects.Store currentStore;

            if (productionPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)productionPlanet).GroundSelected)
                currentStore = ((Earth)productionPlanet).Stores;
            else
                currentStore = productionPlanet.Station.Resources.Stores;

            foreach (var material in productionItem.BuildRequirements)
                currentStore[material.ItemType] -= material.ItemCount;
        }

        public static void AddResourceByItem(IPlanet productionPlanet, Item productionItem)
        {
            Objects.Store currentStore;

            if (productionPlanet.PlanetId == Enums.Planetoids.earth && ((Earth)productionPlanet).GroundSelected)
                currentStore = ((Earth)productionPlanet).Stores;
            else
                currentStore = productionPlanet.Station.Resources.Stores;

            foreach (var material in productionItem.BuildRequirements)
                currentStore[material.ItemType] += material.ItemCount;
        }

        #endregion
    }
}