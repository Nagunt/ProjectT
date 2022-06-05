using System.Collections;
using System.Collections.Generic;
using TP.Event;
using UnityEngine;

namespace TP.UI {

    public class Layer_TouchBlock : Layer_Default<UI_TouchBlock> {
        protected override void Start() {
            base.Start();
            Global_EventSystem.UI.Register(UIEventID.Global_��ġ��ݼ���, Lock, true);
            Global_EventSystem.UI.Register(UIEventID.Global_��ġ�������, Unlock, true);
        }

        private void Lock() {
            if(_uiObject != null) {
                _uiObject.Lock();
            }
        }

        private void Unlock() {
            if(_uiObject != null) {
                _uiObject.Unlock();
            }
        }
    }
}


