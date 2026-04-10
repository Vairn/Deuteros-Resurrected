using Godot;

namespace Deuteros.Code.Platform.Base
{
    public partial class HoverButton : Button
    {
        [Export]
        public string HoverText { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            //if (HoverText != "")
            //{
                this.Connect("mouse_entered", new Callable(this, nameof(MouseHover_Enter)));
                this.Connect("mouse_exited", new Callable(this, nameof(MouseHover_Exit)));
            //}
        }

        private void MouseHover_Enter()
        {
            Deuteros.Code.GameCore.HoverText = HoverText;
        }

        private void MouseHover_Exit()
        {
            Deuteros.Code.GameCore.HoverText = "";
        }
    }
}

