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
            Event.Global_EventSystem.UI.Register<float>(UIEventID.World_����ƮUI��鸲ȿ��, MakeShake);

            Event.Global_EventSystem.VisualNovel.onSave += OnSave;
            Event.Global_EventSystem.VisualNovel.onLoad += OnLoad;
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

        private void MakeShake(float time) {
            if(_uiObject != null) {
                _uiObject.Effect_Shake(time);
            }
        }

        private void OnSave(Data.Global_LocalData.Save.SaveData saveData) {
            if(_uiObject != null) {
                saveData.spriteData = _uiObject.ToData();
            }
        }

        private void OnLoad(Data.Global_LocalData.Save.SaveData saveData) {
            if (_uiObject != null) {
                _uiObject.Parse(saveData.spriteData);
            }
        }

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onSave -= OnSave;
            Event.Global_EventSystem.VisualNovel.onLoad -= OnLoad;
        }
    }
}

