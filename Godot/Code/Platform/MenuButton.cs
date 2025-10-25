using Deuteros.Code.Objects;
using Deuteros.Code.Platform.Helpers;
using Godot;
using System.Linq;

namespace Deuteros.Code.Platform
{
    public partial class MenuButton : SceneChangeButton
    {
        [Export(PropertyHint.Enum)]
        public Enums.Menu_Buttons ButtonType { get; set; }
        TextureRect MenuButtonImageTextureRect { get; set; }
        
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            MenuButtonImageTextureRect = GetNode<TextureRect>("Sprite");
            MenuButtonImageTextureRect = SpriteManager.LoadImageToTextureRect("Sprites/Buttons/MainMenu/" + ButtonType.ToString() + ".png", MenuButtonImageTextureRect);

            base._Ready();
        }
    }
}