using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRUI;
using UnityEngine.UI;
using TMPro;

namespace BeatSaberTweaks
{
    public class TweakSettingsViewController : VRUIViewController
    {
        //protected bool _firstTimeActivated = true;

        public VRUIViewController _leftSettings;
        public VRUIViewController _rightSettings;
        public List<SimpleSettingsController> tweakedSettingsControllers = new List<SimpleSettingsController>();

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                SetupButtons();
                foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
                {
                    // NOTE: The problem seems to be in the internal Init function.
                    // As "Activating" logs, but not "Activated"
                    Console.WriteLine("Activating: " + simpleSettingsController.name);
                    //simpleSettingsController.gameObject.SetActive(true);
                    simpleSettingsController.Init();
                    Console.WriteLine("Activated: " + simpleSettingsController.name);
                }
            }
            //VRUIScreen leftScreen = screen.screenSystem.leftScreen;
            //VRUIScreen rightScreen = screen.screenSystem.rightScreen;
            //leftScreen.SetRootViewController(_leftSettings);
            //rightScreen.SetRootViewController(_rightSettings);
        }

        void SetupButtons()
        {
            DestroyImmediate(transform.Find("OkButton").gameObject);
            DestroyImmediate(transform.Find("ApplyButton").gameObject);
            Button cancelButton = transform.Find("CancelButton").GetComponent<Button>();
            DestroyImmediate(cancelButton.GetComponent<GameEventOnUIButtonClick>());
            cancelButton.onClick = new Button.ButtonClickedEvent();
            cancelButton.onClick.AddListener(CloseButtonPressed);
            cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        }

        protected override void LeftAndRightScreenViewControllers(out VRUIViewController leftScreenViewController, out VRUIViewController rightScreenViewController)
        {
            leftScreenViewController = _leftSettings;
            rightScreenViewController = _rightSettings;
        }

        public event Action<TweakSettingsViewController, FinishAction> didFinishEvent;
        HierarchyRebuildData _hierarchyRebuildData;

        public enum FinishAction
        {
            Ok,
            Cancel,
            Apply
        }

        private class HierarchyRebuildData
        {
            public HierarchyRebuildData(FinishAction finishAction)
            {
                this.finishAction = finishAction;
            }

            public FinishAction finishAction;
        }

        protected override void RebuildHierarchy(object hierarchyRebuildData)
        {
            HierarchyRebuildData hierarchyRebuildData2 = hierarchyRebuildData as HierarchyRebuildData;
            if (hierarchyRebuildData2 != null)
            {
                this.HandleFinishButton(hierarchyRebuildData2.finishAction);
            }
        }

        protected override object GetHierarchyRebuildData()
        {
            return this._hierarchyRebuildData;
        }

        public virtual void HandleFinishButton(FinishAction finishAction)
        {
            this._hierarchyRebuildData = new HierarchyRebuildData(finishAction);
            if (this.didFinishEvent != null)
            {
                this.didFinishEvent(this, finishAction);
            }
        }

        void ApplySettings()
        {
            foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
            {
                simpleSettingsController.ApplySettings();
            }
            Settings.Save();
        }

        public virtual void CloseButtonPressed()
        {
            ApplySettings();
            DismissModalViewController(null, false);
        }
    }
}
