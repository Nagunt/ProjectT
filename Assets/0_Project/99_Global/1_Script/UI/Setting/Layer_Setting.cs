using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_Setting : Layer_Default<UI_Setting> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.Global_����UIOpen, Open, true);
            Event.Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, Close, true);
        }
    }
}
