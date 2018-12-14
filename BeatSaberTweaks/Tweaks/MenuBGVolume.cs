using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeatSaberTweaks
{
    class MenuBGVolume : MonoBehaviour
    {
        public static MenuBGVolume Instance;

        static float normalVolume = 0;
        static SongPreviewPlayer player = null;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Menu BG Volume").AddComponent<MenuBGVolume>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public static void UpdateBGVolume()
        {
            Plugin.Log("Updating BG volume", Plugin.LogLevel.DebugOnly);
            if (player != null && SceneUtils.isMenuScene(SceneManager.GetActiveScene()))
            {
                float newVolume = normalVolume * Settings.MenuBGVolume;
                ReflectionUtil.SetPrivateField(player, "_ambientVolumeScale", newVolume);
                player.CrossfadeTo(ReflectionUtil.GetPrivateField<AudioClip>(player, "_defaultAudioClip"), 0f, -1f, newVolume);
                //player.CrossfadeToDefault();
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            Plugin.Log("MenuBGVolume SceneManagerOnActiveSceneChanged", Plugin.LogLevel.DebugOnly);
            try
            {
                if (SceneUtils.isMenuScene(scene))
                {
                    player = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault();
                    if (normalVolume == 0)
                    {
                        normalVolume = ReflectionUtil.GetPrivateField<float>(player, "_ambientVolumeScale");
                    }
                    UpdateBGVolume();
                }
                else
                {
                    player = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks (MenuMusic) done fucked up: " + e);
            }
        }
    }
}
