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
        VRUIViewController _howToPlayViewController = null;
        VRUIViewController _releaseInfoViewController = null;

        TweakSettingsViewController tweakSettings = null;
        VRUIViewController left = null;
        VRUIViewController right = null;

        List<string> warningPlugins = new List<string>();

        bool CameraPlusInstalled = false;

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
                    "Beat Saber Score Mover"
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
                }

                MoveEnergyBar.OnLoad(transform);
                ScoreMover.OnLoad(transform);
                InGameClock.OnLoad(transform);
                NoteHitVolume.OnLoad(transform);
                MenuBGVolume.OnLoad(transform);
                SongDataModifer.OnLoad(transform);
            }
            else
            {
                Destroy(this);
            }
        }

        public void Update()
        {

        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex == 1)
            {
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _howToPlayViewController = ReflectionUtil.GetPrivateField<VRUIViewController>(_mainMenuViewController, "_howToPlayViewController");
                _releaseInfoViewController = ReflectionUtil.GetPrivateField<VRUIViewController>(_mainMenuViewController, "_releaseInfoViewController");

                if (warningPlugins.Count > 0)
                {
                    StartCoroutine(LoadWarning());
                }

                SetupTweakSettings();
                CreateTweakSettingsButton();
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

            var _menuMasterViewController = Resources.FindObjectsOfTypeAll<MenuMasterViewController>().First();
            var _simpleDialogPromptViewControllerPrefab = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController ,"_simpleDialogPromptViewControllerPrefab");
            SimpleDialogPromptViewController warning = Instantiate(_simpleDialogPromptViewControllerPrefab);
            warning.gameObject.SetActive(false);
            warning.Init("Plugin warning", warningText, "YES", "NO");
            warning.didFinishEvent += Warning_didFinishEvent;

            yield return new WaitForSeconds(0.1f);

            _mainMenuViewController.PresentModalViewController(warning, null, false);
        }

        private void Warning_didFinishEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= this.Warning_didFinishEvent;
            if (ok)
            {
                viewController.DismissModalViewController(null, false);
            }
            else
            {
                Application.Quit();
            }
        }

        void SetupTweakSettings()
        {
            var origianlSettingsObject = Resources.FindObjectsOfTypeAll<SettingsViewController>().FirstOrDefault();

            var tweakSettingsObject = Instantiate(origianlSettingsObject.gameObject, origianlSettingsObject.transform.parent);
            tweakSettingsObject.SetActive(false);
            tweakSettingsObject.name = "Tweak Settings View Controller";

            var originalSettings = tweakSettingsObject.GetComponent<SettingsViewController>();
            tweakSettings = tweakSettingsObject.AddComponent<TweakSettingsViewController>();
            DestroyImmediate(originalSettings);

            left = CopyScreens(origianlSettingsObject, "Left Screen", _howToPlayViewController.transform.parent);
            right = CopyScreens(origianlSettingsObject, "Right Screen", _releaseInfoViewController.transform.parent);

            tweakSettings._leftSettings = left;
            tweakSettings._rightSettings = right;

            CleanScreen(tweakSettings);
            CleanScreen(left);
            CleanScreen(right);

            SetTitle(tweakSettings, "TWEAKS");
            SetTitle(left, "PARTY MODE ONLY");
            SetTitle(right, "TWEAKS");

            Transform mainContainer = tweakSettingsObject.transform.Find("SettingsContainer");
            Transform leftContainer = left.transform.Find("SettingsContainer");
            Transform rightContainer = right.transform.Find("SettingsContainer");
            SetRectYPos(mainContainer.GetComponent<RectTransform>(), 12);
            SetRectYPos(leftContainer.GetComponent<RectTransform>(), 12);
            SetRectYPos(rightContainer.GetComponent<RectTransform>(), 12);

            CopyListSettingsController<NoteHitVolumeSettingsController>("Note Hit Volume", mainContainer);
            CopyListSettingsController<NoteMissVolumeSettingsController>("Note Miss Volume", mainContainer);
            CopyListSettingsController<MenuBGVolumeSettingsController>("Menu BG Music Volume", mainContainer);

            CopySwitchSettingsController<MoveEnergyBarSettingsController>("Move Energy Bar", rightContainer);
            CopySwitchSettingsController<MoveScoreSettingsController>("Move Score", rightContainer);
            CopySwitchSettingsController<ShowClockSettingsController>("Show Clock", rightContainer);
            CopySwitchSettingsController<Use24hrClockSettingsController>("24hr Clock", rightContainer);

            CopySwitchSettingsController<NoArrowsSettingsController>("No Arrows", leftContainer);
            CopySwitchSettingsController<OneColourSettingsController>("One Color", leftContainer);
            CopySwitchSettingsController<RemoveBombsSettingsController>("Remove Bombs", leftContainer);
            CopySwitchSettingsController<RemoveHighWallsSettingsController>("Remove High Walls", leftContainer);

            if (CameraPlusInstalled)
            {
                CopySwitchSettingsController<CameraPlusThirdPersonSettingsController>("Third Person Camera", mainContainer);
            }
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
            var tempList = container.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void SetTitle(VRUIViewController screen, string title)
        {
            var titleTransform = screen.transform.Find("Title");
            titleTransform.GetComponent<TextMeshProUGUI>().text = title;
            SetRectYPos(titleTransform.GetComponent<RectTransform>(), -2);
        }

        VRUIViewController CopyScreens(SettingsViewController view, string name, Transform parent)
        {
            var origianlScreen = ReflectionUtil.GetPrivateField<VRUIViewController>(view, "_advancedGraphicsSettingsViewController");
            var tweakScreen = Instantiate(origianlScreen.gameObject, parent);
            tweakScreen.name = name;

            var originalSettings = tweakScreen.GetComponent<VRUIViewController>();
            return originalSettings;
        }

        void CopyListSettingsController<T>(string name, Transform container) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = Instantiate(volumeSettings.gameObject, container);
            newSettingsObject.name = name;

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;
            tweakSettings.AddController(newListSettingsController);
        }

        void CopySwitchSettingsController<T>(string name, Transform container) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = Instantiate(volumeSettings.gameObject, container);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;
            tweakSettings.AddController(newListSettingsController);
        }

        private void CreateTweakSettingsButton()
        {
            var settingsButton = _mainMenuViewController.transform.Find("SettingsButton").gameObject;
            GameObject btnGo = Instantiate(settingsButton, settingsButton.transform.parent, false);
            btnGo.name = "TweaksButton";
            Button btn = btnGo.GetComponent<Button>();
            (btn.transform as RectTransform).anchoredPosition = new Vector2(-36f, 7f);
            (btn.transform as RectTransform).sizeDelta = new Vector2(28f, 10f);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Tweaks";
            DestroyImmediate(btn.GetComponent<GameEventOnUIButtonClick>());
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(ShowTweakSettings);
        }

        public void ShowTweakSettings()
        {
            _mainMenuViewController.PresentModalViewController(tweakSettings, null, false);
        }
    }
}
