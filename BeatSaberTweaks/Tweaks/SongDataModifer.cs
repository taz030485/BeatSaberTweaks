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

                MethodInfo original = typeof(GameSongController).GetMethod("CreateTransformedSongData", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo modified = typeof(SongDataModifer).GetMethod(nameof(CreateTransformedSongData), BindingFlags.Public | BindingFlags.Instance);
                songDataRedirect = new Redirection(original, modified, true);
            }
            else
            {
                Destroy(this);
            }
        }

        public virtual SongData CreateTransformedSongData(SongData songData, GameplayOptions gameplayOptions, GameplayMode gameplayMode)
        {
            SongData songData2 = songData;

            if (gameplayMode == GameplayMode.PartyStandard)
            {
                Console.WriteLine("Modified");
                songData2 = ApplyModifiers(songData2);
            }
            else
            {
                Console.WriteLine("Original");
                if (gameplayMode == GameplayMode.SoloNoArrows)
                {
                    songData2 = SongDataNoArrowsTransform.CreateTransformedData(songData2);
                }   
            }

            if (gameplayOptions.mirror)
            {
                songData2 = SongDataMirrorTransform.CreateTransformedData(songData2);
            }

            if (songData2 == songData)
            {
                songData2 = songData.GetCopy();
            }
            return songData2;

            /*

            if (gameplayMode == GameplayMode.PartyStandard)
            {
                Console.WriteLine("Modified");
                SongData songData2 = songData;
                if (gameplayOptions.mirror)
                {
                    songData2 = SongDataMirrorTransform.CreateTransformedData(songData2);
                }
                if (songData2 == songData)
                {
                    songData2 = songData.GetCopy();
                }
                return songData2;
            }
            else
            {
                Console.WriteLine("Original");
                SongData newData = (SongData)songDataRedirect.InvokeOriginal(null, songData, gameplayOptions, gameplayMode);
                Console.WriteLine(newData.BeatsPerMinute);
                return newData;
            }
            */
        }

        public static SongData ApplyModifiers(SongData songData)
        {
            SongLineData[] songLinesData = songData.SongLinesData;
            int[] array = new int[songLinesData.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
            int num = 0;
            for (int j = 0; j < songLinesData.Length; j++)
            {
                num += songLinesData[j].songObjectsData.Length;
            }
            List<SongObjectData> list = new List<SongObjectData>(num);
            bool flag;
            do
            {
                flag = false;
                float num2 = 999999f;
                int num3 = 0;
                for (int k = 0; k < songLinesData.Length; k++)
                {
                    SongObjectData[] songObjectsData = songLinesData[k].songObjectsData;
                    int num4 = array[k];
                    if (num4 < songObjectsData.Length)
                    {
                        flag = true;
                        SongObjectData songObjectData = songObjectsData[num4];
                        float time = songObjectData.time;
                        if (time < num2)
                        {
                            num2 = time;
                            num3 = k;
                        }
                    }
                }
                if (flag)
                {
                    list.Add(songLinesData[num3].songObjectsData[array[num3]].GetCopy());
                    array[num3]++;
                }
            }
            while (flag);

            ModifyObjects(list);
            list.RemoveAll(item => item == null);

            int[] array2 = new int[songLinesData.Length];
            for (int l = 0; l < list.Count; l++)
            {
                SongObjectData songObjectData2 = list[l];
                array2[songObjectData2.lineIndex]++;
            }
            SongLineData[] array3 = new SongLineData[songLinesData.Length];
            for (int m = 0; m < songLinesData.Length; m++)
            {
                array3[m] = new SongLineData();
                array3[m].songObjectsData = new SongObjectData[array2[m]];
                array[m] = 0;
            }
            for (int n = 0; n < list.Count; n++)
            {
                SongObjectData songObjectData3 = list[n];
                int lineIndex = songObjectData3.lineIndex;
                array3[lineIndex].songObjectsData[array[lineIndex]] = songObjectData3;
                array[lineIndex]++;
            }


            SongEventData[] array4 = new SongEventData[songData.SongEventData.Length];

            if (songData.SongEventData.Length == 0)
            {
                Console.WriteLine("No event data");
            }
            else
            {
                for (int num5 = 0; num5 < songData.SongEventData.Length; num5++)
                {
                    SongEventData songEventData = songData.SongEventData[num5];
                    array4[num5] = songEventData.GetCopy();
                }
            }
            return new SongData(songData.BeatsPerMinute, songData.NoteJumpSpeed, array3, array4);
        }

        private static void ModifyObjects(List<SongObjectData> songObjects)
        {
            for (int i = 0; i < songObjects.Count; i++)
            {
                SongObjectData songObjectData = songObjects[i];
                if (songObjectData.songObjectType == SongObjectData.SongObjectTypeEnum.Note)
                {
                    NoteData noteData = songObjectData as NoteData;
                    if (noteData != null)
                    {
                        if (Settings.NoArrows)
                        {
                            noteData.SetNoteToAnyCutDirection();
                        }

                        if (Settings.OneColour && noteData.noteType == NoteData.NoteType.NoteA)
                        {
                            noteData.SwitchNoteType();
                        }

                        if (Settings.RemoveBombs && noteData.noteType == NoteData.NoteType.Bomb)
                        {
                            songObjects[i] = null;
                        }
                    }
                }
                
                if (songObjectData.songObjectType == SongObjectData.SongObjectTypeEnum.Obstacle)
                {
                    ObstacleData obstacleData = songObjectData as ObstacleData;
                    if (obstacleData != null)
                    {
                        if (obstacleData.obstacleType == ObstacleData.ObstacleType.Top)
                        {
                        if (Settings.RemoveHighWalls)
                        {
                            songObjects[i] = null;
                        }
                        }
                    }
                }
            }
        }
    }
}
