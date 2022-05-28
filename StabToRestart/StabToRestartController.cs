using UnityEngine;
using UnityEngine.XR;
using StabToRestart.Utilities;
using StabToRestart.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using Zenject;

namespace StabToRestart
{
    public class StabToRestartController : MonoBehaviour
    {
        private readonly float MAX_HORIZONTAL_ANGLE = StabToRestartConfig.Instance.MaxHorizontal;
        private readonly float MAX_VERTICAL_ANGLE = StabToRestartConfig.Instance.MaxVertical;
        private readonly float MIN_SPEED = StabToRestartConfig.Instance.MinSpeed;

        private float _playerHeight;
        private bool _automaticHeight;

        [Inject]
        private readonly SaberManager _saberManager = null;

        [InjectOptional]
        private readonly PlayerHeightDetector _phd = null;

        [Inject]
        private readonly MainCamera _mainCamera = null;

        public CapsuleCollider BodyCollider { get; private set; }

        [Inject]
        private readonly ILevelRestartController _restartController = null;

        [Inject]
        private readonly GameplayCoreSceneSetupData _sceneSetupData = null;

        private InputDevice? _leftController;

        private InputDevice? _rightController;

        private readonly float[] _holdTimes = new float[2];

        #region Monobehaviour Messages

        private void Start()
        {
            ControllerGetter.GetControllers(out _leftController, out _rightController);

            initPlayerHeight();

            BodyCollider = gameObject.AddComponent<CapsuleCollider>();
            BodyCollider.transform.localScale = Vector3.one + (Vector3.up * ((_playerHeight / 2) - 1)) - (Vector3.forward * 0.25f);

            _saberManager.didUpdateSaberPositionsEvent += HandleSaberMovement;
        }

        private void restart()
        {
            _restartController.RestartLevel();
        }

        static bool IsInputActive(InputDevice device, string input) {
            List<InputFeatureUsage> usages = new List<InputFeatureUsage>();
            if (!device.TryGetFeatureUsages(usages))
                return false;
            InputFeatureUsage usage = usages.FirstOrDefault(u => u.name == input);
            if (usage.type == typeof(float))
                return device.TryGetFeatureValue(usage.As<float>(), out float value) && value > .8;
            if (usage.type == typeof(bool))
                return device.TryGetFeatureValue(usage.As<bool>(), out bool value) && value;
            return false;
        }

        public void HandleSaberMovement(Saber leftSaber, Saber rightSaber)
        {
            int i = 0;
            foreach((InputDevice? controller, Saber saber) in new[] {(_leftController, leftSaber), (_rightController, rightSaber)}) {
                Vector3 saberVector = saber.saberBladeTopPos - saber.saberBladeBottomPos;
                float saberLength = saberVector.magnitude;
                if (BodyCollider.Raycast(new Ray(saber.saberBladeBottomPos, saberVector), out _, saberLength * 0.75f) && saberCollisionOkay(saber))
                {
                    if (StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Instant)
                        restart();
                    else if ((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Button || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                        && (controller.HasValue && IsInputActive(controller.Value, StabToRestartConfig.Instance.SelectedButton)))
                    {
                        restart();
                    } 
                    else if ((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Hold || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                        && _holdTimes[i] > StabToRestartConfig.Instance.MinStabTime)
                    {
                        restart();
                    }
                }
                else
                {
                    _holdTimes[i] = 0f;
                }
                ++i;
            }
        }

        private bool saberCollisionOkay(Saber saber)
        {
            Vector3 saberVector = saber.saberBladeTopPos - saber.saberBladeBottomPos;

            Vector3 ignoreYVector = Vector3.one - Vector3.up;
            float horizontalAngle = Vector3.Angle(Vector3.Scale(saberVector, ignoreYVector), Vector3.Scale(BodyCollider.transform.position - saber.saberBladeBottomPos, ignoreYVector));
            float verticalAngle = Vector3.Angle(saberVector, Vector3.Scale(saberVector, ignoreYVector));
            float speed = saber.bladeSpeed;
            return horizontalAngle < MAX_HORIZONTAL_ANGLE && verticalAngle < MAX_VERTICAL_ANGLE && speed > MIN_SPEED;
        }

        private void initPlayerHeight()
        {
            _automaticHeight = _sceneSetupData.playerSpecificSettings.automaticPlayerHeight;
            if (_automaticHeight)
            {
                _playerHeight = _phd.playerHeight;
                _phd.playerHeightDidChangeEvent += playerHeightChanged;
            }
            else
            {
                _playerHeight = _sceneSetupData.playerSpecificSettings.playerHeight;
            }
        }

        private void playerHeightChanged(float newHeight)
        {
            _playerHeight = newHeight;
            BodyCollider.transform.localScale = Vector3.one + (Vector3.up * ((_playerHeight / 2) - 1)) - (Vector3.forward * 0.25f);
        }

        private void OnDestroy()
        {
            if (_automaticHeight)
            {
                _automaticHeight = false;
                _phd.playerHeightDidChangeEvent -= playerHeightChanged;
            }
            _saberManager.didUpdateSaberPositionsEvent -= HandleSaberMovement;
        }

        private void LateUpdate()
        {
            _holdTimes[0] += Time.deltaTime;
            _holdTimes[1] += Time.deltaTime;
            BodyCollider.transform.position = _mainCamera.transform.position - (Vector3.up * (_playerHeight / 2));
            BodyCollider.transform.Rotate(0, _mainCamera.transform.rotation.eulerAngles.y - BodyCollider.transform.rotation.eulerAngles.y, 0);
        }
        #endregion
    }
}
