using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Basis: A script that handles player input using the New Player Input System. It works with multiplayer.
// Source: This script is taken from: https://github.com/adammyhre/Advanced-Player-Controller/blob/master/Assets/_Project/Scripts/InputReader.cs
namespace VS.Utilities.Input {
    // This approach inherits from the 'IPlayerActions' interface that is in the auto generated 'PlayerInputActions' class.
    // When adding an input action to the action map, the interface requires that you implement its respective method here. This makes everything much more manageable.
    // Inheriting from 'ScriptableObject' just encapsulates all input logic into an easy-to-use object in the inspector.
    // It also removes the need to have a whole script (component) for managing input. 
    // It also removes the need of checking if the player 'IsOwner' with Netcode. 
    // 'CreateAssetMenu' allows for the scriptable object to be created in the inspector.
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader", order = 0)]
    public class InputReader : ScriptableObject, PlayerInputSystem.IPlayerActions {
        // A 'UnityAction' is basically just the C# 'Action' delegate, but with some special Unity-specific backing.
        // They are assigned to an empty delegate to prevent the need to null check when firing events.
        // You could just as easily not assign the variable to an empty delegate and invoke it like: action?.Invoke();
        // 'Action<>' delegates are used instead of 'Func<>' because we don't want a return value.
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
        
        
        // Enables ActionMap so that it can be used by the player or whatever object.
        public void EnablePlayerActions() {
            if (InputActions == null) {
                InputActions = new PlayerInputSystem();
                InputActions.Player.SetCallbacks(this);
            }
            
            InputActions.Player.Enable();
        }
        
        // Disables ActionMap for cleanups and other purposes.
        public void DisablePlayerActions() {
            if (InputActions != null) {
                InputActions.Player.Disable();
                InputActions.Player.RemoveCallbacks(this);
            }
        }
        
        //***********************************************************************//
        // The rest of the script is implementation of 'IPlayerActions' members 
        // The methods are called when a certain action occurs (a respective input is pressed)
        // An event is then fired, and everyone subscribed to the event will receive information and act accordingly.
        public void OnMove(InputAction.CallbackContext context) {
            OnMoveEvent.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnLook(InputAction.CallbackContext context) {
            OnLookEvent.Invoke(context.ReadValue<Vector2>(), IsTargetedDevice(context));
        }
        
        public void OnAttack(InputAction.CallbackContext context) {
            // Each input has 'context' phases describing the input. For example, when it was started, stopped, its duration, etc.
            // Documentation about this: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputAction.CallbackContext.html
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