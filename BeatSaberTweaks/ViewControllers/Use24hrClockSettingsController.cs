using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Settings.Use24hrClock = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}