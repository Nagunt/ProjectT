using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_VisualNovel : Layer_Default<UI_VisualNovel> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register<Sprite>(UIEventID.World_���־�뺧UI��漳��, SetBackground);
            Event.Global_EventSystem.UI.Register<Sprite>(UIEventID.World_���־�뺧UI���ͼ���, SetFilter);
            Event.Global_EventSystem.UI.Register<int, Sprite>(UIEventID.World_���־�뺧UIĳ���ͼ���, SetSprite);
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

