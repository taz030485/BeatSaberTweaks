using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace BeatSaberTweaks
{
    class OneColour : MonoBehaviour
    {
        public static OneColour Instance;

        MainSettingsModel model;
        bool rumble = false;

        public static void OnLoad(Transform parent)
        {
            Plugin.Log("One Color Load", Plugin.LogLevel.Info);
            if (Instance != null) return;
            new GameObject("One Colour").AddComponent<OneColour>().transform.parent = parent;
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

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            try
            {
                if (scene.name == "Menu")
                    {
                        if (model == null)
                        {
                            model = Resources.FindObjectsOfTypeAll<MainSettingsModel>().FirstOrDefault();
                            rumble = model.controllersRumbleEnabled;
                        }
                        model.controllersRumbleEnabled = rumble;
                    }
                if (SceneUtils.isGameScene(scene) && Settings.OneColour && TweakManager.IsPartyMode())
                {
                    StartCoroutine(WaitForLoad());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks (OneColour) done fucked up: " + e);
            }
        }

        private IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                var resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();

                if (resultsViewController == null)
                {
                    Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Found resultsViewController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
            LoadingDidFinishEvent();
        }

        private void LoadingDidFinishEvent()
        {
            try
            {
                Plugin.Log("One Color Activation Attempt", Plugin.LogLevel.Info);
                PlayerController _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
                var leftSaberType = _playerController.leftSaber.GetPrivateField<SaberTypeObject>("_saberType");
                leftSaberType.SetPrivateField("_saberType", Saber.SaberType.SaberB);
                rumble = model.controllersRumbleEnabled;
                model.controllersRumbleEnabled = false;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    } 
}
