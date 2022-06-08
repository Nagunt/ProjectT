using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_VisualNovel : Layer_Default<UI_VisualNovel> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register<Sprite>(UIEventID.World_비주얼노벨UI배경설정, SetBackground);
            Event.Global_EventSystem.UI.Register<Sprite>(UIEventID.World_비주얼노벨UI필터설정, SetFilter);
            Event.Global_EventSystem.UI.Register<int, Sprite>(UIEventID.World_비주얼노벨UI캐릭터설정, SetSprite);
        }

        private void SetSprite(int index, Sprite sprite) {
            if (_uiObject != null) {
                _uiObject.SetSprite(index, sprite);
            }
        }

        private void SetFilter(Sprite sprite) {
            if (_uiObject != null) {
                _uiObject.SetFilter(sprite);
            }
        }

        private void SetBackground(Sprite sprite) {
            if (_uiObject != null) {
                _uiObject.SetBackground(sprite);
            }
        }
    }
}

