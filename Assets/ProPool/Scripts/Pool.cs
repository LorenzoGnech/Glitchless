using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProPool.Scripts {
    public class Pool<T> {
        public delegate T CreateObject();
        public delegate void EnableObject(T tObject);
        public delegate void DisableObject(T tObject);
        
        private readonly List<T> _availableObjects;

        private readonly CreateObject _createObject;
        private readonly EnableObject _enableObject;
        private readonly DisableObject _disableObject;

        public Pool(CreateObject createObject, EnableObject enableObject, DisableObject disableObject) {
            _availableObjects = new List<T>();
            
            _createObject = createObject;
            _enableObject = enableObject;
            _disableObject = disableObject;
        }

        public T Depool(float time, MonoBehaviour starter) {
            var newObj = Depool();
            starter.StartCoroutine(EnpoolAfterTime(time, newObj));
            return newObj;
        }
        
        public T Depool() {
            if (_availableObjects.Count == 0) {
                var newObj = _createObject();
                return newObj;
            }
            else {
                var newObj = _availableObjects[0];
                _availableObjects.RemoveAt(0);
                _enableObject(newObj);
                return newObj;
            }
        }

        public void Enpool(T tObject) {
            _disableObject(tObject);
            _availableObjects.Add(tObject);
        }

        private IEnumerator EnpoolAfterTime(float time, T data) {
            yield return new WaitForSeconds(time);
            Enpool(data);
        }
    }
}