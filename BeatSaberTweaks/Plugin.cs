using System;
using System.Collections.Generic;
using System.Linq;
using IllusionPlugin;

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

        private static bool debug = true;

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
