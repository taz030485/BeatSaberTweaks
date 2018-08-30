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
        public string Version => "3.4";
#else
        public string Version => "3.3.2";
#endif

        private bool _init = false;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            Settings.Load();
            SettingsUI.OnLoad();
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
    }
}
