using Deuteros.Code.Platform.Base;
using Godot;
using System;
using System.Collections.Generic;

namespace Deuteros.Code.Platform.Screens
{
    public partial class Ground : BaseSubScene
    {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetupMenus();

            base._Ready();
        }

        private void SetupMenus()
        {
            //TODO - This is lame, do it better
            this.MenuButtons = new List<Objects.MenuButton>();

            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Production, Enums.Scenes.Production, true,
               new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
               }));
            
            this.MenuButtons.Add(null);

            if (GameCore.SingletonInstance.GameData.CompleteStages.Contains(Enums.Game_Stages.Earth_Shuttle_Built))
                this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Shuttle, Enums.Scenes.Shuttle, true,
                    new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                    }));
            else
                this.MenuButtons.Add(null);

            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Training, Enums.Scenes.Earth_Training, true,
                new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                }));
            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Ship_Bay, Enums.Scenes.ShipBay, true,
                new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                }));
            this.MenuButtons.Add(null);
            this.MenuButtons.Add(null);
            this.MenuButtons.Add(null);
            this.MenuButtons.Add(null);
            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Research, Enums.Scenes.Earth_Research, true,
                new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                }));
            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.GroundMaterials, Enums.Scenes.GroundMaterials, true,
                new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                }));
            this.MenuButtons.Add(new Objects.MenuButton(Enums.Menu_Buttons.Store, Enums.Scenes.Store, true,
                new List<Enums.SceneVariables>() {
                        Enums.SceneVariables.Ground
                }));
        }
    }
}