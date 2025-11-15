using Deuteros.Code.Objects;
using Deuteros.Code.Platform.Base;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Deuteros.Code.Utility
{
    public class Buttons
    {
        public static List<ButtonType> CreateButtons<ButtonType, ObjectDataType>(GridContainer buttonControlNode, Dictionary<int, ObjectDataType> objectDataList, BaseSubScene referenceScene, string clickedEventName, string codePath, string buttonPrefabName) where ButtonType : ButtonAdapter<ObjectDataType>
        {
            // Add +1 to Y on all rows except these:
            HashSet<int> noExtraPixelRows = new() { 0, 5, 8, 12, 14 }; // example 5

            var packedButton = GD.Load<PackedScene>("res://PreFabs/Buttons/" + buttonPrefabName + ".tscn");

            var script = GD.Load<Script>("res://" + codePath);

            var researchButtonControl = buttonControlNode;
            var xHeight = 0;

            var createdButtons = new List<ButtonType>();

            foreach (Node child in researchButtonControl.GetChildren())
                child.QueueFree();

            for (var i = 1; i <= 16; i++)
            {
                var createdButton = packedButton.Instantiate();
                createdButton.SetScript(script);

                if (createdButton is ButtonType typedButton)
                {
                    if (objectDataList.ContainsKey(i))
                    {
                        typedButton.ObjectData = objectDataList[i];
                        typedButton.Connect("Clicked", new Callable(referenceScene, clickedEventName));
                    }
                    createdButtons.Add(typedButton);
                    researchButtonControl.AddChild(typedButton);
                }

                var createdButton2 = packedButton.Instantiate();
                createdButton2.SetScript(script);

                if (createdButton2 is ButtonType typedButton2)
                {
                    if (objectDataList.ContainsKey(i + 16))
                    {
                        typedButton2.ObjectData = objectDataList[i + 16];
                        typedButton2.Connect("Clicked", new Callable(referenceScene, clickedEventName));
                    }
                    createdButtons.Add(typedButton2);
                    researchButtonControl.AddChild(typedButton2);
                }
            }

            return createdButtons;
        }

    }
}
