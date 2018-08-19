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

#if NewUI
using BeatSaberUI;
#endif

namespace BeatSaberTweaks
{
    public class TweakManager : MonoBehaviour
    {
        public static TweakManager Instance = null;
        MainMenuViewController _mainMenuViewController = null;
        SimpleDialogPromptViewController prompt = null;

        List<string> warningPlugins = new List<string>();

        static MainGameSceneSetupData _mainGameSceneSetupData = null;

        float carTime = 0;

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
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                DontDestroyOnLoad(gameObject);

                Console.WriteLine("Tweak Manager started.");

                MoveEnergyBar.OnLoad(transform);
                ScoreMover.OnLoad(transform);
                InGameClock.OnLoad(transform);
                NoteHitVolume.OnLoad(transform);
                MenuBGVolume.OnLoad(transform);
                OneColour.OnLoad(transform);
                SongDataModifer.OnLoad(transform);
            }
            else
            {
                Destroy(this);
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //Console.WriteLine("Loaded: " + arg0.name);
        }

        public void Update()
        {
            if (SettingsUI.isMenuScene(SceneManager.GetActiveScene()))
            {
                if (_mainMenuViewController.childViewController == null &&
                   (Input.GetAxis("TriggerLeftHand") > 0.75f) &&
                   (Input.GetAxis("TriggerRightHand") > 0.75f))
                {
                    carTime += Time.deltaTime;
                    if (carTime > 5.0f)
                    {
                        carTime = 0;
                        prompt.didFinishEvent += CarEvent;
                        prompt.Init("Flying Cars", "Turn Flying Cars?", "ON", "OFF");
                        _mainMenuViewController.PresentModalViewController(prompt, null, false);
                    }
                }
                else
                {
                    carTime = 0;
                }
            }
        }

        private void CarEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= CarEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            FlyingCar.startflyingCars = ok;
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
            //Console.WriteLine("Active: " + scene.name);
            if (SettingsUI.isMenuScene(scene))
            {
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");

                if (warningPlugins.Count > 0)
                {
                    StartCoroutine(LoadWarning());
                }

                CreateUI();
            }
        }

        private void CreateUI()
        {
            var subMenu2 = SettingsUI.CreateSubMenu("Interface Tweaks");

            var energyBar = subMenu2.AddBool("Move Energy Bar");
            energyBar.GetValue += delegate { return Settings.MoveEnergyBar; };
            energyBar.SetValue += delegate (bool value) { Settings.MoveEnergyBar = value; };

            var moveScore = subMenu2.AddBool("Move Score");
            moveScore.GetValue += delegate { return Settings.MoveScore; };
            moveScore.SetValue += delegate (bool value) { Settings.MoveScore = value; };

            var showClock = subMenu2.AddBool("Show Clock");
            showClock.GetValue += delegate { return Settings.ShowClock; };
            showClock.SetValue += delegate (bool value) { Settings.ShowClock = value; };

            var clock24hr = subMenu2.AddBool("24hr Clock");
            clock24hr.GetValue += delegate { return Settings.Use24hrClock; };
            clock24hr.SetValue += delegate (bool value)
            {
                Settings.Use24hrClock = value;
                InGameClock.UpdateClock();
            };

            var subMenu1 = SettingsUI.CreateSubMenu("Volume Tweaks");

            var noteHit = subMenu1.AddList("Note Hit Volume", volumeValues());
            noteHit.GetValue += delegate { return Settings.NoteHitVolume; };
            noteHit.SetValue += delegate (float value) { Settings.NoteHitVolume = value; };
            noteHit.FormatValue += delegate (float value) { return string.Format("{0:0.0}", value); };

            var noteMiss = subMenu1.AddList("Note Miss Volume", volumeValues());
            noteMiss.GetValue += delegate { return Settings.NoteMissVolume; };
            noteMiss.SetValue += delegate (float value) { Settings.NoteMissVolume = value; };
            noteMiss.FormatValue += delegate (float value) { return string.Format("{0:0.0}", value); };

            var menuBG = subMenu1.AddList("Menu BG Music Volume", volumeValues());
            menuBG.GetValue += delegate { return Settings.MenuBGVolume; };
            menuBG.SetValue += delegate (float value) { Settings.MenuBGVolume = value; };
            menuBG.FormatValue += delegate (float value) { return string.Format("{0:0.0}", value); };

            var subMenu3 = SettingsUI.CreateSubMenu("Party Mode Tweaks");

            var noArrows = subMenu3.AddBool("No Arrows");
            noArrows.GetValue += delegate { return Settings.NoArrows; };
            noArrows.SetValue += delegate (bool value) { Settings.NoArrows = value; };

            var oneColour = subMenu3.AddBool("One Color");
            oneColour.GetValue += delegate { return Settings.OneColour; };
            oneColour.SetValue += delegate (bool value) { Settings.OneColour = value; };

            var removeBombs = subMenu3.AddBool("Remove Bombs");
            removeBombs.GetValue += delegate { return Settings.RemoveBombs; };
            removeBombs.SetValue += delegate (bool value) { Settings.RemoveBombs = value; };

            var overrideNoteSpeed = subMenu3.AddBool("Override Note Speed");
            overrideNoteSpeed.GetValue += delegate { return Settings.OverrideJumpSpeed; };
            overrideNoteSpeed.SetValue += delegate (bool value) { Settings.OverrideJumpSpeed = value; };

            var noteSpeed = subMenu3.AddList("Note Speed", noteSpeeds());
            noteSpeed.GetValue += delegate { return Settings.NoteJumpSpeed; };
            noteSpeed.SetValue += delegate (float value) { Settings.NoteJumpSpeed = value; };
            noteSpeed.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };

            //if (HiddenNotesInstalled)
            //{
            //    var tweaks5 = SettingsUI.CreateSubMenu("Hidden Notes");
            //    tweaks5.AddToggleSetting<HiddenNotesSettingsController>("Hidden Notes");
            //}
        }

        private float[] volumeValues()
        {
            float startValue = 0.0f;
            float step = 0.1f;
            var numberOfElements = 11;
            var values = new float[numberOfElements];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = startValue + step * i;
            }
            return values;
        }

        private float[] noteSpeeds()
        {
            float startValue = 5f;
            float step = 1f;
            var numberOfElements = 16;
            var values = new float[numberOfElements];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = startValue + step * i;
            }
            return values;
        }

        public static void LogComponents(Transform t, bool includeScipts = false, string prefix = "")
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "|<-" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, includeScipts, prefix + "|");
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
            warning.didFinishEvent += Warning_didFinishEvent;
            warning.Init("Plugin warning", warningText, "YES", "NO");

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
                //Console.WriteLine("OK");
            }
            else
            {
                //Console.WriteLine("NO");
            }
            viewController.DismissModalViewController(null, false);
        }
    }
}
