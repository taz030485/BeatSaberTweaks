using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberTweaks
{
	public class NoteJumpSpeedSettingsController : ListSettingsController
	{
        protected float[] _volumes;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            float minValue = 5f;
            float increments = 1f;
            numberOfElements = 16;
            _volumes = new float[numberOfElements];
            for (int i = 0; i < _volumes.Length; i++)
            {
                _volumes[i] = minValue + increments * i;
            }
            float volume = Settings.NoteJumpSpeed;
            idx = numberOfElements - 1;
            for (int j = 0; j < _volumes.Length; j++)
            {
                if (volume == _volumes[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            Settings.NoteJumpSpeed = _volumes[idx];
        }

        protected override string TextForValue(int idx)
        {
            Settings.NoteJumpSpeed = _volumes[idx];
            return string.Format("{0:0}", _volumes[idx]);
        }
    }
}