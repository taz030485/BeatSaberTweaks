using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BeatSaberTweaks
{
    class SceneUtils
    {
        public static bool isMenuScene(Scene scene)
        {
            return checkSceneByName(scene, "Menu");
        }

        public static bool isGameScene(Scene scene)
        {
            return checkSceneByName(scene, "GameCore");
        }

        private static bool checkSceneByName(Scene scene, String sceneName)
        {
            try
            {
                return (scene.name == sceneName);
            }
            catch (Exception e)
            {
                Plugin.Log("Error getting " + sceneName + " scene:" + e, Plugin.LogLevel.Error);
            }
            return false;
        }
    }
}
