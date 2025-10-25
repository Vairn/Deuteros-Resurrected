using Deuteros.Code.Platform.Base;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Deuteros.Code.Platform.Helpers;
using System.Threading.Tasks;

namespace Deuteros.Code.Platform.Screens
{
    public partial class Research : BaseSubScene
    {
        public const string ResearchSpriteBasePath = "res://Sprites//Items//Research//";
        public List<ResearchButton> Buttons { get; set; }
        public ResearchButton SelectedButton { get; set; }
        Label StaffCountLabel { get; set; }
        Label RankLabel { get; set; }
        Label LeaderNameLabel { get; set; }
        Label ItemAnalysisLabel { get; set; }
        Label ItemNameLabel { get; set; }
        Label TechLevelDataLabel { get; set; }
        Label TechLevelLabel { get; set; }
        Label MassLabel { get; set; }
        Label TeamWorkingLabel { get; set; }
        Label ProjectCompletionLabel { get; set; }
        Label MassDataLabel { get; set; }
        Label ProductionMaterialListLabel { get; set; }
        Label ItemNotesLabel { get; set; }
        Label ItemNotesDataLabel { get; set; }
        TextureRect ResearchImageTextureRect { get; set; }

        public override void _Ready()
        {
            StaffCountLabel = GetNode<Label>("Labels/StaffCountLabel");
            RankLabel = GetNode<Label>("Labels/RankLabel");
            LeaderNameLabel = GetNode<Label>("Labels/LeaderNameLabel");
            ItemAnalysisLabel = GetNode<Label>("Labels/ItemAnalysisLabel");
            ItemNameLabel = GetNode<Label>("Labels/ItemNameLabel");
            TechLevelDataLabel = GetNode<Label>("Labels/TechLevelDataLabel");
            TechLevelLabel = GetNode<Label>("Labels/TechLevelLabel");
            MassLabel = GetNode<Label>("Labels/Researched/MassLabel");
            TeamWorkingLabel = GetNode<Label>("Labels/InProgress/TeamWorkingLabel");
            ProjectCompletionLabel = GetNode<Label>("Labels/InProgress/ProjectCompletionLabel");
            MassDataLabel = GetNode<Label>("Labels/Researched/MassDataLabel");
            ProductionMaterialListLabel = GetNode<Label>("Labels/Researched/ProductionMaterialListLabel");
            ItemNotesLabel = GetNode<Label>("Labels/Researched/ItemNotesLabel");
            ItemNotesDataLabel = GetNode<Label>("Labels/Researched/ItemNotesDataLabel");

            ResearchImageTextureRect = GetNode<TextureRect>("Sprites/ResearchImage");

            SelectedButton = new ResearchButton();

            Buttons = Utility.Buttons.CreateButtons<ResearchButton, ResearchItem>(GetNode<Control>("ResearchButtons"),
                GameCore.SingletonInstance.GameData.ItemList.Where(T => T.Research != null).Select(T => T.Research).ToDictionary(obj => obj.Index),
                this,
                nameof(ResearchButton_Clicked),
                "/Code/Platform/ResearchButton.cs",
                "ResearchButton");

            if (GameCore.SingletonInstance.Earth.CurrentResearchItem != null)
            {
                SelectedButton = Buttons.Single(T => T.ObjectData != null && T.ObjectData.Index == GameCore.SingletonInstance.Earth.CurrentResearchItem.Index);
                DrawData(true);
            }
            else
            {
                DrawData(false);
            }

            base._Ready();
        }

        private void ResearchButton_Clicked(int index)
        {
            var clickedButton = Buttons.Single(T => T.ObjectData != null && T.ObjectData.Index == index);

            if (!clickedButton.ObjectData.Locked)
            {
                SelectedButton.Selected = false;
                SelectedButton = clickedButton;
                SelectedButton.Selected = true;

                var earth = GameCore.SingletonInstance.GetPlanet<Earth>(Enums.Planetoids.earth);
                earth.CurrentResearchItem = SelectedButton.ObjectData;

                DrawData(false);
            }
        }

