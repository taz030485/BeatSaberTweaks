using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class ShowClockSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.ShowClock;
        }

        protected override void ApplyValue(bool value)
        {
            
        }

        protected override string TextForValue(bool value)
        {
            Settings.ShowClock = value;
            return (!value) ? "OFF" : "ON";
        }
    }
}