using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI
{
    public class Layer_Effect : Layer_Default<UI_Effect>
    {
        protected override void Start()
        {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_이펙트UI검은화면설정, SetDark);
            Event.Global_EventSystem.UI.Register(UIEventID.World_이펙트UI검은화면해제, RemoveDark);
            Event.Global_EventSystem.UI.Register<float, UnityAction>(UIEventID.World_이펙트UI흔들림효과, MakeShake);
            Event.Global_EventSystem.UI.Register<string, UnityAction, bool>(UIEventID.World_이펙트UI타이틀생성, MakeTitle);
        }
        
        public void SetDark()
        {
            if(_uiObject != null)
            {
                _uiObject.SetDark();
            }
        }

        public void RemoveDark()
        {
            if (_uiObject != null)
            {
                _uiObject.RemoveDark();
            }
        }

        private void MakeTitle(string data, UnityAction onComplete, bool isDark)
        {
            if(_uiObject != null)
            {
                _uiObject.Effect_Title(data, onComplete, isDark, Event.Global_EventSystem.VisualNovel.Skip);
            }
        }

        private void MakeShake(float time, UnityAction onComplete) {
            if (_uiObject != null) {
                _uiObject.Effect_Shake(time, onComplete);
            }
        }
    }
}
