using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberTweaks
{
    [Serializable]
    public class Settings
    {
        static Settings instance = null;

        // Note Hit/Miss and Menu BG Volume
        [SerializeField]
        float noteHitVolume = 1.0f;
        public static float NoteHitVolume { get => instance.noteHitVolume; set => instance.noteHitVolume = value; }

        [SerializeField]
        float noteMissVolume = 1.0f;
        public static float NoteMissVolume { get => instance.noteMissVolume; set => instance.noteMissVolume = value; }

        [SerializeField]
        float menuBGVolume = 1.0f;
        public static float MenuBGVolume { get => instance.menuBGVolume; set => instance.menuBGVolume = value; }

        // In Game Clock
        [SerializeField]
        bool showclock = false;
        public static bool ShowClock { get => instance.showclock; set => instance.showclock = value; }

        [SerializeField]
        bool use24hrClock = false;
        public static bool Use24hrClock { get => instance.use24hrClock; set => instance.use24hrClock = value; }

        [SerializeField]
        float clockFontSize = 4.0f;
        public static float ClockFontSize { get => instance.clockFontSize; set => instance.clockFontSize = value; }

        [SerializeField]
        Vector3 clockPosition = new Vector3(0, 2.4f, 2.4f);
        public static Vector3 ClockPosition { get => instance.clockPosition; set => instance.clockPosition = value; }

        [SerializeField]
        Vector3 clockRotation = new Vector3(0, 0, 0);
        public static Quaternion ClockRotation { get => Quaternion.Euler(instance.clockRotation); set => instance.clockPosition = value.eulerAngles; }

        // Move Energy Bar
        [SerializeField]
        bool moveEnergyBar = false;
        public static bool MoveEnergyBar { get => instance.moveEnergyBar; set => instance.moveEnergyBar = value; }

        [SerializeField]
        float energyBarHeight = 3.0f;
        public static float EnergyBarHeight { get => instance.energyBarHeight; set => instance.energyBarHeight = value; }

        // Move Score
        [SerializeField]
        bool moveScore = false;
        public static bool MoveScore { get => instance.moveScore; set => instance.moveScore = value; }

        bool removeHighWalls = false;
        public static bool RemoveHighWalls { get => instance.removeHighWalls; set => instance.removeHighWalls = value; }

        bool noArrows = false;
        public static bool NoArrows { get => instance.noArrows; set => instance.noArrows = value; }

        bool oneColour = false;
        public static bool OneColour { get => instance.oneColour; set => instance.oneColour = value; }

        bool removeBombs = false;
        public static bool RemoveBombs { get => instance.removeBombs; set => instance.removeBombs = value; }

        public Settings()
        {
            
        }

        public static string SettingsPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "Tweaks.cfg");
        }

        public static void Load()
        {
            // I'm a dumbass and spelled the filename wrong
            string oldPath = Path.Combine(Environment.CurrentDirectory, "Tweask.cfg");
            if (File.Exists(oldPath))
            {
                string dataAsJson = File.ReadAllText(oldPath);
                instance = JsonUtility.FromJson<Settings>(dataAsJson);
                File.Delete(oldPath);
                return;
            }

            string filePath = SettingsPath();
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                instance = JsonUtility.FromJson<Settings>(dataAsJson);
            }
            else
            {
                instance = new Settings();
            }
        }

        public static void Save()
        {
            string dataAsJson = JsonUtility.ToJson(instance, true);
            File.WriteAllText(SettingsPath(), dataAsJson);
        }
    }
}
