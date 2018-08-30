using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberTweaks
{
    class ScoreMover : MonoBehaviour
    {
        public static ScoreMover Instance;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Score Mover").AddComponent<ScoreMover>().transform.parent = parent;
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
                if (Settings.MoveScore && SettingsUI.isGameScene(scene))
                {
                    var loader = SceneEvents.GetSceneLoader();
                    if (loader != null)
                    {
                        loader.loadingDidFinishEvent += LoadingDidFinishEvent;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks (MoveScore) done fucked up: " + e);
            }
        }

        private void LoadingDidFinishEvent()
        {
            var scorePanel = Resources.FindObjectsOfTypeAll<ScoreUIController>().FirstOrDefault();
            scorePanel.transform.position = Settings.ScorePosition;
            scorePanel.transform.rotation = Quaternion.identity;
            scorePanel.transform.localScale *= Settings.ScoreSize;
        }
    }
}
