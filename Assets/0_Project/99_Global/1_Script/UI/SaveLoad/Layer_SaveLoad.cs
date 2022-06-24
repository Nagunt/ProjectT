using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_SaveLoad : Layer_Default<UI_SaveLoad> {

        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.Global_����UIOpen, OpenUI_Save, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, Close, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_�ε�UIOpen, OpenUI_Load, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_�ε�UIClose, Close, true);
            Event.Global_EventSystem.Scene.onSceneChanged += (a, b) =>
            {
                Event.Global_EventSystem.UI.Call(UIEventID.Global_�ε�UIClose);
            };
            Event.Global_EventSystem.VisualNovel.onSceneReloaded += () =>
            {
                Event.Global_EventSystem.UI.Call(UIEventID.Global_�ε�UIClose);
            };
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

