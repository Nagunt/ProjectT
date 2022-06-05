using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {


    public class UI_QuickMenu : UI_Default {
        [Header("- QuickMenu")]
        [SerializeField]
        private Button button_Pause;
        [SerializeField]
        private Button button_QuickSave;
        [SerializeField]
        private Button button_QuickLoad;
        [SerializeField]
        private Button button_Log;
        [SerializeField]
        private Button button_Skip;

        protected override void Start() {
            button_Pause.onClick.AddListener(OnClick_Pause);
            button_QuickSave.onClick.AddListener(OnClick_QuickSave);
            button_QuickLoad.onClick.AddListener(OnClick_QuickLoad);
            button_Log.onClick.AddListener(OnClick_Log);
            button_Skip.onClick.AddListener(OnClick_Skip);
        }

        private void OnClick_Pause() {
            Event.Global_EventSystem.UI.Call(UIEventID.World_정지메뉴UIOpen);
        }

        private void OnClick_QuickSave() {
            Debug.Log("QuickSave");
        }

        private void OnClick_QuickLoad() {
            Debug.Log("QuickLoad");
        }

        private void OnClick_Log() {
            Event.Global_EventSystem.UI.Call(UIEventID.World_로그UIOpen);
        }

        private void OnClick_Skip() {
            Debug.Log("Skip");
        }
    }
}

