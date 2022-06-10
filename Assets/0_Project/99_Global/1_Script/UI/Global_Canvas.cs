using System.Collections;
using System.Collections.Generic;
using TP.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {
    public class Global_Canvas : MonoBehaviour {

        private static Global_Canvas m_instance;

        [SerializeField]
        private RectTransform m_RectTransform;
        [SerializeField]
        private Canvas m_Canvas; 

        private Camera mainCamera;

        private void Awake() {
            m_instance = this;
            Event.Global_EventSystem.Scene.onSceneChanged += OnSceneChanged;
        }

        public static Vector2 ScreenToLocalPosition(Vector2 position) {
            Vector2 canvasSize = m_instance.m_RectTransform.sizeDelta;
            Vector2 screenSize = m_instance.mainCamera.pixelRect.size;
            Vector2 offset = new Vector2(Screen.width * m_instance.mainCamera.rect.x, Screen.height * m_instance.mainCamera.rect.y);
            Vector2 positionRatio = new Vector2((position.x - offset.x) / screenSize.x, (position.y - offset.y) / screenSize.y);
            return new Vector2(canvasSize.x * positionRatio.x, canvasSize.y * positionRatio.y);
        }

        private void OnSceneChanged(SceneID current, SceneID next) {
            mainCamera = Camera.main;
            m_Canvas.worldCamera = mainCamera;
        }
    }
}

