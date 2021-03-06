﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using System.Threading;

namespace BeatSaberTweaks
{
    public class InGameClock : MonoBehaviour
    {
        public static InGameClock Instance;

        private static GameObject ClockCanvas = null;
        private static TextMeshProUGUI text = null;
        private static Vector3 timePos;
        private static Quaternion timeRot;
        private static float timeSize;

        float timer = 0;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("In Game Time").AddComponent<InGameClock>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
                timePos = Settings.ClockPosition;
                timeRot = Settings.ClockRotation;
                timeSize = Settings.ClockFontSize;
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
                if (SettingsUI.isMenuScene(scene) && ClockCanvas == null)
                {
                    ClockCanvas = new GameObject();
                    DontDestroyOnLoad(ClockCanvas);
                    ClockCanvas.AddComponent<Canvas>();

                    ClockCanvas.name = "Clock Canvas";
                    ClockCanvas.transform.position = timePos;
                    ClockCanvas.transform.rotation = timeRot;
                    ClockCanvas.transform.localScale = new Vector3(0.02f, 0.02f, 1.0f);

                    var textGO = new GameObject();
                    textGO.transform.SetParent(ClockCanvas.transform);
                    textGO.transform.localPosition = Vector3.zero;
                    textGO.transform.localRotation = Quaternion.identity;
                    textGO.transform.localScale = Vector3.one;

                    text = textGO.AddComponent<TextMeshProUGUI>();
                    text.name = "Clock Text";
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = timeSize;
                    text.text = "";

                    UpdateClock();

                    ClockCanvas.SetActive(Settings.ShowClock);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks (Clock) done fucked up: " + e);
            }
        }

        public void Update()
        {
            if (ClockCanvas != null && Settings.ShowClock != ClockCanvas.activeSelf)
            {
                ClockCanvas.SetActive(Settings.ShowClock);
            }

            timer += Time.deltaTime;
            if (text != null && timer > 60.0f)
            {
                timer = 0;
                UpdateClock();
            }
        }

        public static void UpdateClock()
        {
            string time;
            if (Settings.Use24hrClock)
            {
                time = DateTime.Now.ToString("HH:mm");
            }
            else
            {
                time = DateTime.Now.ToString("h:mm tt");
            }
            text.text = time;
        }
    }
}
