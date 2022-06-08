using System.Collections.Generic;
using TP.VisualNovel;
using UnityEngine;

namespace TP.UI
{

    public class Layer_Book : Layer_Default<UI_Book> {

        private int index = 0;
        private IReadOnlyDictionary<CharacterID, CharacterData> charData;

        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIClose, Close);
            Event.Global_EventSystem.UI.Register<IReadOnlyDictionary<CharacterID, CharacterData>>(UIEventID.World_도감UI데이터설정, SetData);
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

        private void SetData(IReadOnlyDictionary<CharacterID, CharacterData> data)
        {
            charData = data;
        }

        private void SetIndex(int data)
        {
            index = data;
        }
    }
}
