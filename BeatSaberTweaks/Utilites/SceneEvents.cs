using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using IllusionPlugin;

namespace BeatSaberTweaks
{
    static class SceneEvents
    {
        static AsyncScenesLoader loader = null;
        public static AsyncScenesLoader GetSceneLoader()
        {
            if (SceneManager.GetActiveScene().name == "StandardLevelLoader")
            {
                if (loader == null)
                {
                    loader = Resources.FindObjectsOfTypeAll<AsyncScenesLoader>().FirstOrDefault();
                    if (loader != null)
                    {
                        //Console.WriteLine("Found Scene Loader");
                    }
                }
                return loader;
            }
            return null;
        }
    }
}
