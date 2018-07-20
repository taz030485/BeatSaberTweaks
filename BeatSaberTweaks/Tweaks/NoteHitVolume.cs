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

        static float normalVolume = 0;
        static float normalMissVolume = 0;

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

        public static void UpdateVolumes()
        {
            if (noteCutSoundEffect != null)
            {
                float newGoodVolume = normalVolume * Settings.NoteHitVolume;
                float newBadVolume = normalMissVolume * Settings.NoteMissVolume;
                ReflectionUtil.SetPrivateField(noteCutSoundEffect, "_goodCutVolume", newGoodVolume);
                ReflectionUtil.SetPrivateField(noteCutSoundEffect, "_badCutVolume", newBadVolume);
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex == TweakManager.GameScene)
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
    }
}
