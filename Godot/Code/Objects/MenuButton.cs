using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class MenuButton
    {
        public Enums.Menu_Buttons ButtonType { get; set; }
        public Enums.Scenes SceneToLoad { get; set; }
        public bool LoadScene { get; set; }
        public List<Enums.SceneVariables> SceneVariables { get; set; }

        public MenuButton(Enums.Menu_Buttons buttonType, Enums.Scenes sceneToLoad, bool loadScene, List<Enums.SceneVariables> sceneVariables)
        {
            ButtonType = buttonType;
            SceneToLoad = sceneToLoad;
            LoadScene = loadScene;
            SceneVariables = sceneVariables;
        }
    }
}
