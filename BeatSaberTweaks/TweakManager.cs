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
    class TweakManager : MonoBehaviour
    {
        public static TweakManager Instance = null;
        TweakSettingsViewController tweakSettings = null;
        MainMenuViewController _mainMenuViewController = null;

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

                string[] oldPlugins = new string[] { "In Game Time", "Move Energy Bar", "Note Hit Volume" };

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
            if (scene.buildIndex == 1)
            {
                if (tweakSettings == null)
                {   
                    SetupTweakSettings();
                }

                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                CreateTweakSettingsButton();
            }
        }

        void SetupTweakSettings()
        {
            var origianlSettingsObject = Resources.FindObjectsOfTypeAll<SettingsViewController>().FirstOrDefault();

            var tweakSettingsObject = Instantiate(origianlSettingsObject.gameObject, transform);
            tweakSettingsObject.SetActive(false);
            tweakSettingsObject.name = "Tweak Settings View Controller";

            var originalSettings = tweakSettingsObject.GetComponent<SettingsViewController>();
            //tweakSettings = (TweakSettingsViewController)ReflectionUtil.CopyComponent(originalSettings, typeof(SettingsViewController), typeof(TweakSettingsViewController), tweakSettingsObject);
            tweakSettings = tweakSettingsObject.AddComponent<TweakSettingsViewController>();
            DestroyImmediate(originalSettings);


            //var left = CopyScreens(origianlSettingsObject, "Left Screen");
            //var right = CopyScreens(origianlSettingsObject, "Right Screen");

            CleanScreen(tweakSettings);
            //CleanScreen(left);
            //CleanScreen(right);

            //Transform mainContainer = tweakSettingsObject.transform.Find("SettingsContainer");

            //var noteHitVolume = CopyListSettingsController<MenuBGVolumeSettingsController>("Note Hit Volume");
            //noteHitVolume.transform.parent = mainContainer;

            //var noteMissVolume = CopyListSettingsController<MenuBGVolumeSettingsController>("Note Miss Volume");
            //noteMissVolume.transform.parent = mainContainer;

            //var menuBG = CopyListSettingsController<MenuBGVolumeSettingsController>("Menu BG Music Volume");
            //menuBG.transform.parent = mainContainer;

            //var moveEnergy = CopySwitchSettingsController<MoveEnergyBarSettingsController>("Move Energy Bar")

            //var showClock = CopySwitchSettingsController<ShowClockSettingsController>("Show Clock");

            //var use24hr = CopySwitchSettingsController<Use24hrClockSettingsController>("24hr Clock");
        }

        void SetRectYPos(RectTransform rect, float y)
        {
            var pos = rect.anchoredPosition;
            pos.y = y;
            rect.anchoredPosition = pos;
        }

        void CleanScreen(VRUIViewController screen)
        {
            var container = screen.transform.Find("SettingsContainer");
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        void LogComponets(Component go)
        {
            Console.WriteLine("== Begin " + go.name + " ==");
            foreach (var thing in go.GetComponentsInChildren<Component>())
            {
                Console.WriteLine(thing.name + " " + thing.transform.parent + " " + thing.GetType().ToString());
            }
            Console.WriteLine("== End " + go.name + " ==");
            Console.WriteLine("");
        }

        VRUIViewController CopyScreens(SettingsViewController view, string name)
        {
            var origianlScreen = ReflectionUtil.GetPrivateField<VRUIViewController>(view, "_advancedGraphicsSettingsViewController");
            var tweakScreen = Instantiate(origianlScreen.gameObject, transform);
            tweakScreen.SetActive(false);
            tweakScreen.name = name;

            var originalSettings = tweakScreen.GetComponent<VRUIViewController>();
            //tweakSettings = (TweakSettingsViewController)ReflectionUtil.CopyComponent(originalSettings, typeof(SettingsViewController), typeof(TweakSettingsViewController), tweakSettingsObject);
            //DestroyImmediate(originalSettings);

            return originalSettings;
        }

        T CopyListSettingsController<T>(string name) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            volumeSettings.gameObject.SetActive(false);

            var SettingsObject = Instantiate(volumeSettings.gameObject, volumeSettings.transform.parent);
            SettingsObject.SetActive(false);
            SettingsObject.name = name;

            volumeSettings.gameObject.SetActive(true);

            var volume = SettingsObject.GetComponent<VolumeSettingsController>();
            var newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), SettingsObject);
            DestroyImmediate(volume);

            SettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            tweakSettings.tweakedSettingsControllers.Add(newListSettingsController);

            return newListSettingsController;
        }

        T CopySwitchSettingsController<T>(string name) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            volumeSettings.gameObject.SetActive(false);

            var SettingsObject = Object.Instantiate(volumeSettings.gameObject, volumeSettings.transform.parent);
            SettingsObject.SetActive(false);
            SettingsObject.name = name;

            volumeSettings.gameObject.SetActive(true);

            var volume = SettingsObject.GetComponent<WindowModeSettingsController>();
            var newSwitchSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), SettingsObject);
            DestroyImmediate(volume);

            SettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            tweakSettings.tweakedSettingsControllers.Add(newSwitchSettingsController);

            return newSwitchSettingsController;
        }

        public void Update()
        {

        }

        private void CreateTweakSettingsButton()
        {
            var settingsButton = _mainMenuViewController.transform.Find("SettingsButton").GetComponent<Button>();

            Button btn = Instantiate(settingsButton, settingsButton.transform.parent, false);
            DestroyImmediate(btn.GetComponent<GameEventOnUIButtonClick>());
            btn.onClick = new Button.ButtonClickedEvent();

            (btn.transform as RectTransform).anchoredPosition = new Vector2(-36f, 7f);
            (btn.transform as RectTransform).sizeDelta = new Vector2(28f, 10f);

            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Tweaks";

            btn.onClick.AddListener(onClick);   
        }

        void onClick()
        {
            Console.WriteLine("Tweaks button pressed.");
            Console.WriteLine("_mainMenuViewController name: " + _mainMenuViewController.name);
            Console.WriteLine("tweakSettings name: " + tweakSettings.name);
            _mainMenuViewController.PresentModalViewController(tweakSettings, null, false);
        }
    }
}
