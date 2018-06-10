using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberTweaks
{
    class MoveEnergyBarSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            return Settings.MoveEnergyBar;
        }

        protected override void ApplyValue(bool value)
        {
            Settings.MoveEnergyBar = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}