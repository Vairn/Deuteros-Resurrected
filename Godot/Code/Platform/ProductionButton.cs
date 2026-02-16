using Deuteros.Code;
using Deuteros.Code.Objects;
using Godot;
using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Deuteros.Code.Platform
{
    public partial class ProductionButton : ButtonAdapter<Item>
    {
        public ProductionButton()
        {
        }

        public override void Redraw(bool dayPassed)
        {
            if (ObjectData != null)
            {
                var currentPlanet = GameCore.SingletonInstance.GetCurrentPlanet();
                Code.Objects.Factory currentFactory;

                if (currentPlanet.PlanetId == Enums.StellarBodies.earth && ((Earth)currentPlanet).GroundSelected)
                    currentFactory = ((Earth)currentPlanet).Factory;
                else
                    currentFactory = currentPlanet.Station.Factory;

                if (!currentFactory.AOC)
                {
                    AnimationState = Enums.SidePanel_Button_State_Animations.Static_Green;
                }
                else if (currentFactory.AOC && currentFactory.ProductionQueue.Any(T => T.Product.ItemType == ObjectData.ItemType) &&
                    currentFactory.ProductionQueue.Single(T => T.Product.ItemType == ObjectData.ItemType).AOCOneTime)
                {
                    AnimationState = Enums.SidePanel_Button_State_Animations.Static_Yellow;
                }
                else if (currentFactory.AOC && currentFactory.ProductionQueue.Any(T => T.Product.ItemType == ObjectData.ItemType) &&
                    currentFactory.ProductionQueue.Single(T => T.Product.ItemType == ObjectData.ItemType).AOCRepeat)
                {
                    AnimationState = Enums.SidePanel_Button_State_Animations.Static_Green;
                }
                else
                {
                    AnimationState = Enums.SidePanel_Button_State_Animations.Static_Locked;
                }
            }
            else if (ObjectData == null)
            {
                AnimationState = Enums.SidePanel_Button_State_Animations.Static_Locked;
            }

            if (AnimatedSprite != null && AnimatedSprite.Animation.ToString() != AnimationState.ToString())
                AnimatedSprite.Play(AnimationState.ToString());
        }

        public override void MouseClickedMe()
        {
            if (ObjectData != null)
                EmitSignal("Clicked", ObjectData.Research.ResearchOrder);
        }
    }
}