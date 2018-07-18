using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class RemoveBombsSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.RemoveBombs;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.RemoveBombs = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}