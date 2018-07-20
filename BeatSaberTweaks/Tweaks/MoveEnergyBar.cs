using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberTweaks
{
    class MoveEnergyBar : MonoBehaviour
    {
        public static MoveEnergyBar Instance;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Move Energy Bar").AddComponent<MoveEnergyBar>().transform.parent = parent;
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
            if (Settings.MoveEnergyBar && scene.buildIndex == TweakManager.GameScene)
            {
                var energyPanel = Resources.FindObjectsOfTypeAll<GameEnergyUIPanel>().FirstOrDefault();
                var pos = energyPanel.transform.position;
                pos.y = Settings.EnergyBarHeight;
                energyPanel.transform.position = pos;
            }
        }
    }
}
