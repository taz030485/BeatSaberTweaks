#if !NewUI
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
using HMUI;

namespace BeatSaberTweaks
{
    public class ListViewController : ListSettingsController
    {
        public delegate float GetFloat();
        public event GetFloat GetValue;

        public delegate void SetFloat(float value);
        public event SetFloat SetValue;

        public delegate string StringForValue(float value);
        public event StringForValue FormatValue;

        protected float[] values;

        public void SetValues(float[] values)
        {
            this.values = values;
        }

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = values.Length;
            float value = 0;
            if (GetValue != null)
            {
                value = GetValue();
            }

            idx = numberOfElements - 1;
            for (int j = 0; j < values.Length; j++)
            {
                if (value == values[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            if (SetValue != null)
            {
                SetValue(values[idx]);
            }
        }

        protected override string TextForValue(int idx)
        {
            string text = "?";
            if (FormatValue != null)
            {
                text = FormatValue(values[idx]);
            }
            return text;
        }
    }

    public class BoolViewController : SwitchSettingsController
    {
        public delegate bool GetBool();
        public event GetBool GetValue;

        public delegate void SetBool(bool value);
        public event SetBool SetValue;

        protected override bool GetInitValue()
        {
            bool value = false;
            if (GetValue != null)
            {
                value = GetValue();
            }
            return value;
        }

        protected override void ApplyValue(bool value)
        {
            if (SetValue != null)
            {
                SetValue(value);
            }
        }

        protected override string TextForValue(bool value)
        {
            return (value) ? "ON" : "OFF";
        }
    }

    public class SubMenu
    {
        public Transform transform;

        public SubMenu(Transform transform)
        {
            this.transform = transform;
        }

        public BoolViewController AddBool(string name)
        {
            return AddToggleSetting<BoolViewController>(name);
        }

        public ListViewController AddList(string name, float[] values)
        {
            var view = AddListSetting<ListViewController>(name);
            view.SetValues(values);
            return view;
        }

        public T AddListSetting<T>(string name) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newListSettingsController;
        }

        public T AddToggleSetting<T>(string name) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newToggleSettingsController;
        }
    }

    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance = null;
        static bool ready = false;
        public static bool Ready
        {
            get => ready;
        }

        static MainMenuViewController _mainMenuViewController = null;
        static SettingsNavigationController settingsMenu = null;
        static MainSettingsMenuViewController mainSettingsMenu = null;
        static MainSettingsTableView _mainSettingsTableView = null;
        static TableView subMenuTableView = null;
        static MainSettingsTableCell tableCell = null;
        static TableViewHelper subMenuTableViewHelper;

        static Transform othersSubmenu = null;

        static SimpleDialogPromptViewController prompt = null;

