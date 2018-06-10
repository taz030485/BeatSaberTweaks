using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;

namespace BeatSaberTweaks
{
    public class TweakSettingsViewController : VRUIViewController
    {
        protected bool _firstTimeActivated = true;

        public VRUIViewController _leftSettings;
        public VRUIViewController _rightSettings;
        public List<SimpleSettingsController> tweakedSettingsControllers = new List<SimpleSettingsController>();

        //public event Action<TweakSettingsViewController, SettingsViewController.FinishAction> tweakSettingsViewControllerDidFinishEvent;

        protected override void DidActivate()
        {
            base.DidActivate();
            if (_firstTimeActivated)
            {
                _firstTimeActivated = false;
                Init();
            }
            //VRUIScreen leftScreen = screen.screenSystem.leftScreen;
            //VRUIScreen rightScreen = screen.screenSystem.rightScreen;
            //leftScreen.SetRootViewController(_leftSettings);
            //rightScreen.SetRootViewController(_rightSettings);
        }

        public virtual void Init()
        {
            foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
            {
                simpleSettingsController.Init();
            }
        }

        public virtual void ApplySettings()
        {
            foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
            {
                simpleSettingsController.ApplySettings();
            }
            Settings.Save();
        }

        //public virtual void OkButtonPressed()
        //{
        //    ApplySettings();
        //    if (settingsViewControllerDidFinishEvent != null)
        //    {
        //        settingsViewControllerDidFinishEvent(this, SettingsViewController.FinishAction.Ok);
        //    }
        //}

        //public virtual void ApplyButtonPressed()
        //{
        //    ApplySettings();
        //    if (settingsViewControllerDidFinishEvent != null)
        //    {
        //        settingsViewControllerDidFinishEvent(this, SettingsViewController.FinishAction.Apply);
        //    }
        //}

        //public virtual void CancelButtonPressed()
        //{
        //    if (settingsViewControllerDidFinishEvent != null)
        //    {
        //        settingsViewControllerDidFinishEvent(this, SettingsViewController.FinishAction.Cancel);
        //    }
        //}
    }
}
