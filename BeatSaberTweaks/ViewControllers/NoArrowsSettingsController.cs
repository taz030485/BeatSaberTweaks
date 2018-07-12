using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class NoArrowsSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.NoArrows;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.NoArrows = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}