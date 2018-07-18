using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class OverrideJumpSpeedSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.OverrideJumpSpeed;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.OverrideJumpSpeed = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}