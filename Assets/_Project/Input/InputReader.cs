using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VS.Utilities.Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader", order = 0)]
    public class InputReader : ScriptableObject, PlayerInputSystem.IPlayerActions {
        public event UnityAction<Vector2> OnMoveEvent = delegate { };
        public event UnityAction<Vector2, bool> OnLookEvent = delegate { };
        public event UnityAction<bool> OnAttackEvent = delegate { };
        public event UnityAction<bool> OnInteractEvent = delegate { };
        public event UnityAction<bool> OnCrouchEvent = delegate { };
        public event UnityAction<bool> OnJumpEvent = delegate { };
        public event UnityAction<bool> OnSprintEvent = delegate { };

        public PlayerInputSystem InputActions;
        public string TargetedDeviceName; // Debug.Log($"Device name: {context.control.device.name}");
        
        public Vector2 Direction => InputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => InputActions.Player.Look.ReadValue<Vector2>();
        
        
        public void EnablePlayerActions() {
            if (InputActions == null) {
                InputActions = new PlayerInputSystem();
                InputActions.Player.SetCallbacks(this);
            }
            
            InputActions.Player.Enable();
        }
        
        public void DisablePlayerActions() {
            if (InputActions != null) {
                InputActions.Player.Disable();
                InputActions.Player.RemoveCallbacks(this);
            }
        }

        public void OnMove(InputAction.CallbackContext context) {
            OnMoveEvent.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnLook(InputAction.CallbackContext context) {
            OnLookEvent.Invoke(context.ReadValue<Vector2>(), IsTargetedDevice(context));
        }
        
        public void OnAttack(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started when IsTargetedDevice(context):
                    OnAttackEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled when IsTargetedDevice(context):
                    OnAttackEvent.Invoke(false);;
                    break;
            }
        }
        
        public void OnInteract(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    OnInteractEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    OnInteractEvent.Invoke(false);
                    break;
            }
        }
        
        public void OnCrouch(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    OnCrouchEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    OnCrouchEvent.Invoke(false);
                    break;
            }
        }
        
        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    OnJumpEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    OnJumpEvent.Invoke(false);
                    break;
            }
        }
        
        public void OnSprint(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    OnSprintEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    OnSprintEvent.Invoke(false);
                    break;
            }
        }
        
        private bool IsTargetedDevice(InputAction.CallbackContext context) {
            return context.control.device.name == TargetedDeviceName;
        }
    }
}