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
            Event.Global_EventSystem.UI.Register<string, UnityAction, bool>(UIEventID.World_����ƮUIŸ��Ʋ����, MakeTitle);
        }

        private void MakeTitle(string data, UnityAction onComplete, bool isDark)
        {
            if(_uiObject != null)
            {
                _uiObject.MakeTitleEffect(data, onComplete, isDark, Event.Global_EventSystem.VisualNovel.Skip);
            }
        }
    }





}
