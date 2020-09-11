using UnityEngine;

namespace ProSave.Scripts.Binary {
    [System.Serializable]
    public struct BVector2 {
        private float _posX;
        private float _posY;

        public BVector2(float posX, float posY) {
            _posX = posX;
            _posY = posY;
        }
        public BVector2(Vector2 vector3) {
            _posX = vector3.x;
            _posY = vector3.y;
        }

        public float PosX {
            get => _posX;
            set => _posX = value;
        }
        public float PosY {
            get => _posY;
            set => _posY = value;
        }
        public Vector2 Pos {
            get => new Vector2(_posX, _posY);
            set {
                _posX = value.x;
                _posY = value.y;
            }
        }
    }
}