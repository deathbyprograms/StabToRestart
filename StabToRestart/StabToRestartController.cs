using UnityEngine;
using UnityEngine.XR;
using StabToRestart.Utilities;
using StabToRestart.Configuration;
using System;
using System.Collections.Generic;
using Zenject;

namespace StabToRestart
{
    public class StabToRestartController : MonoBehaviour
    {
        private readonly float MAX_HORIZONTAL_ANGLE = StabToRestartConfig.Instance.MaxHorizontal;
        private readonly float MAX_VERTICAL_ANGLE = StabToRestartConfig.Instance.MaxVertical;
        private readonly float MIN_SPEED = 2f;

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

        private float _leftHoldTime { get; set; }
        private float _rightHoldTime { get; set; }

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

        public void HandleSaberMovement(Saber leftSaber, Saber rightSaber)
        {
            Vector3 leftSaberVector = leftSaber.saberBladeTopPos - leftSaber.saberBladeBottomPos;
            Vector3 rightSaberVector = rightSaber.saberBladeTopPos - rightSaber.saberBladeBottomPos;

            float leftSaberLength = leftSaberVector.magnitude;
            float rightSaberLength = rightSaberVector.magnitude;

            if (BodyCollider.Raycast(new Ray(leftSaber.saberBladeBottomPos, leftSaberVector), out _, leftSaberLength * 0.75f) && saberCollisionOkay(leftSaber))
            {
                bool buttonIsPressed;
                if(StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Instant)
                {
                    restart();
                }
                else if ((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Button || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                    && (_leftController.HasValue && _leftController.Value.TryGetFeatureValue(StabToRestartConfig.Instance.SelectedButton, out buttonIsPressed) && buttonIsPressed))
                {
                    restart();
                } 
                else if((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Hold || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                    && _leftHoldTime > StabToRestartConfig.Instance.MinStabTime)
                {
                    restart();
                }
            }
            else
            {
                _leftHoldTime = 0f;
            }

            if (BodyCollider.Raycast(new Ray(rightSaber.saberBladeBottomPos, rightSaberVector), out _, rightSaberLength * 0.75f) && saberCollisionOkay(rightSaber))
            {
                bool buttonIsPressed;
                if (StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Instant)
                {
                    restart();
                }
                else if ((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Button || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                    && (_rightController.HasValue && _rightController.Value.TryGetFeatureValue(StabToRestartConfig.Instance.SelectedButton, out buttonIsPressed) && buttonIsPressed))
                {
                    restart();
                }
                else if ((StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Hold || StabToRestartConfig.Instance.Mode == StabToRestartConfig.TriggerCondition.Both)
                    && _rightHoldTime > StabToRestartConfig.Instance.MinStabTime)
                {
                    restart();
                }
            }
            else
            {
                _rightHoldTime = 0f;
            }
        }

        private bool saberCollisionOkay(Saber saber)
        {
            Vector3 saberVector = saber.saberBladeTopPos - saber.saberBladeBottomPos;

            Vector3 ignoreYVector = Vector3.one - Vector3.up;
            Vector3 onlyYVector = Vector3.up;
            float horizontalAngle = Vector3.Angle(Vector3.Scale(saberVector, ignoreYVector), Vector3.Scale(BodyCollider.transform.position - saber.saberBladeBottomPos, ignoreYVector));
            float verticalAngle = Vector3.Angle(Vector3.Scale(saberVector, onlyYVector), Vector3.zero);
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
            _leftHoldTime += Time.deltaTime;
            _rightHoldTime += Time.deltaTime;
            BodyCollider.transform.position = _mainCamera.transform.position - (Vector3.up * (_playerHeight / 2));
            BodyCollider.transform.Rotate(0, _mainCamera.transform.rotation.eulerAngles.y - BodyCollider.transform.rotation.eulerAngles.y, 0);
        }
        #endregion
    }
}
