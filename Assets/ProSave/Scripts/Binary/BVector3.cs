using UnityEngine;

namespace ProSave.Scripts.Binary {
    [System.Serializable]
    public struct BVector3 {
        private float _posX;
        private float _posY;
        private float _posZ;

        public BVector3(float posX, float posY, float posZ) {
            _posX = posX;
            _posY = posY;
            _posZ = posZ;
        }
        public BVector3(Vector3 vector3) {
            _posX = vector3.x;
            _posY = vector3.y;
            _posZ = vector3.z;
        }

        public float PosX {
            get => _posX;
            set => _posX = value;
        }
        public float PosY {
            get => _posY;
            set => _posY = value;
        }
        public float PosZ {
            get => _posZ;
            set => _posZ = value;
        }
        public Vector3 Pos {
            get => new Vector3(_posX, _posY, _posZ);
            set {
                _posX = value.x;
                _posY = value.y;
                _posZ = value.z;
            }
        }
    }
}