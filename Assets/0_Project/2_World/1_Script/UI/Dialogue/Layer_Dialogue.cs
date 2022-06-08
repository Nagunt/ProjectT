using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI {

    public class Layer_Dialogue : Layer_Default<UI_Dialogue> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register<string>(UIEventID.World_��ȭUI�̸�����, SetText_Name);
            Event.Global_EventSystem.UI.Register<string>(UIEventID.World_��ȭUI��å����, SetText_Position);
            Event.Global_EventSystem.UI.Register<string, UnityAction>(UIEventID.World_��ȭUI���뼳��, SetText_Dialogue);
            Event.Global_EventSystem.UI.Register<string, UnityAction>(UIEventID.World_��ȭUI�������, ModifyText_Dialogue);
        }

        private void SetText_Name(string value) {
            if (_uiObject != null) {
                _uiObject.SetText_Name(value);
            }
        }

        private void SetText_Position(string value) {
            if (_uiObject != null) {
                _uiObject.SetText_Position(value);
            }
        }

        private void SetText_Dialogue(string value, UnityAction callback) {
            if(_uiObject != null) {
                _uiObject.SetText_Dialogue(value, true, callback);
            }
        }

        private void ModifyText_Dialogue(string value, UnityAction callback) {
            if (_uiObject != null) {
                _uiObject.SetText_Dialogue(value, false, callback);
            }
        }

    }
}

