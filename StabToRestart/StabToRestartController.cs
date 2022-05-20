using IPA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace StabToRestart
{
    public class StabToRestartController : MonoBehaviour
    {
        private const float MAX_HORIZONTAL_ANGLE = 20f;
        private const float MAX_VERTICAL_ANGLE = 10f;
        private const float MIN_SPEED = 2f;

        private float _playerHeight;
        private bool _automaticHeight;

        [Inject]
        private readonly SaberManager _saberManager;

        [Inject]
        private readonly PlayerHeightDetector _phd;

        [Inject]
        private readonly MainCamera _mainCamera;

        [Inject]
        private readonly CapsuleCollider _bodyCollider;

        [Inject]
        private readonly ILevelRestartController _restartController;

        [Inject]
        private readonly GameplayCoreSceneSetupData _sceneSetupData;

        #region Monobehaviour Messages

        private void Start()
        {
            initPlayerHeight();

            _bodyCollider.transform.localScale = Vector3.one + (Vector3.up * ((_playerHeight / 2) - 1)) - (Vector3.forward * 0.25f);

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

            if (_bodyCollider.Raycast(new Ray(leftSaber.saberBladeBottomPos, leftSaberVector), out _, leftSaberLength * 0.75f))
            {
                if (saberCollisionOkay(leftSaber))
                {
                    restart();
                }
            }

            if (_bodyCollider.Raycast(new Ray(rightSaber.saberBladeBottomPos, rightSaberVector), out _, rightSaberLength))
            {
                if (saberCollisionOkay(rightSaber))
                {
                    restart();
                }
            }
        }

        private bool saberCollisionOkay(Saber saber)
        {
            Vector3 saberVector = saber.saberBladeTopPos - saber.saberBladeBottomPos;

            Vector3 ignoreYVector = Vector3.one - Vector3.up;
            Vector3 onlyYVector = Vector3.up;
            float horizontalAngle = Vector3.Angle(Vector3.Scale(saberVector, ignoreYVector), Vector3.Scale(_bodyCollider.transform.position - saber.saberBladeBottomPos, ignoreYVector));
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
            _bodyCollider.transform.localScale = Vector3.one + (Vector3.up * ((_playerHeight / 2) - 1)) - (Vector3.forward * 0.25f);
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
            _bodyCollider.transform.position = _mainCamera.transform.position - (Vector3.up * (_playerHeight / 2));
            _bodyCollider.transform.Rotate(0, _mainCamera.transform.rotation.eulerAngles.y - _bodyCollider.transform.rotation.eulerAngles.y, 0);
        }
        #endregion
    }
}
