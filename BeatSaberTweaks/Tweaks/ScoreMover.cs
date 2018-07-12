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
            if (Settings.MoveScore && scene.buildIndex == 4)
            {
                var scorePanel = Resources.FindObjectsOfTypeAll<ScoreUIController>().FirstOrDefault();
                scorePanel.transform.position = new Vector3(3.3f, 2.5f, 7.0f);
                scorePanel.transform.rotation = Quaternion.identity;
                scorePanel.transform.localScale *= 4;
            }
        }
    }
}
