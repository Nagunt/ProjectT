using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP {
    [RequireComponent(typeof(Camera))]
    public class Global_CameraCrop : MonoBehaviour {
        [Header("- Camera")]
        [SerializeField]
        private new Camera camera;
        [Header("- Aspect Ratio")]
        public Vector2 maxTargetAspect = new Vector2(2400, 1080);
        public Vector2 minTargetAspect = new Vector2(1920, 1080);

        public static Vector2 ScreenRatio { get; private set; }

        void Awake() {
            UpdateCrop(new Vector2(Screen.width, Screen.height));
        }

        void OnPreCull() => GL.Clear(true, true, Color.black);

        public void UpdateCrop(Vector2 screen) {
            float screenRatio = screen.x / screen.y;
            float minTargetRatio = minTargetAspect.x / maxTargetAspect.y;
            float maxTargetRatio = maxTargetAspect.x / maxTargetAspect.y;
            if (maxTargetRatio < screenRatio) {
                float normalizedWidth = maxTargetRatio / screenRatio;
                float barThickness = (1f - normalizedWidth) / 2f;
                camera.rect = new Rect(barThickness, 0, normalizedWidth, 1);
            }
            else if (minTargetRatio > screenRatio) {
                float normalizedHeight = screenRatio / minTargetRatio;
                float barThickness = (1f - normalizedHeight) / 2f;
                camera.rect = new Rect(0, barThickness, 1, normalizedHeight);
            }
            else {
                camera.rect = new Rect(0, 0, 1, 1);
            }
            ScreenRatio = new Vector2(Mathf.Clamp(screenRatio, minTargetRatio, maxTargetRatio), 1);
        }
    }
}
