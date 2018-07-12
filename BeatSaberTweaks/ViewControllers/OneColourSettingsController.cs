using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class OneColourSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.OneColour;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.OneColour = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}