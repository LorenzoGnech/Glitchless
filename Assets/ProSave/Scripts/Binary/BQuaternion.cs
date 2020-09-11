using UnityEngine;

namespace ProSave.Scripts.Binary {
    [System.Serializable]
    public struct BQuaternion {
        private float _rotX;
        private float _rotY;
        private float _rotZ;
        private float _rotW;

        public BQuaternion(float rotX, float rotY, float rotZ, float rotW) {
            _rotX = rotX;
            _rotY = rotY;
            _rotZ = rotZ;
            _rotW = rotW;
        }
        public BQuaternion(Quaternion quaternion) {
            _rotX = quaternion.x;
            _rotY = quaternion.y;
            _rotZ = quaternion.z;
            _rotW = quaternion.w;
        }

        public float RotX {
            get => _rotX;
            set => _rotX = value;
        }
        public float RotY {
            get => _rotY;
            set => _rotY = value;
        }
        public float RotZ {
            get => _rotZ;
            set => _rotZ = value;
        }
        public float RotW {
            get => _rotW;
            set => _rotW = value;
        }
        public Quaternion Rot {
            get => new Quaternion(_rotX, _rotY, _rotZ, _rotW);
            set {
                _rotX = value.x;
                _rotY = value.y;
                _rotZ = value.z;
                _rotW = value.w;
            }
        }
        public Vector3 Euler {
            get => Rot.eulerAngles;
            set => Rot = Quaternion.Euler(value);
        }
    }
}