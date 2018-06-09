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
    public class NoteHitVolume : MonoBehaviour
    {
        public static NoteHitVolume Instance;
        private static NoteCutSoundEffect noteCutSoundEffect;

        float normalVolume = 0;
        float normalMissVolume = 0;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Note Volume").AddComponent<NoteHitVolume>().transform.parent = parent;
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

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex == 1)
            {
                var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
                volumeSettings.gameObject.SetActive(false);

                var SettingsObject = Object.Instantiate(volumeSettings.gameObject,volumeSettings.transform.parent);
                SettingsObject.SetActive(false);
                SettingsObject.name = "Note Hit Volume";

                var SettingsMissObject = Object.Instantiate(volumeSettings.gameObject, volumeSettings.transform.parent);
                SettingsMissObject.SetActive(false);
                SettingsMissObject.name = "Note Miss Volume";

                volumeSettings.gameObject.SetActive(true);

                var volume = SettingsObject.GetComponent<VolumeSettingsController>();
                ReflectionUtil.CopyComponent(volume, typeof(SimpleSettingsController), typeof(HitVolumeSettingsController), SettingsObject);
                Object.DestroyImmediate(volume);

                var missVolume = SettingsMissObject.GetComponent<VolumeSettingsController>();
                ReflectionUtil.CopyComponent(missVolume, typeof(SimpleSettingsController), typeof(MissVolumeSettingsController), SettingsMissObject);
                Object.DestroyImmediate(missVolume);

                SettingsObject.GetComponentInChildren<TMP_Text>().text = "Note Hit Volume";
                SettingsObject.SetActive(true);
                SettingsObject.GetComponent<HitVolumeSettingsController>().Init();

                SettingsMissObject.GetComponentInChildren<TMP_Text>().text = "Note Miss Volume";
                SettingsMissObject.SetActive(true);
                SettingsMissObject.GetComponent<MissVolumeSettingsController>().Init();

                SetRectYPos(volumeSettings.transform.parent.GetComponent<RectTransform>(), 12);
                SetRectYPos(volumeSettings.transform.parent.parent.Find("Title").GetComponent<RectTransform>(), -2);
            }
            else
            {
                bool pooled = false;
                if (noteCutSoundEffect == null)
                {
                    var noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
                    noteCutSoundEffect = ReflectionUtil.GetPrivateField<NoteCutSoundEffect>(noteCutSoundEffectManager, "_noteCutSoundEffectPrefab");
                    pooled = true;
                }

                if (normalVolume == 0)
                {
                    normalVolume = ReflectionUtil.GetPrivateField<float>(noteCutSoundEffect, "_goodCutVolume");
                    normalMissVolume = ReflectionUtil.GetPrivateField<float>(noteCutSoundEffect, "_badCutVolume");
                }

                float newGoodVolume = normalVolume * Settings.NoteHitVolume;
                float newBadVolume = normalMissVolume * Settings.NoteMissVolume;
                ReflectionUtil.SetPrivateField(noteCutSoundEffect, "_goodCutVolume", newGoodVolume);
                ReflectionUtil.SetPrivateField(noteCutSoundEffect, "_badCutVolume", newBadVolume);

                if (pooled)
                {
                    var pool = Resources.FindObjectsOfTypeAll<NoteCutSoundEffect>();
                    foreach (var effect in pool)
                    {
                        if (effect.name.Contains("Clone"))
                        {
                            ReflectionUtil.SetPrivateField(effect, "_goodCutVolume", newGoodVolume);
                            ReflectionUtil.SetPrivateField(effect, "_badCutVolume", newBadVolume);
                        }
                    }
                }
            }
        }

        void SetRectYPos(RectTransform rect, float y)
        {
            var pos = rect.anchoredPosition;
            pos.y = y;
            rect.anchoredPosition = pos;
        }
    }
}
