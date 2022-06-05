using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_SaveLoad : Layer_Default<UI_SaveLoad> {

        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.Global_저장UIOpen, OpenUI_Save, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_저장UIClose, Close, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_로드UIOpen, OpenUI_Load, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_로드UIClose, Close, true);
        }

        private void OpenUI_Save() {
            if (IsOpen == false) {
                Open();
                if (_uiObject != null) {
                    _uiObject.SetUIMode(true);
                }
            }
        }

        private void OpenUI_Load() {
            if (IsOpen == false) {
                Open();
                if (_uiObject != null) {
                    _uiObject.SetUIMode(false);
                }
            }
        }
    }
}

