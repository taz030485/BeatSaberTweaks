using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class RemoveHighWallsSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.RemoveHighWalls;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.RemoveHighWalls = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}