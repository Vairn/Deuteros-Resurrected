using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deuteros.Code.Platform
{
    public partial class SceneChangeButton : Base.HoverButton
    {
        [Export]
        public Enums.Scenes TargetScene;
        public List<Action> ClickActions { get; set; }
        [Export]
        public Godot.Collections.Array<Enums.SceneVariables> SceneVariables { get; set; } = new Godot.Collections.Array<Enums.SceneVariables>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            this.Pressed += this.SceneChange_ButtonUp;

            base._Ready();
        }

        private void SceneChange_ButtonUp()
        {
            if (TargetScene != Enums.Scenes.None)
            {
                //Double underscores in scene names represent a flag to pass to the scene
                var sceneNameSplit = TargetScene.ToString().Split(new string[] { "__" }, System.StringSplitOptions.None);

                //Underscores in scene names represent a folder
                Deuteros.Code.GameCore.SingletonInstance.ChangeScene(sceneNameSplit[0].Replace("_", "/") + ".tscn", SceneVariables.ToList());
            }
        }
    }
}