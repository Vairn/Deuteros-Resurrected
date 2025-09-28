using Godot;
using System.Linq;

namespace Deuteros.Code.Platform
{
    public partial class SceneChangeButton : Base.HoverButton
    {
        [Export(PropertyHint.Enum)]
        public Enums.Scenes TargetScene { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            this.Connect("button_up", new Callable(this, nameof(SceneChange_ButtonUp)));

            base._Ready();
        }

        private void SceneChange_ButtonUp()
        {
            //Double underscores in scene names represent a flag to pass to the scene
            var sceneNameSplit = TargetScene.ToString().Split(new string[] { "__" }, System.StringSplitOptions.None);
            var sceneFlags = sceneNameSplit.Skip(1);

            //Underscores in scene names represent a folder
            Deuteros.Code.GameCore.SingletonInstance.ChangeScene(sceneNameSplit[0].Replace("_", "/") + ".tscn", sceneFlags.ToList());
        }
    }
}