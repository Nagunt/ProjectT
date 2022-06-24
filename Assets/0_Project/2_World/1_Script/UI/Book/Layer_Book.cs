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
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIClose, Close);
            Event.Global_EventSystem.UI.Register<int>(UIEventID.World_����UI�ε�������, SetIndex);
            Event.Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
        }

        protected override void Open()
        {
            base.Open();
            if(_uiObject != null)
            {
                _uiObject.SetCursor(Mathf.Max(index, 1));
            }
        }

        private void SetIndex(int data)
        {
            index = data;
        }

        private void OnSceneReloaded()
        {
            Close();
        }

        private void OnDestroy()
        {
            Event.Global_EventSystem.VisualNovel.onSceneReloaded -= OnSceneReloaded;
        }
    }
}
