using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeatSaberTweaks
{
    class SongSpeed : MonoBehaviour
    {
        public static SongSpeed Instance;

        public static GameObject SettingsObject = null;
        private static AudioTimeSyncController _audioTimeSync;
        private static AudioSource _songAudio;
        private static AudioSource _noteCutAudioSource;

        static float timeScale = 1.0f;
        public static float TimeScale
        {
            get => timeScale;
            set
            {
                timeScale = value;
                SetSpeed();
            }
        }

        private static bool _enabled;
        public static bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Song Speed").AddComponent<SongSpeed>().transform.parent = parent;
        }

        public static void SetSpeed()
        {
            var newTimeScale = timeScale;
            if (!Enabled)
            {
                newTimeScale = 1.0f;
            }

            if (_songAudio != null)
            {
                _songAudio.pitch = newTimeScale;
            }

            if (_noteCutAudioSource != null)
            {
                _noteCutAudioSource.pitch = newTimeScale;
            }

            if (_audioTimeSync != null)
            {
                if (newTimeScale < 1.0f)
                {
                    _audioTimeSync.forcedAudioSync = true;
                }else
                {
                    _audioTimeSync.forcedAudioSync = false;
                }
            }
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
            if (scene.buildIndex == TweakManager.MainScene)
            {
                if (SettingsObject == null)
                {
                    var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
                    SettingsObject = Instantiate(volumeSettings.gameObject);
                    SettingsObject.name = "Song Speed";
                    
                    VolumeSettingsController volume = SettingsObject.GetComponent<VolumeSettingsController>();
                    SongSpeedSettingsController newListSettingsController = (SongSpeedSettingsController)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(SongSpeedSettingsController), SettingsObject);
                    DestroyImmediate(volume);

                    SettingsObject.GetComponentInChildren<TMP_Text>().text = "Song Speed";
                    SettingsObject.SetActive(false);
                    DontDestroyOnLoad(SettingsObject);
                }
            }
            else if (scene.buildIndex == TweakManager.GameScene)
            {
                Enabled = TweakManager.IsPartyMode();

                if (!Enabled) return;

                _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                _songAudio = ReflectionUtil.GetPrivateField<AudioSource>(_audioTimeSync, "_audioSource");

                var noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
                var noteCutSoundEffect = ReflectionUtil.GetPrivateField<NoteCutSoundEffect>(noteCutSoundEffectManager, "_noteCutSoundEffectPrefab");
                _noteCutAudioSource = ReflectionUtil.GetPrivateField<AudioSource>(noteCutSoundEffect, "_audioSource");

                var canvas = Resources.FindObjectsOfTypeAll<HorizontalLayoutGroup>().FirstOrDefault(x => x.name == "Buttons")
                    .transform.parent;
                canvas.gameObject.AddComponent<SongSpeedSettingsCreator>();

                SetSpeed();
            }
        }
    }

    public class SongSpeedSettingsCreator : MonoBehaviour
    {
        private GameObject _speedSettings;

        private void OnEnable()
        {
            if (!SongSpeed.Enabled) return;
            _speedSettings = Instantiate(SongSpeed.SettingsObject, transform);
            _speedSettings.SetActive(true);
            _speedSettings.GetComponent<SongSpeedSettingsController>().Init();
            var rectTransform = (RectTransform)_speedSettings.transform;
            rectTransform.anchorMin = Vector2.right * 0.5f;
            rectTransform.anchorMax = Vector2.right * 0.5f;
            rectTransform.anchoredPosition = new Vector2(0, rectTransform.sizeDelta.y * 1.5f);
        }

        private void OnDisable()
        {
            if (_speedSettings == null) return;
            DestroyImmediate(_speedSettings);
        }
    }
}
