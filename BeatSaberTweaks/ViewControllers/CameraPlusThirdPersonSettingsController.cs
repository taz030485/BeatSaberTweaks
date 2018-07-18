using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CameraPlus;

namespace BeatSaberTweaks
{
    class CameraPlusThirdPersonSettingsController : SwitchSettingsController
    {
        GameObject cam = null;

        protected override bool GetInitValue()
        {
            if (cam == null)
            {
                var thing = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>().First();
                cam = ReflectionUtil.GetPrivateField<Transform>(thing, "_cameraCube").gameObject;
            }
            return CameraPlusBehaviour.ThirdPerson;
        }

        protected override void ApplyValue(bool value)
        {
            CameraPlusBehaviour.ThirdPerson = value;
            if (value)
            {
                CameraPlus.Plugin.Ini.WriteValue("thirdPerson", "true");
            }
            else
            {
                CameraPlus.Plugin.Ini.WriteValue("thirdPerson", "false");
            }
            if (cam)
            {
                cam.SetActive(value);
            }
        }

        protected override string TextForValue(bool value)
        {
            return (!value) ? "OFF" : "ON";
        }
    }
}