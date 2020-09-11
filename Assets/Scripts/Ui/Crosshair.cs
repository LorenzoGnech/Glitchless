using UnityEngine;
using UnityEngine.UI;

namespace Ui {
    public class Crosshair : MonoBehaviour {
        public Transform playerCamera;
        public Image crosshairImage;
        public Color notFoundColor;
        public Color foundColor;
        public float colorSpeed;
        public int maxDistance;

        private void Update() {
            int layerMask = 1 << 9;
            RaycastHit hit;
            crosshairImage.color = Color.Lerp(crosshairImage.color,
                Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, layerMask) ? foundColor : notFoundColor,
                colorSpeed * Time.deltaTime);
        }
    }
}