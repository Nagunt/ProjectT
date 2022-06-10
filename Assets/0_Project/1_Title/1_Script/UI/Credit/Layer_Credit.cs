using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_Credit : Layer_Default<UI_Credit> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.Title_제작진UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.Title_제작진UIClose, Close);
        }
    }



}
