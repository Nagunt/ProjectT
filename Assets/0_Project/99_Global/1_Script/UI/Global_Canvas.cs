using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {
    public class Global_Canvas : MonoBehaviour {

        private static Global_Canvas m_instance;

        [SerializeField]
        private RectTransform m_RectTransform;

        private Camera mainCamera;

        private void Awake() {
            m_instance = this;
        }

        private void Start() {
            Event.Global_EventSystem.Scene.onSceneChanged += (a, b) => {
                mainCamera = Camera.main;
            };
        }

        public static Vector2 ScreenToLocalPosition(Vector2 position) {
            Vector2 canvasSize = m_instance.m_RectTransform.sizeDelta;
            Vector2 screenSize = m_instance.mainCamera.pixelRect.size;
            Vector2 positionRatio = new Vector2(position.x / screenSize.x, position.y / screenSize.y);
            return new Vector2(canvasSize.x * positionRatio.x, canvasSize.y * positionRatio.y);
        }
    }
}

