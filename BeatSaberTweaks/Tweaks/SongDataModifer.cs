using System;
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

                MethodInfo original = typeof(BeatDataTransformHelper).GetMethod("CreateTransformedBeatmapData", BindingFlags.Public | BindingFlags.Static);
                MethodInfo modified = typeof(SongDataModifer).GetMethod(nameof(CreateTransformedBeatmapData), BindingFlags.Public | BindingFlags.Static);
                songDataRedirect = new Redirection(original, modified, true);
            }
            else
            {
                Destroy(this);
            }
        }

        static bool ShouldApplyModifers()
        {
            return TweakManager.IsPartyMode() && (Settings.RemoveBombs || Settings.OneColour || Settings.NoArrows);
        }

        public static BeatmapData CreateTransformedBeatmapData(BeatmapData beatmapData, GameplayOptions gameplayOptions, GameplayMode gameplayMode)
        {
            BeatmapData newData = (BeatmapData)songDataRedirect.InvokeOriginal(null, beatmapData, gameplayOptions, gameplayMode);

            if (ShouldApplyModifers())
            {
                Console.WriteLine("Applying BeatMap modifiers.");
                newData = ApplyModifiers(newData);
            }

            return newData;
        }

        public static BeatmapData ApplyModifiers(BeatmapData beatmapData)
        {
            BeatmapLineData[] beatmapLinesData = beatmapData.beatmapLinesData;
            int[] array = new int[beatmapLinesData.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
            int num = 0;
            for (int j = 0; j < beatmapLinesData.Length; j++)
            {
                num += beatmapLinesData[j].beatmapObjectsData.Length;
            }
            List<BeatmapObjectData> list = new List<BeatmapObjectData>(num);
            bool flag;
            do
            {
                flag = false;
                float num2 = 999999f;
                int num3 = 0;
                for (int k = 0; k < beatmapLinesData.Length; k++)
                {
                    BeatmapObjectData[] beatmapObjectsData = beatmapLinesData[k].beatmapObjectsData;
                    int num4 = array[k];
                    if (num4 < beatmapObjectsData.Length)
                    {
                        flag = true;
                        BeatmapObjectData beatmapObjectData = beatmapObjectsData[num4];
                        float time = beatmapObjectData.time;
                        if (time < num2)
                        {
                            num2 = time;
                            num3 = k;
                        }
                    }
                }
                if (flag)
                {
                    var note = beatmapLinesData[num3].beatmapObjectsData[array[num3]].GetCopy();
                    if (Settings.RemoveBombs && !IsBomb(note))
                    {
                        list.Add(beatmapLinesData[num3].beatmapObjectsData[array[num3]].GetCopy());
                    }
                    array[num3]++;
                }
            }
            while (flag);
            ModifyObjects(list, beatmapData.beatmapLinesData.Length);
            int[] array2 = new int[beatmapLinesData.Length];
            for (int l = 0; l < list.Count; l++)
            {
                BeatmapObjectData beatmapObjectData2 = list[l];
                array2[beatmapObjectData2.lineIndex]++;
            }
            BeatmapLineData[] array3 = new BeatmapLineData[beatmapLinesData.Length];
            for (int m = 0; m < beatmapLinesData.Length; m++)
            {
                array3[m] = new BeatmapLineData();
                array3[m].beatmapObjectsData = new BeatmapObjectData[array2[m]];
                array[m] = 0;
            }
            for (int n = 0; n < list.Count; n++)
            {
                BeatmapObjectData beatmapObjectData3 = list[n];
                int lineIndex = beatmapObjectData3.lineIndex;
                array3[lineIndex].beatmapObjectsData[array[lineIndex]] = beatmapObjectData3;
                array[lineIndex]++;
            }
            BeatmapEventData[] array4 = new BeatmapEventData[beatmapData.beatmapEventData.Length];
            for (int num5 = 0; num5 < beatmapData.beatmapEventData.Length; num5++)
            {
                BeatmapEventData beatmapEventData = beatmapData.beatmapEventData[num5];
                array4[num5] = beatmapEventData.GetCopy();
            }
            return new BeatmapData(array3, array4);
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
                beatmapObjectData.MirrorLineIndex(beatmapLineCount);
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
