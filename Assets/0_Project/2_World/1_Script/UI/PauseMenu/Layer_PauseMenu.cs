using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_PauseMenu : Layer_Default<UI_PauseMenu> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_정지메뉴UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_정지메뉴UIClose, Close);
        }
    }
}
