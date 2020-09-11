using System.Linq;
using ProInput.Scripts.Binary;
using ProSave.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProInput.Scripts {
    /// <summary>
    /// Binding class
    /// </summary>
    public class InputObject {
        // Default folder for bindings
        private const string BindingsFolder = "Bindings";
        
        // Handle used to save binding
        private readonly SaveHandle<BKey> _saveHandle;
        private readonly string _name;
        private Key _binding;

        private bool _isPressed;
        private bool _isDown;
        private bool _isUp;

        /// <summary>
        /// Constructor for InputObject
        /// </summary>
        /// <param name="name">Name of the input</param>
        /// <param name="defaultValue">Default binding</param>
        public InputObject(string name, Key defaultValue) {
            var directory = $"{Application.persistentDataPath}/{BindingsFolder}";
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            
            _name = name;
            _saveHandle = new SaveHandle<BKey>($"{BindingsFolder}/{name}_bind");

            var loadedData = _saveHandle.LoadData();
            _binding = loadedData ?? defaultValue;
            ProInput.RegisterInput(this);
        }

        /// <summary>
        /// Update input state
        /// </summary>
        /// <param name="current">Active keyboard</param>
        public void Update(Keyboard current) {
            var input = current.allKeys.First(control => control.keyCode == _binding);
            var pressed = input.isPressed;
            _isDown = !_isPressed && pressed;
            _isUp = _isPressed && !pressed;
            _isPressed = pressed;
        }

        /// <summary>
        /// Function called on application exit or saving
        /// </summary>
        public void Save() {
            _saveHandle.SaveData(_binding);
        }

        /// <summary>
        /// Bind input to new key
        /// </summary>
        public void Bind(Key key) {
            _binding = key;
            Save();
        }

        public string Name => _name;
        public bool IsPressed => _isPressed;
        public bool IsDown => _isDown;
        public bool IsUp => _isUp;
    }
}