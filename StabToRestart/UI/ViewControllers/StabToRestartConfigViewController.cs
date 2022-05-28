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
        private string _currButton
        {
            get => StabToRestartConfig.Instance.SelectedButton;
            set => StabToRestartConfig.Instance.SelectedButton = value;
        }

        [UIValue("button-list")]
        private readonly List<object> _buttons = new object[] {
            "PrimaryButton",
            "SecondaryButton",
            "Grip",
            "TriggerButton",
            "Primary2DAxisClick"
        }.ToList<object>();

        private readonly Dictionary<string, string> _oculusMappings = new Dictionary<string, string>() {
            {"PrimaryButton", "[X/A]"},
            {"SecondaryButton", "[Y/B]"},
            {"Grip", "Grip"},
            {"TriggerButton", "Trigger"},
            {"Primary2DAxisClick", "Thumbstick"}
        };

        private readonly Dictionary<string, string> _oculusOpenVRMappings = new Dictionary<string, string>() {
            {"PrimaryButton", "[Y/B]"},
            {"SecondaryButton", "[X/A]"},
            {"Grip", "Grip"},
            {"TriggerButton", "Trigger"},
            {"Primary2DAxisClick", "Joystick"}
        };

        private readonly Dictionary<string, string> _openVRMappings = new Dictionary<string, string>() {
            {"PrimaryButton", "Primary"},
            {"SecondaryButton", "Alternate"},
            {"Grip", "Grip"},
            {"TriggerButton", "Trigger"},
            {"Primary2DAxisClick", "Trackpad/Joystick"}
        };

        static bool InputSupported(InputDevice device, string input) {
            List<InputFeatureUsage> usages = new List<InputFeatureUsage>();
            return device.TryGetFeatureUsages(usages) && usages.Select(u => u.name).Contains(input);
        }

        [UIAction("button-to-string")]
        private string ButtonToString(string button)
        {
            InputDevice? leftController, rightController;
            if (!ControllerGetter.GetControllers(out leftController, out rightController))
                return "No controllers found";
            if (!(InputSupported(leftController.Value, button) || InputSupported(rightController.Value, button)))
                return "Button not supported";
            if (leftController.Value.manufacturer.Contains("Oculus"))
            {
                if (InputSupported(leftController.Value, "MenuButton")) {
                    if (_oculusMappings.TryGetValue(button, out string oculusName))
                        return oculusName;
                }
                else if (_oculusOpenVRMappings.TryGetValue(button, out string oculusOpenVRName))
                {
                    return oculusOpenVRName;
                }
            }
            else if (_openVRMappings.TryGetValue(button, out string openVRName))
            {
                return openVRName;
            }
            return "Error";
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

        [UIValue("min-speed")]
        private float _minSpeed
        {
            get => StabToRestartConfig.Instance.MinSpeed;
            set => StabToRestartConfig.Instance.MinSpeed = value;
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            _buttonModeChoice.SetActive(StabToRestartConfig.Instance.Mode == TriggerCondition.Button || StabToRestartConfig.Instance.Mode == TriggerCondition.Both);
            _settingsContainer.SetActive(StabToRestartConfig.Instance.IsEnabled);
        }
}
}
