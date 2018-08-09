using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using Hidden_Notes;

namespace BeatSaberTweaks
{
    class HiddenNotesSettingsController : SwitchSettingsController
    {
        protected override bool GetInitValue()
        {
            //return Hidden_Notes.Plugin.Config.Enabled;
            return false;
        }

        protected override void ApplyValue(bool value)
        {
            //Hidden_Notes.Plugin.Config.Enabled = value;
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}