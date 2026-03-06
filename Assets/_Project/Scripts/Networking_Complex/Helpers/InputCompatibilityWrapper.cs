using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VS.NetcodeExampleProject.Networking {
    public enum KeyDownCheckState {
        Singular,
        Continuous
    }

    public static class InputCompatibilityWrapper {
        private static Mouse _currentMouse;
        private static Keyboard _currentKeyboard;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ClearStaticState() {
            _currentMouse = null;
            _currentKeyboard = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckTabKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).tabKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).tabKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.Tab)
                : Input.GetKey(KeyCode.Tab);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).tabKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).tabKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckEscapeKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).escapeKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).escapeKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.Escape)
                : Input.GetKey(KeyCode.Escape);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).escapeKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).escapeKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckWKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).wKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).wKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.W)
                : Input.GetKey(KeyCode.W);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).wKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).wKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckAKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).aKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).aKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.A)
                : Input.GetKey(KeyCode.A);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).aKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).aKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckSKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).sKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).sKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.S)
                : Input.GetKey(KeyCode.S);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).sKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).sKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckDKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).dKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).dKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.D)
                : Input.GetKey(KeyCode.D);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).dKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).dKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckFKeyPressed(KeyDownCheckState checkState = KeyDownCheckState.Singular) {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).fKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).fKey.isPressed;

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return checkState == KeyDownCheckState.Singular
                ? Input.GetKeyDown(KeyCode.F)
                : Input.GetKey(KeyCode.F);

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return checkState == KeyDownCheckState.Singular
                ? (_currentKeyboard ??= Keyboard.current).fKey.wasPressedThisFrame
                : (_currentKeyboard ??= Keyboard.current).fKey.isPressed;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetMouseDelta() {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return (_currentMouse ??= Mouse.current).delta.ReadValue();

#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            return (_currentMouse ??= Mouse.current).delta.ReadValue();
#endif
        }
    }
}