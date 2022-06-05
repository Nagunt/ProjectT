using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_Log : Layer_Default<UI_Log> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_로그UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_로그UIClose, Close);
        }
    }
}

