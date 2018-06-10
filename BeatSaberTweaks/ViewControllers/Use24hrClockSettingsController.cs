using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class Use24hrClockSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.Use24hrClock;
        }

        protected override void ApplyValue(bool value)
        {
            
        }

        protected override string TextForValue(bool value)
        {
            Settings.Use24hrClock = value;
            return (!value) ? "OFF" : "ON";
        }
    }
}