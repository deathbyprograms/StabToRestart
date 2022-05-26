using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using StabToRestart.Configuration;
using StabToRestart.Utilities;
using TriggerCondition = StabToRestart.Configuration.StabToRestartConfig.TriggerCondition;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Linq;
using System.Collections.Generic;


namespace StabToRestart.UI.ViewControllers
{
    [HotReload(RelativePathToLayout = @"..\Views\StabToRestartConfigView.bsml")]
    [ViewDefinition("StabToRestart.UI.Views.StabToRestartConfigView.bsml")]
    internal class StabToRestartConfigViewController : BSMLAutomaticViewController
    {

        [UIValue("enabled")]
        private bool _enabled
        {
            get => StabToRestartConfig.Instance.IsEnabled;
            set
            {
                _settingsContainer.SetActive(value);
                StabToRestartConfig.Instance.IsEnabled = value;
            }
        }

        [UIObject("settings")]
        private GameObject _settingsContainer = null;

        [UIValue("trigger-list")]
        private readonly List<object> _triggerOptions = Enum.GetValues(typeof(TriggerCondition)).Cast<object>().ToList<object>();

        [UIValue("current-trigger")]
        private TriggerCondition _currentTrigger
        {
            get => StabToRestartConfig.Instance.Mode;
            set {
                _buttonModeChoice.SetActive(value == TriggerCondition.Button || value == TriggerCondition.Both);
                StabToRestartConfig.Instance.Mode = value;
            }
        }

        [UIAction("condition-to-string")]
        private string GetStringFromTriggerCondition(TriggerCondition condition)
        {
            switch (condition)
            {
                case TriggerCondition.Instant:
                    return "No Condition";
                case TriggerCondition.Button:
                    return "Holding Button";
                case TriggerCondition.Hold:
                    return "Keep Saber in Body";
                case TriggerCondition.Both:
                    return "Either";
                default:
                    return "You broke it. Good job";
            }
        }

        [UIObject("button-mode-choice")]
        private GameObject _buttonModeChoice = null;

        [UIValue("current-button")]
        private InputFeatureUsage<bool> _currButton
        {
            get => StabToRestartConfig.Instance.SelectedButton;
            set => StabToRestartConfig.Instance.SelectedButton = value;
        }

        [UIValue("button-list")]
        private readonly List<object> _buttons = new object[] {
            CommonUsages.primaryButton,
            CommonUsages.secondaryButton,
            CommonUsages.gripButton,
            CommonUsages.triggerButton,
            CommonUsages.primary2DAxisClick
        }.ToList<object>();

        private readonly Dictionary<InputFeatureUsage<bool>, string> _oculusMappings = new Dictionary<InputFeatureUsage<bool>, string>() {
            { CommonUsages.primaryButton, "[X/A]"},
            { CommonUsages.secondaryButton, "[Y/B]"},
            { CommonUsages.gripButton, "Grip"},
            { CommonUsages.triggerButton, "Trigger"},
            { CommonUsages.primary2DAxisClick, "Thumbstick"}
        };

        private readonly Dictionary<InputFeatureUsage<bool>, string> _oculusOpenVRMappings = new Dictionary<InputFeatureUsage<bool>, string>() {
            { CommonUsages.primaryButton, "[Y/B]"},
            { CommonUsages.secondaryButton, "[X/A]"},
            { CommonUsages.gripButton, "Grip"},
            { CommonUsages.triggerButton, "Trigger"},
            { CommonUsages.primary2DAxisClick, "Joystick"}
        };

        private readonly Dictionary<InputFeatureUsage<bool>, string> _openVRMappings = new Dictionary<InputFeatureUsage<bool>, string>() {
            { CommonUsages.primaryButton, "Primary"},
            { CommonUsages.secondaryButton, "Alternate"},
            { CommonUsages.gripButton, "Grip"},
            { CommonUsages.triggerButton, "Trigger"},
            { CommonUsages.primary2DAxisClick, "Trackpad/Joystick"}
        };

        [UIAction("button-to-string")]
        private string ButtonToString(InputFeatureUsage<bool> button)
        {
            InputDevice? leftController, rightController;
            if (!ControllerGetter.GetControllers(out leftController, out rightController))
                return "No controllers found";
            if(!(leftController.Value.TryGetFeatureValue(button, out _)))
            {
                if (!(rightController.Value.TryGetFeatureValue(button, out _)))
                {
                    return "Button not supported";
                }
            }
            if(leftController.Value.manufacturer.Contains("Oculus"))
            {
                if(leftController.Value.TryGetFeatureValue(CommonUsages.menuButton, out _))
                {
                    if (_oculusMappings.ContainsKey(button))
                        return _oculusMappings[button];
                    return "Error";
                }
                else
                {
                    if (_oculusOpenVRMappings.ContainsKey(button))
                        return _oculusOpenVRMappings[button];
                    return "Error";
                }
            }
            else
            {
                if (_openVRMappings.ContainsKey(button))
                    return _openVRMappings[button];
                return "Error";
            }
        }

        [UIValue("min-stab-time")]
        private float _minStabTime
        {
            get => StabToRestartConfig.Instance.MinStabTime;
            set => StabToRestartConfig.Instance.MinStabTime = value;
        }

        [UIValue("max-horizontal")]
        private float _maxHorizontal
        {
            get => StabToRestartConfig.Instance.MaxHorizontal;
            set => StabToRestartConfig.Instance.MaxHorizontal = value;
        }

        [UIValue("max-vertical")]
        private float _maxVertical
        {
            get => StabToRestartConfig.Instance.MaxVertical;
            set => StabToRestartConfig.Instance.MaxVertical = value;
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            _buttonModeChoice.SetActive(StabToRestartConfig.Instance.Mode == TriggerCondition.Button || StabToRestartConfig.Instance.Mode == TriggerCondition.Both);
            _settingsContainer.SetActive(StabToRestartConfig.Instance.IsEnabled);
        }
}
}
