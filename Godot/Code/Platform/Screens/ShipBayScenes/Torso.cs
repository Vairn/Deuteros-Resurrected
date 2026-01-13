using Deuteros.Code.Objects;
using Deuteros.Code.Platform.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Platform.Screens.ShipBayScenes
{
    public partial class Torso : Control
    {
        public const string ComponentSpriteBasePath = "res://Sprites//SceneSprites//Ships//";
        public Control SpriteHolder { get; set; }
        public TextureRect Component { get; set; }
        public Control Cargo { get; set; }
        public TextureButton AddSupplyPod { get; set; }
        public TextureButton AddToolPod { get; set; }
        public TextureButton AddCryoPod { get; set; }
        public Item CurrentComponent { get; set; }
        public Staff CurrentStaff { get; set; }
        public Module_Types ModuleType { get; set; }
        public int TorsoSection { get; set; }

        public delegate void ModuleChangedDelegate(Module_Types moduleType, int torsoSection);
        public event ModuleChangedDelegate ModuleChanged;

        public override void _Ready()
        {
            SpriteHolder = GetNode<Control>("SpriteHolder");
            Component = GetNode<TextureRect>("SpriteHolder/Cargo/Component");
            Cargo = GetNode<Control>("SpriteHolder/Cargo");
            AddSupplyPod = GetNode<TextureButton>("SpriteHolder/Buttons/AddSupplyPod");
            AddToolPod = GetNode<TextureButton>("SpriteHolder/Buttons/AddToolPod");
            AddCryoPod  = GetNode<TextureButton>("SpriteHolder/Buttons/AddCryoPod");

            AddSupplyPod.Pressed += () => ChangeModuleType(Module_Types.Supply);
            AddToolPod.Pressed += () => ChangeModuleType(Module_Types.Tool);
            AddCryoPod.Pressed += () => ChangeModuleType(Module_Types.Cryo);
        }

        public void UpdateState()
        {
            if (ModuleType == Module_Types.None)
            {
                Cargo.Visible = false;
            }
            else if (ModuleType == Module_Types.Tool)
            {
                Cargo.Visible = true;

                if (CurrentComponent == null)
                    Component = SpriteManager.LoadImageToTextureRect(ComponentSpriteBasePath + "Component_ToolPod.png", Component);
                else if (CurrentComponent.ItemCategory == Enums.ItemCategory.resource)
                    Component = SpriteManager.LoadImageToTextureRect(ComponentSpriteBasePath + "Component_Generic.png", Component);
                else if (CurrentComponent.ItemCategory == Enums.ItemCategory.item)
                    Component = SpriteManager.LoadImageToTextureRect(ComponentSpriteBasePath + "Component_" + CurrentComponent.ItemType.ToScreenString().Replace(".", "") + ".png", Component);
            }
            else if (ModuleType == Module_Types.Supply)
            {
                Cargo.Visible = true;
                Component = SpriteManager.LoadImageToTextureRect(ComponentSpriteBasePath + "Component_SupplyPod.png", Component);
            }
            else if (ModuleType == Module_Types.Cryo)
            {
                Cargo.Visible = true;
                Component = SpriteManager.LoadImageToTextureRect(ComponentSpriteBasePath + "Component_CryoPod.png", Component);
            }
        }

        public void ChangeModuleType(Module_Types moduleType)
        {
            if (ModuleType == moduleType)
                ModuleType = Module_Types.None;
            else
                ModuleType = moduleType;

            ModuleChanged?.Invoke(ModuleType, TorsoSection);

            UpdateState();
        }

        public void ChangeComponent(Item currentComponent)
        {
            CurrentComponent = currentComponent;

            UpdateState();
        }

        public void AddStaff(Staff currentStaff)
        {
            CurrentStaff = currentStaff;
        }
    }
}