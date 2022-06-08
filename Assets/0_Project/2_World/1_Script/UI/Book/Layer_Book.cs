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
            Debug.Log("도감 이벤트 등록");
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIClose, Close);
            Event.Global_EventSystem.UI.Register<ReadOnlyDictionary<CharacterID, CharacterData>>(UIEventID.World_도감UI데이터설정, SetData);
            Event.Global_EventSystem.UI.Register<int>(UIEventID.World_도감UI인덱스설정, SetIndex);
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
