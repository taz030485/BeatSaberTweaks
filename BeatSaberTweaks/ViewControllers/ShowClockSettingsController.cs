using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Settings.ShowClock = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}