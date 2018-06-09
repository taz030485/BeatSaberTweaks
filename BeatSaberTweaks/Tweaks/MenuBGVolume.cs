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
    class MenuBGVolume : MonoBehaviour
    {
        public static MenuBGVolume Instance;

        float normalVolume = 0;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Menu BG Volume").AddComponent<MenuBGVolume>().transform.parent = parent;
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
                var songPreviewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault();
                if (normalVolume == 0)
                {
                    normalVolume = ReflectionUtil.GetPrivateField<float>(songPreviewPlayer, "_ambientVolumeScale");
                }
                float newVolume = normalVolume * Settings.MenuBGVolume;
                ReflectionUtil.SetPrivateField(songPreviewPlayer, "_ambientVolumeScale", newVolume);
                songPreviewPlayer.CrossfadeTo(ReflectionUtil.GetPrivateField<AudioClip>(songPreviewPlayer,"_defaultAudioClip"), 0f, -1f, newVolume);
            }
        }
    }
}
