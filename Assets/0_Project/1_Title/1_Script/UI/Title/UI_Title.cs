using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TP.Scene;

namespace TP.UI {
    public class UI_Title : UI_Default {
        [Header("- Title")]
        [SerializeField]
        private Button button_Start;
        [SerializeField]
        private Button button_Load;
        [SerializeField]
        private Button button_Setting;
        [SerializeField]
        private Button button_Credit;

        protected override void Start() {
            button_Start.image.alphaHitTestMinimumThreshold = 0.1f;
            button_Load.image.alphaHitTestMinimumThreshold = 0.1f;
            button_Setting.image.alphaHitTestMinimumThreshold = 0.1f;
            button_Credit.image.alphaHitTestMinimumThreshold = 0.1f;

            button_Start.onClick.AddListener(OnClick_Start);
            button_Load.onClick.AddListener(OnClick_Load);
            button_Setting.onClick.AddListener(OnClick_Setting);
            button_Credit.onClick.AddListener(OnClick_Credit);
        }

        private void OnClick_Start() {
            Global_SceneManager.LoadScene(SceneID.World);
        }

        private void OnClick_Load() {
            Event.Global_EventSystem.UI.Call(UIEventID.Global_로드UIOpen);
        }

        private void OnClick_Setting() {
            Event.Global_EventSystem.UI.Call(UIEventID.Global_설정UIOpen);
        }

        private void OnClick_Credit() {
            Debug.Log("credit");
        }

    }
}