        //Triggered from gamecore
        protected override async void DayTick(uint currentDay, uint nextDay)
        {
            UpdateResearch(true);
        }

        public void UpdateResearch(bool dayPassed)
        {
            if (SelectedButton != null && SelectedButton.ObjectData != null)
            {
                SelectedButton.Redraw(dayPassed);

                DrawData(dayPassed);
            }
        }

        // Called every update.
        public override void _Draw()
        {
            //DrawData(false);
        }

        public void DrawData(bool dayPassed)
        {
            ResearchImageTextureRect.Texture = null;
            ItemAnalysisLabel.Text = "";
            ItemNameLabel.Text = "";
            TechLevelDataLabel.Text = "";
            TechLevelLabel.Text = "";
            MassLabel.Text = "";
            TeamWorkingLabel.Text = "";
            ProjectCompletionLabel.Text = "";
            MassDataLabel.Text = "";
            ProductionMaterialListLabel.Text = "";
            ItemNotesLabel.Text = "";
            ItemNotesDataLabel.Text = "";

            if (GameCore.SingletonInstance.Earth.ResearchStaff == null)
            {
                StaffCountLabel.Text = "";
                RankLabel.Text = "";
                LeaderNameLabel.Text = "None";
            }
            else
            {
                StaffCountLabel.Text = GameCore.SingletonInstance.Earth.ResearchStaff.Count.ToString();
                RankLabel.Text = ((Enums.StaffLevel_Researcher)GameCore.SingletonInstance.Earth.ResearchStaff.GetLevel()).ToString();
                LeaderNameLabel.Text = "Seth";
            }

            if (SelectedButton != null && SelectedButton.ObjectData != null)
            {
                var researchItem = GameCore.SingletonInstance.GameData.ItemList.Single(T => T.Research != null && T.Research.Index == SelectedButton.ObjectData.Index);

                ItemAnalysisLabel.Text = "Item Analysis";
                ItemNameLabel.Text = researchItem.FullName;
                TechLevelLabel.Text = "Tech Level";
                TechLevelDataLabel.Text = researchItem.Research.TechLevel.ToString();

                if (researchItem.Research.Researched)
                {
                    ResearchImageTextureRect = SpriteManager.LoadImageToTextureRect(ResearchSpriteBasePath + researchItem.Research.ItemType.ToString() + ".png", ResearchImageTextureRect);
                    MassLabel.Text = "Mass " + "".PadRight(researchItem.Mass.ToString().Length, ' ') + "t.";
                    MassDataLabel.Text = researchItem.Mass.ToString();
                    TeamWorkingLabel.Text = "";
                    ProjectCompletionLabel.Text = "";
                    ProductionMaterialListLabel.Text = string.Join('\n', researchItem.BuildRequirements.Select(T => T.ItemCount + " " + T.ItemType.ToString()));
                    ItemNotesLabel.Text = "This item may\nbe produced";
                    ItemNotesDataLabel.Text = researchItem.OrbitOnly ? "In Orbit Only" : "by any factory";
                }
                else if (GameCore.SingletonInstance.Earth.ResearchStaff != null && researchItem.Research.ResearchPercentageComplete > 0 && dayPassed)
                {
                    ResearchImageTextureRect.Texture = null;
                    MassLabel.Text = "";
                    MassDataLabel.Text = "";
                    TeamWorkingLabel.Text = "Team Working";
                    ProjectCompletionLabel.Text = "Project is\n" + researchItem.Research.ResearchPercentageComplete.ToString().PadLeft(2, ' ') + "% complete";
                    ProductionMaterialListLabel.Text = "";
                    ItemNotesLabel.Text = "";
                    ItemNotesDataLabel.Text = "";
                }
            }
        }
    }
}