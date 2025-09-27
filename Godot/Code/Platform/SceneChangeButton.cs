using Godot;

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
            Deuteros.Code.GameCore.SingletonInstance.ChangeScene(TargetScene.ToString().Replace("_", "/") + ".tscn");
        }
    }
}