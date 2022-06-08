using System.Collections.Generic;
using System.Collections.ObjectModel;
using TP.VisualNovel;
using UnityEngine;

namespace TP.UI
{

    public class Layer_Book : Layer_Default<UI_Book> {

        private int index = 0;
        private ReadOnlyDictionary<CharacterID, CharacterData> charData;

        protected override void Start() {
            base.Start();
            Debug.Log("���� �̺�Ʈ ���");
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIClose, Close);
            Event.Global_EventSystem.UI.Register<ReadOnlyDictionary<CharacterID, CharacterData>>(UIEventID.World_����UI�����ͼ���, SetData);
            Event.Global_EventSystem.UI.Register<int>(UIEventID.World_����UI�ε�������, SetIndex);
        }

        protected override void Open()
        {
            base.Open();
            if(_uiObject != null)
            {
                _uiObject.SetData(charData);
                _uiObject.SetCursor(Mathf.Max(index, 1));
            }
        }

        private void SetData(ReadOnlyDictionary<CharacterID, CharacterData> data)
        {
            charData = data;
        }

        private void SetIndex(int data)
        {
            index = data;
        }
    }
}
