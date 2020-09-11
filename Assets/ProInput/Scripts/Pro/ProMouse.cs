using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ProInput.Scripts {
    public class CustomMouse {
        private readonly ButtonControl _buttonControl;
        private readonly bool _enabled;

        public CustomMouse(ButtonControl buttonControl) {
            _buttonControl = buttonControl;
            _enabled = true;
        }
        
        public CustomMouse() {
            _enabled = false;
        }

        public bool IsPressed => _enabled && ProMouse.MouseActive && _buttonControl.isPressed;
        public bool IsDown => _enabled && ProMouse.MouseActive && _buttonControl.wasPressedThisFrame;
        public bool IsUp => _enabled && ProMouse.MouseActive && _buttonControl.wasReleasedThisFrame;
    }
    
    public static class ProMouse {
        public static CustomMouse LeftButton => MouseActive ? new CustomMouse(Current.leftButton) : new CustomMouse();
        public static CustomMouse RightButton => MouseActive ? new CustomMouse(Current.rightButton) : new CustomMouse();
        public static Vector2 Position => MouseActive ? Current.position.ReadValue() : Vector2.zero;
        public static bool MouseActive => Current != null && Current.enabled;
        public static Mouse Current => Mouse.current;
    }
}