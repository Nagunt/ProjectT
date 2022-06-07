namespace TP.UI {

    public class Layer_Book : Layer_Default<UI_Book> {

        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_도감UIClose, Close);
        }
    }
}