        static Button _pageUpButton = null;
        static Button _pageDownButton = null;
        static Vector2 buttonOffset = new Vector2(24, 0);


        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("SettingsUI").AddComponent<SettingsUI>();
        }

        public static bool isMenuScene(Scene scene)
        {
            return (scene.name == "Menu");
        }

        public static bool isGameScene(Scene scene)
        {
            //return scene.name.Contains("Environment");
            return (scene.name == "StandardLevelLoader");
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
                if (isMenuScene(scene))
                {
                    SetupUI();

                    //var testSub = CreateSubMenu("Test 1");
                    //var testSub2 = CreateSubMenu("Test 2");
                    //var testSub3 = CreateSubMenu("Test 3");
                    //var testSub4 = CreateSubMenu("Test 4");
                    //var testSub5 = CreateSubMenu("Test 5");
                    //var testSub6 = CreateSubMenu("Test 6");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SettingsUI done fucked up: " + e);
            }
        }

        private static void SetupUI()
        {
            if (mainSettingsMenu == null)
            {
                ready = false;
            }

            if (!Ready)
            {
                try
                {
                    var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                    prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");

                    _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                    settingsMenu = Resources.FindObjectsOfTypeAll<SettingsNavigationController>().FirstOrDefault();
                    mainSettingsMenu = Resources.FindObjectsOfTypeAll<MainSettingsMenuViewController>().FirstOrDefault();
                    _mainSettingsTableView = mainSettingsMenu.GetPrivateField<MainSettingsTableView>("_mainSettingsTableView");
                    subMenuTableView = _mainSettingsTableView.GetComponentInChildren<TableView>();
                    subMenuTableViewHelper = subMenuTableView.gameObject.AddComponent<TableViewHelper>();
                    othersSubmenu = settingsMenu.transform.Find("Others");

                    //var buttons = settingsMenu.transform.Find("Buttons");
                    //RectTransform okButton = (RectTransform)buttons.Find("OkButton"); //{x: -17, y: 6}
                    //RectTransform CancelButton = (RectTransform)buttons.Find("CancelButton"); // {x: 0, y: 6}
                    //RectTransform ApplyButton = (RectTransform)buttons.Find("ApplyButton"); // {x: 17, y: 6}

                    //okButton.anchoredPosition += buttonOffset;
                    //CancelButton.anchoredPosition += buttonOffset;
                    //ApplyButton.anchoredPosition += buttonOffset;

                    if (_mainSettingsTableView != null)
                    {
                        AddPageButtons();
                    }

                    if (tableCell == null)
                    {
                        tableCell = Resources.FindObjectsOfTypeAll<MainSettingsTableCell>().FirstOrDefault();
                        // Get a refence to the Settings Table cell text in case we want to change font size, etc
                        var text = tableCell.GetPrivateField<TextMeshProUGUI>("_settingsSubMenuText");
                    }

                    ready = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Beat Saver UI: Oops - " + e.Message);
                }
            }
        }

        static void AddPageButtons()
        {
            RectTransform viewport = _mainSettingsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
            viewport.anchorMin = new Vector2(0f, 0.5f);
            viewport.anchorMax = new Vector2(1f, 0.5f);
            viewport.sizeDelta = new Vector2(0f, 48f);
            viewport.anchoredPosition = new Vector2(0f, 0f);

            if (_pageUpButton == null)
            {
                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), _mainSettingsTableView.transform, false);
                (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 24f);
                _pageUpButton.interactable = true;
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollUp();
                });
            }

            if (_pageDownButton == null)
            {
                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), _mainSettingsTableView.transform, false);
                (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -24f);
                _pageDownButton.interactable = true;
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollDown();
                });
            }

            subMenuTableViewHelper._pageUpButton = _pageUpButton;
            subMenuTableViewHelper._pageDownButton = _pageDownButton;

            //subMenuTableView.SetPrivateField("_pageUpButton", _pageUpButton);
            //subMenuTableView.SetPrivateField("_pageDownButton", _pageDownButton);
        }

        public static void LogComponents(Transform t, string prefix = "=", bool includeScipts = false)
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "-->" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, prefix + "=", includeScipts);
            }
        }

        public static SubMenu CreateSubMenu(string name)
        {
            if (!isMenuScene(SceneManager.GetActiveScene()))
            {
                Console.WriteLine("Cannot create settings menu when no in the main scene.");
                return null;
            }

            SetupUI();

            var subMenuGameObject = Instantiate(othersSubmenu.gameObject, othersSubmenu.transform.parent);
            Transform mainContainer = CleanScreen(subMenuGameObject.transform);

            var newSubMenuInfo = new SettingsSubMenuInfo();
            newSubMenuInfo.SetPrivateField("_menuName", name);
            newSubMenuInfo.SetPrivateField("_controller", subMenuGameObject.GetComponent<VRUIViewController>());

            var subMenuInfos = mainSettingsMenu.GetPrivateField<SettingsSubMenuInfo[]>("_settingsSubMenuInfos").ToList();
            subMenuInfos.Add(newSubMenuInfo);
            mainSettingsMenu.SetPrivateField("_settingsSubMenuInfos", subMenuInfos.ToArray());

            SubMenu menu = new SubMenu(mainContainer);
            return menu;
        }

        static Transform CleanScreen(Transform screen)
        {
            var container = screen.Find("Content").Find("SettingsContainer");
            var tempList = container.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
            return container;
        }
    }
}
#endif