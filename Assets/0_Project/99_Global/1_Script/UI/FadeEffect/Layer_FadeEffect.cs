using TP.Event;
using UnityEngine.Events;

namespace TP.UI {
    public class Layer_FadeEffect : Layer_Default<UI_FadeEffect> {
        protected override void Start() {
            base.Start();
            Global_EventSystem.UI.Register<float, UnityAction>(UIEventID.Global_FadeIn, FadeIn, true);
            Global_EventSystem.UI.Register<float, UnityAction>(UIEventID.Global_FadeOut, FadeOut, true);
        }

        private void FadeIn(float time, UnityAction onComplete) {
            if (_uiObject != null) {
                _uiObject.FadeIn(time, onComplete);
            }
        }

        private void FadeOut(float time, UnityAction onComplete) {
            if (_uiObject != null) {
                _uiObject.FadeOut(time, onComplete);
            }
        }
    }
}

