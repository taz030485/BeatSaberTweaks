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
using System.Threading;
using System.Reflection;

namespace BeatSaberTweaks
{
    public class SongDataModifer : MonoBehaviour
    {
        public static SongDataModifer Instance;
        private static Redirection songDataRedirect;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Song Data Modifier").AddComponent<SongDataModifer>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                /*
                 * TODO
                 * Figure out how any of this works
                MethodInfo original = typeof(BeatDataTransformHelper).GetMethod("CreateTransformedBeatmapData", BindingFlags.Public | BindingFlags.Static);
                MethodInfo modified = typeof(SongDataModifer).GetMethod(nameof(CreateTransformedBeatmapData), BindingFlags.Public | BindingFlags.Static);
                songDataRedirect = new Redirection(original, modified, true);
                */

                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
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
                if (SceneUtils.isGameScene(scene))
                {
                    if (TweakManager.IsPartyMode() && (Settings.OverrideJumpSpeed || Settings.OneColour || Settings.NoArrows || Settings.RemoveBombs))
                    {
                        Plugin.Log("Party Mode Active", Plugin.LogLevel.Info);
                        StartCoroutine(WaitForLoad());
                    }
                    else
                        Plugin.Log("Party Mode Not Active", Plugin.LogLevel.Info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks (SongDataModifer) done fucked up: " + e);
            }
        }

        private IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                var resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();

                if (resultsViewController == null)
                {
                    Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Found resultsViewController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            LoadingDidFinishEvent();
        }

        private void LoadingDidFinishEvent()
        {
            StartCoroutine(SetNJS());
            StartCoroutine(CreateTransformedBeatmapData(Plugin._gameplayMode));
        }

        IEnumerator SetNJS()
        {
   
            var _levelData = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();
            var _currentLevelPlaying = _levelData.difficultyBeatmap;

            var ONJS = _currentLevelPlaying.GetPrivateField<float>("_noteJumpMovementSpeed");

            if (_currentLevelPlaying.noteJumpMovementSpeed < 0)
            {
                _currentLevelPlaying.SetPrivateField("_noteJumpMovementSpeed", -Settings.NoteJumpSpeed);
            }
            else
            {
                _currentLevelPlaying.SetPrivateField("_noteJumpMovementSpeed", Settings.NoteJumpSpeed);
            }
            
            yield return new WaitForSeconds(0.5f);
           
            _currentLevelPlaying.SetPrivateField("_noteJumpMovementSpeed", ONJS);
        }

        static bool ShouldApplyModifers()
        {
            return TweakManager.IsPartyMode() && (Settings.RemoveBombs || Settings.OneColour || Settings.NoArrows);
        }

        
        
        public static IEnumerator CreateTransformedBeatmapData(string gameplayMode)
        {
            Plugin.Log("Waiting to change beatmap", Plugin.LogLevel.Info);
            yield return new WaitForSeconds(0.2f);
            if (ShouldApplyModifers())
            {
                Plugin.Log("Attempting to change beatmap", Plugin.LogLevel.Info);
                //Console.WriteLine("Applying BeatMap modifiers.");
                ApplyModifiers();
            }


        }
        

        public static void ApplyModifiers()
        {
            Plugin.Log("Modifying BeatMap Data", Plugin.LogLevel.Info);
            GameplayCoreSceneSetup gameplayCoreSceneSetup = Resources.FindObjectsOfTypeAll<GameplayCoreSceneSetup>().First();
            BeatmapDataModel dataModel = gameplayCoreSceneSetup.GetPrivateField<BeatmapDataModel>("_beatmapDataModel");
            BeatmapData beatmapData = dataModel.beatmapData;
            BeatmapObjectData[] objects;
            NoteData noteData;
            foreach (BeatmapLineData line in beatmapData.beatmapLinesData)
            {
                objects = line.beatmapObjectsData;
                foreach (BeatmapObjectData beatmapObject in objects)
                {
                    if (beatmapObject.beatmapObjectType == BeatmapObjectType.Note)
                    {
                        noteData = beatmapObject as NoteData;

                        if (noteData != null)
                        {
                            if (Settings.NoArrows)
                            {
                                Plugin.Log("Changing Note", Plugin.LogLevel.Info);
                                noteData.SetNoteToAnyCutDirection();
                            }

                            if (Settings.OneColour && noteData.noteType == NoteType.NoteA)
                            {
                                noteData.SwitchNoteType();
                            }
                            if(noteData.noteType == NoteType.Bomb && Settings.RemoveBombs)
                            {
                                //Admittedly ghetto way of removing bombs but should be amusing at the very least
                                noteData.MirrorLineIndex(10);
                            }
                        }
                    }
                    




                }
            }
            //return new SongData(songData.BeatsPerMinute, Settings.OverrideJumpSpeed ? Settings.NoteJumpSpeed : songData.NoteJumpSpeed, array3, array4);
        }

        private static bool IsBomb(BeatmapObjectData beatmapObjectData)
        {
            if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note)
            {
                NoteData obstacleData = beatmapObjectData as NoteData;
                return obstacleData.noteType == NoteType.Bomb;
            }
            return false;
        }

        private static void ModifyObjects(List<BeatmapObjectData> beatmapObjects, int beatmapLineCount)
        {
            for (int i = 0; i < beatmapObjects.Count; i++)
            {
                BeatmapObjectData beatmapObjectData = beatmapObjects[i];
                if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note)
                {
                    NoteData noteData = beatmapObjectData as NoteData;
                    if (noteData != null)
                    {
                        if (Settings.NoArrows)
                        {
                            noteData.SetNoteToAnyCutDirection();
                        }

                        if (Settings.OneColour && noteData.noteType == NoteType.NoteA)
                        {
                            noteData.SwitchNoteType();
                        }
                    }
                }
            }
        }
    }
}
