using System.Collections;
using System.Collections.Generic;
using TP.Event;

namespace TP.UI{

    public class Layer_Loading : Layer_Default<UI_Loading> {
        protected override void Start() {
            base.Start();
            Global_EventSystem.UI.Register(UIEventID.Global_�ε�UIOpen, Open, true);
            Global_EventSystem.UI.Register(UIEventID.Global_�ε�UIClose, Close, true);
            Global_EventSystem.UI.Register<float>(UIEventID.Global_�ε�UI���൵����, SetProgress, true);
        }

        protected override void OnUIOpened() {
            base.OnUIOpened();
            SetProgress(0);
        }

        private void SetProgress(float value) {
            if(_uiObject != null) {
                _uiObject.SetProgress(value);
            }
        }
    }
}
