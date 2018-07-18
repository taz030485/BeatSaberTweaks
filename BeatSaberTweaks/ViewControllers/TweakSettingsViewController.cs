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
        public VRUIViewController _leftSettings;
        public VRUIViewController _rightSettings;
        List<SimpleSettingsController> tweakedSettingsControllers;
        
        public void AddController(SimpleSettingsController controller)
        {
            if (tweakedSettingsControllers == null)
            {
                tweakedSettingsControllers = new List<SimpleSettingsController>();
            }
            tweakedSettingsControllers.Add(controller);
        }

#if true    
        // Old version
        private bool _firstTimeActivated = true;
        protected override void DidActivate()
        {
            base.DidActivate();
            if (_firstTimeActivated)
            {
                _firstTimeActivated = false;
                SetupButtons();
                foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
                {
                    simpleSettingsController.Init();
                }
            }
            VRUIScreen leftScreen = screen.screenSystem.leftScreen;
            VRUIScreen rightScreen = screen.screenSystem.rightScreen;
            leftScreen.SetRootViewController(_leftSettings);
            rightScreen.SetRootViewController(_rightSettings);
        }
#else
        // New version
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                SetupButtons();
                foreach (SimpleSettingsController simpleSettingsController in tweakedSettingsControllers)
                {
                    simpleSettingsController.Init();
                }
            }
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
                HandleFinishButton(hierarchyRebuildData2.finishAction);
            }
        }

        protected override object GetHierarchyRebuildData()
        {
            return _hierarchyRebuildData;
        }

        public virtual void HandleFinishButton(FinishAction finishAction)
        {
            _hierarchyRebuildData = new HierarchyRebuildData(finishAction);
            if (didFinishEvent != null)
            {
                didFinishEvent(this, finishAction);
            }
        }
#endif

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
