using Unity.Netcode;
using UnityEngine;
using VS.Utilities.Input;

namespace VS.NetcodeExampleProject.Player {
    public class CharacterCameraController : NetworkBehaviour {
        [SerializeField] private Transform followTarget;
        [SerializeField] private InputReader inputReader;

        [Space, Header("Settings")] 
        [SerializeField] private float rotationSpeed;
        [SerializeField] [Range(0f, 90f)] private float upperVerticalLimit;
        [SerializeField] [Range(0f, 90f)] private float lowerVerticalLimit;
        [SerializeField] [Range(1f, 50f)] private float cameraSmoothingFactor;
        [SerializeField] private bool smoothCameraRotation;

        private float _currentXAngle;
        private float _currentYAngle;
        

        private void Awake() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void OnNetworkSpawn() {
            if (!IsOwner) {
                GetComponentInChildren<AudioListener>().enabled = false;
                GetComponentInChildren<Camera>().enabled = false;
                enabled = false;
            }
        }
        
        private void LateUpdate() => RotateCamera(inputReader.LookDirection.x, -inputReader.LookDirection.y);

        private void RotateCamera(float horizontalInput, float verticalInput) {
            if (smoothCameraRotation) {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }

            _currentXAngle += verticalInput * rotationSpeed * Time.deltaTime;
            _currentYAngle += horizontalInput * rotationSpeed * Time.deltaTime;

            _currentXAngle = Mathf.Clamp(_currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

            followTarget.localRotation = Quaternion.Euler(_currentXAngle, _currentYAngle, 0);
        }
    }
}