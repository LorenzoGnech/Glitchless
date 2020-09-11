using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProInput.Scripts {
    /// <summary>
    /// Main class of ProInput
    /// Outside classes should mainly invoke this class
    /// </summary>
    public class ProInput : MonoBehaviour {
        private static ProInput _proInput;
        private static bool _initialized;

        private Dictionary<string, InputObject> _enabledBindings;

        /// <summary>
        /// Initializes ProInput
        /// If already initialized, destroys object
        /// Doesn't destroy on load
        /// </summary>
        private void Awake() {
            // Checks if is initialized
            if (_initialized) {
                Destroy(gameObject);
                return;
            }

            // Initializes ProInput
            _initialized = true;
            _proInput = this;
            DontDestroyOnLoad(gameObject);
            _enabledBindings = new Dictionary<string, InputObject>();
        }

        /// <summary>
        /// Function called on update loop
        /// </summary>
        private void FixedUpdate() {
            var current = Keyboard.current;
            GetInputObjects().ForEach(o => o.Update(current));
        }
        
        /// <summary>
        /// Function called on application exit
        /// </summary>
        private void OnApplicationQuit() {
            GetInputObjects().ForEach(o => o.Save());
        }

        /// <summary>
        /// Converts dictionary to list
        /// </summary>
        /// <returns>InputObject list</returns>
        private List<InputObject> GetInputObjects() {
            return _enabledBindings.Values.ToList();
        }

        /// <summary>
        /// Add inputObject to bindings
        /// </summary>
        /// <param name="inputObject">New input</param>
        public static void RegisterInput(InputObject inputObject) {
            _proInput._enabledBindings.Add(inputObject.Name, inputObject);
        }
    }
}
