using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberTweaks
{
    class MoveScoreSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.MoveScore;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.MoveScore = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}