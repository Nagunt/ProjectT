using System;
using System.Collections;
using System.Collections.Generic;
using TP.Data;
using TP.VisualNovel;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI {

    public class Layer_Dialogue : Layer_Default<UI_Dialogue> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register<string>(UIEventID.World_대화UI이름설정, SetText_Name);
            Event.Global_EventSystem.UI.Register<string>(UIEventID.World_대화UI직책설정, SetText_Position);
            Event.Global_EventSystem.UI.Register<string, UnityAction>(UIEventID.World_대화UI내용설정, SetText_Dialogue);
            Event.Global_EventSystem.UI.Register<string, UnityAction>(UIEventID.World_대화UI내용수정, ModifyText_Dialogue);
            Event.Global_EventSystem.UI.Register<float>(UIEventID.World_이펙트UI흔들림효과, MakeShake);

            Event.Global_EventSystem.VisualNovel.onLoad += OnLoad;
            Event.Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
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
                _uiObject.SetText_Dialogue(value, true, false, callback);
            }
        }

        private void ModifyText_Dialogue(string value, UnityAction callback) {
            if (_uiObject != null) {
                _uiObject.SetText_Dialogue(value, false, false, callback);
            }
        }

        private void MakeShake(float time) {
            if (_uiObject != null) {
                _uiObject.Effect_Shake(time);
            }
        }

        private void OnLoad(Global_LocalData.Save.SaveData saveData) {
            if (saveData.lastLogData.Check() &&
                _uiObject != null) {
                _uiObject.SetText_Name(saveData.lastLogData.name);
                _uiObject.SetText_Position(CharacterLoader.GetPlacement(saveData.lastLogData.name));
                _uiObject.SetText_Dialogue(saveData.lastLogData.content, true, true, null);
            }
        }

        private void OnSceneReloaded()
        {
            if(_uiObject != null)
            {
                _uiObject.ClearText();
            }
        }

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onLoad -= OnLoad;
            Event.Global_EventSystem.VisualNovel.onSceneReloaded -= OnSceneReloaded;
        }
    }
}

