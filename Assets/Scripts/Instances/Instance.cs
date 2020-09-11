using System;
using System.Collections.Generic;
using UnityEngine;

namespace Instances {
    public class Instance : MonoBehaviour {
        private static Instance _instance;
        private Dictionary<Type, object> _instances;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _instances = new Dictionary<Type, object>();
        }

        public static T Get<T>() {
            var type = typeof(T);
            if (!_instance._instances.ContainsKey(type)) return default;
            return (T)_instance._instances[type];
        }

        public static void Set<T>(object data) {
            var type = typeof(T);
            if (_instance._instances.ContainsKey(type))
                _instance._instances[type] = data;
            else
                _instance._instances.Add(type, data);
        }

        public static bool Has<T>() {
            return _instance._instances.ContainsKey(typeof(T));
        }
    }
}