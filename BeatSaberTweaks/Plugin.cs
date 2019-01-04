using System;
using System.Collections.Generic;
using System.Linq;
using IllusionPlugin;
using UnityEngine;
namespace BeatSaberTweaks
{
    public class Plugin : IPlugin
    {
        public string Name => "Beat Saber Tweaks";
#if NewUI
        public string Version => "4.0";
#else
        public string Version => "3.3.2";
#endif

        private bool _init = false;
        private BeatmapCharacteristicSelectionViewController _characteristicViewController;
        private static SoloFreePlayFlowCoordinator _soloFlowCoordinator;
        private static PartyFreePlayFlowCoordinator _partyFlowCoordinator;

        private static PracticeViewController _practiceViewController;
        private static StandardLevelDetailViewController _soloDetailView;
        private static bool debug = true;
        public static bool party { get; private set; } = false;

        public static string _gameplayMode { get; private set; }
        public enum LogLevel
        {
            DebugOnly = 0,
            Info = 1,
            Error = 2
        }

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            Settings.Load();
            //SettingsUI.OnLoad();
            TweakManager.OnLoad();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
           if(arg1.name == "Menu")
            {
                if (_characteristicViewController == null)
                {
                    _characteristicViewController = Resources.FindObjectsOfTypeAll<BeatmapCharacteristicSelectionViewController>().FirstOrDefault();
                    if (_characteristicViewController == null) return;

                    _characteristicViewController.didSelectBeatmapCharacteristicEvent += _characteristicViewController_didSelectBeatmapCharacteristicEvent;
                }

                if (_soloFlowCoordinator == null)
                {
                    _soloFlowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().FirstOrDefault();
                    if (_soloFlowCoordinator == null) return;
                    _soloDetailView = _soloFlowCoordinator.GetPrivateField<StandardLevelDetailViewController>("_levelDetailViewController");
                   _practiceViewController = _soloFlowCoordinator.GetPrivateField<PracticeViewController>("_practiceViewController");
                    if (_soloDetailView != null)
                        _soloDetailView.didPressPlayButtonEvent += _soloDetailView_didPressPlayButtonEvent;
                    else
                        Log("Detail View Null", Plugin.LogLevel.Info);
                    if (_practiceViewController != null)
                        _practiceViewController.didPressPlayButtonEvent += _practiceViewController_didPressPlayButtonEvent; 
                    else
                        Log("Practice View Null", Plugin.LogLevel.Info);

                }

                if (_partyFlowCoordinator == null)
                {
                    _partyFlowCoordinator = Resources.FindObjectsOfTypeAll<PartyFreePlayFlowCoordinator>().FirstOrDefault();
                }


            }
        }

        private void _practiceViewController_didPressPlayButtonEvent()
        {
            Log("Play Button Press ", Plugin.LogLevel.Info);
            party = _partyFlowCoordinator.isActivated;
            Log(party.ToString(), Plugin.LogLevel.Info);
        }

        private void _soloDetailView_didPressPlayButtonEvent(StandardLevelDetailViewController obj)
        {
            Log("Play Button Press " , Plugin.LogLevel.Info);
            party = _partyFlowCoordinator.isActivated;
            Log(party.ToString(), Plugin.LogLevel.Info);
        }



        private void _characteristicViewController_didSelectBeatmapCharacteristicEvent(BeatmapCharacteristicSelectionViewController arg1, BeatmapCharacteristicSO arg2)
        {
            _gameplayMode = arg2.characteristicName;
        }

        public void OnApplicationQuit()
        {
            Settings.Save();
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public static void Log(string input, Plugin.LogLevel logLvl)
        {
            if (logLvl >= LogLevel.Info || debug) Console.WriteLine("[! ! ! ! Beat Saber Tweaks ! ! ! !]: " + input);
        }
    }
}
