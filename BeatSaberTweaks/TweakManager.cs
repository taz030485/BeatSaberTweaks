using System;
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
    class TweakManager : MonoBehaviour
    {
        public static TweakManager Instance = null;

        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("Tweak Manager").AddComponent<TweakManager>();
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);

                Console.WriteLine("Tweak Manager started.");

                string[] oldPlugins = new string[] { "In Game Time", "Move Energy Bar", "Note Hit Volume"};

                foreach (var plugin in IllusionInjector.PluginManager.Plugins)
                {
                    if (oldPlugins.Contains(plugin.Name))
                    {
                        Console.WriteLine("WARNING:" + plugin.Name + " is not needed anymore. Please remove it. BeatSaberTweaks has replaced it.");
                    }
                }

                MoveEnergyBar.OnLoad(transform);
                InGameClock.OnLoad(transform);
                NoteHitVolume.OnLoad(transform);
                MenuBGVolume.OnLoad(transform);
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {

        }

        public void Update()
        {

        }
    }
}
