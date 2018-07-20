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
    public class TweakManager : MonoBehaviour
    {
        public static TweakManager Instance = null;
        MainMenuViewController _mainMenuViewController = null;
        SimpleDialogPromptViewController prompt = null;

        List<string> warningPlugins = new List<string>();

        static MainGameSceneSetupData _mainGameSceneSetupData = null;

        bool CameraPlusInstalled = false;
        bool HiddenNotesInstalled = false;

        public const int MainScene = 1;
        public const int GameScene = 5;

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

                string[] oldPlugins = new string[] 
                {
                    "In Game Time",
                    "Move Energy Bar",
                    "Note Hit Volume",
                    "Beat Saber Score Mover",
                    "Practice Plugin"
                };

                foreach (var plugin in IllusionInjector.PluginManager.Plugins)
                {
                    if (oldPlugins.Contains(plugin.Name))
                    {
                        warningPlugins.Add(plugin.Name);
                        Console.WriteLine("WARNING:" + plugin.Name + " is not needed anymore. Please remove it. BeatSaberTweaks has replaced it.");
                    }

                    if (plugin.Name == "CameraPlus")
                    {
                        CameraPlusInstalled = true;
                    }

                    if (plugin.Name == "Hidden Notes")
                    {
                        HiddenNotesInstalled = true;
                    }
                }

                MoveEnergyBar.OnLoad(transform);
                ScoreMover.OnLoad(transform);
                InGameClock.OnLoad(transform);
                NoteHitVolume.OnLoad(transform);
                MenuBGVolume.OnLoad(transform);
                OneColour.OnLoad(transform);
                SongDataModifer.OnLoad(transform);
                SongSpeed.OnLoad(transform);
            }
            else
            {
                Destroy(this);
            }
        }

        public void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == MainScene)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //string path = "Testing";
                    //Console.WriteLine(path);
                    //prompt.didFinishEvent += Prompt_didFinishEvent;
                    //prompt.Init("Test Prompt", path, "ON", "OFF");
                    //_mainMenuViewController.PresentModalViewController(prompt, null, false);
                    NewSetup();
                }

                if (_mainMenuViewController.childViewController != null)
                {
                    return;
                }

                if (Input.GetKey((KeyCode)ConInput.Vive.LeftTrackpadPress) &&
                    Input.GetKey((KeyCode)ConInput.Vive.RightTrackpadPress) &&
                    Input.GetKeyDown((KeyCode)ConInput.Vive.RightTrigger))
                {
                    prompt.didFinishEvent += OneColorEvent;
                    prompt.Init("One Color", "Turn One color mode on?", "ON", "OFF");
                    _mainMenuViewController.PresentModalViewController(prompt, null, false);
                    return;
                }

                if (Input.GetKey((KeyCode)ConInput.Vive.LeftTrackpadPress) &&
                    Input.GetKeyDown((KeyCode)ConInput.Vive.RightTrigger))
                {
                    prompt.didFinishEvent += RemoveBombsEvent;
                    prompt.Init("Remove Bombs", "Turn Remove Bombs mode on?", "ON", "OFF");
                    _mainMenuViewController.PresentModalViewController(prompt, null, false);
                    return;
                }

                if (Input.GetKey((KeyCode)ConInput.Vive.RightTrackpadPress) &&
                    Input.GetKeyDown((KeyCode)ConInput.Vive.RightTrigger))
                {
                    prompt.didFinishEvent += NoArrowsEvent;
                    prompt.Init("No Arrows", "Turn No Arrows mode on?", "ON", "OFF");
                    _mainMenuViewController.PresentModalViewController(prompt, null, false);
                    return;
                }
            }
        }

        private void NoArrowsEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= NoArrowsEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            Settings.NoArrows = ok;
            viewController.DismissModalViewController(null, false);
        }

        private void RemoveBombsEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= RemoveBombsEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            Settings.RemoveBombs = ok;
            viewController.DismissModalViewController(null, false);
        }

        private void OneColorEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= OneColorEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            Settings.OneColour = ok;
            viewController.DismissModalViewController(null, false);
        }

        public static bool IsPartyMode()
        {
            if (_mainGameSceneSetupData == null)
            {
                _mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<MainGameSceneSetupData>().FirstOrDefault();
            }

            if (_mainGameSceneSetupData == null)
            {
                return false;
            }

            return _mainGameSceneSetupData.gameplayMode == GameplayMode.PartyStandard;
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex == MainScene)
            {
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");

                if (warningPlugins.Count > 0)
                {
                    StartCoroutine(LoadWarning());
                }
                NewSetup();
            }
        }

        private void NewSetup()
        {
            var tweaks1 = SettingsUI.CreateSubMenu("Volume Tweaks");
            SettingsUI.AddListSetting<NoteHitVolumeSettingsController>("Note Hit Volume", tweaks1);
            SettingsUI.AddListSetting<NoteMissVolumeSettingsController>("Note Miss Volume", tweaks1);
            SettingsUI.AddListSetting<MenuBGVolumeSettingsController>("Menu BG Music Volume", tweaks1);

            var tweaks2 = SettingsUI.CreateSubMenu("Interface Tweaks");
            SettingsUI.AddToggleSetting<MoveEnergyBarSettingsController>("Move Energy Bar", tweaks2);
            SettingsUI.AddToggleSetting<MoveScoreSettingsController>("Move Score", tweaks2);
            SettingsUI.AddToggleSetting<ShowClockSettingsController>("Show Clock", tweaks2);
            SettingsUI.AddToggleSetting<Use24hrClockSettingsController>("24hr Clock", tweaks2);

            var tweaks3 = SettingsUI.CreateSubMenu("Party Mode Tweaks");
            SettingsUI.AddToggleSetting<NoArrowsSettingsController>("No Arrows", tweaks3);
            SettingsUI.AddToggleSetting<OneColourSettingsController>("One Color", tweaks3);
            SettingsUI.AddToggleSetting<RemoveBombsSettingsController>("Remove Bombs", tweaks3);
            SettingsUI.AddListSetting<SongSpeedSettingsController>("Song Speed", tweaks3);
            //CopySwitchSettingsController<OverrideJumpSpeedSettingsController>("Override Note Speed", tweaks3);
            //CopyListSettingsController<NoteJumpSpeedSettingsController>("Note Speed", tweaks3);

            if (CameraPlusInstalled)
            {
                var tweaks4 = SettingsUI.CreateSubMenu("Camera Plus");
                SettingsUI.AddToggleSetting<CameraPlusThirdPersonSettingsController>("Third Person Camera", tweaks4);
            }

            if (HiddenNotesInstalled)
            {
                var tweaks5 = SettingsUI.CreateSubMenu("Hidden Notes");
                SettingsUI.AddToggleSetting<HiddenNotesSettingsController>("Hidden Notes", tweaks5);
            }
        }

        void LogComponents(Transform t, string prefix)
        {
            Console.WriteLine(prefix + ">" + t.name);

            //foreach (var comp in t.GetComponents<MonoBehaviour>())
            //{
            //    Console.WriteLine(prefix + "-->" + comp.GetType());
            //}

            foreach (Transform child in t)
            {
                LogComponents(child, prefix + "=");
            }
        }

        IEnumerator LoadWarning()
        {
            string warningText = "The folling plugins are obsolete:\n";

            foreach(var text in warningPlugins)
            {
                warningText += text + ", ";
            }
            warningText = warningText.Substring(0, warningText.Length - 2);

            warningText +="\nPlease remove them before playing or you my encounter errors.\nDo you want to continue?";

            yield return new WaitForSeconds(0.1f);

            var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
            var warning = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController , "_simpleDialogPromptViewController");
            warning.gameObject.SetActive(false);
            warning.Init("Plugin warning", warningText, "YES", "NO");
            warning.didFinishEvent += Warning_didFinishEvent;

            yield return new WaitForSeconds(0.1f);

            _mainMenuViewController.PresentModalViewController(warning, null, false);
        }

        private void Warning_didFinishEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= Warning_didFinishEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            if (ok)
            {
                viewController.DismissModalViewController(null, false);
            }
            else
            {
                Application.Quit();
            }
        }

        private void Prompt_didFinishEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= Prompt_didFinishEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            if (ok)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("NO");
            }
            viewController.DismissModalViewController(null, false);
        }
    }
}
