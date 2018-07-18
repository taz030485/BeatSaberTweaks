using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberTweaks
{
	public class SongSpeedSettingsController : ListSettingsController
	{
        protected float[] speeds;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            float minValue = 0.25f;
            float increments = 0.05f;
            numberOfElements = 56;
            speeds = new float[numberOfElements];
            for (int i = 0; i < speeds.Length; i++)
            {
                speeds[i] = minValue + increments * i;
            }
            float volume = SongSpeed.TimeScale;
            idx = numberOfElements - 1;
            for (int j = 0; j < speeds.Length; j++)
            {
                if (volume == speeds[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            SongSpeed.TimeScale = speeds[idx];
        }

        protected override string TextForValue(int idx)
        {
            SongSpeed.TimeScale = speeds[idx];
            return string.Format("{0:0}%", speeds[idx]*100);
        }
    }
}