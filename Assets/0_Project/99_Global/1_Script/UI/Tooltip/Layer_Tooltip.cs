using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TP.UI {

    public class Layer_Tooltip : Layer_Default<UI_Tooltip> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register<string, Vector2>(UIEventID.Global_ÅøÆÁUIOpen, SetTooltip, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_ÅøÆÁUIClose, Close, true);
        }

        private void SetTooltip(string data, Vector2 position) {
            if (IsOpen == false) {
                Open();
                if (_uiObject != null) {
                    _uiObject.SetTooltip(data, position);
                }
            }
        }
    }
}

