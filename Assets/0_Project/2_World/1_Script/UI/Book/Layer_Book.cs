namespace TP.UI {

    public class Layer_Book : Layer_Default<UI_Book> {

        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_����UIClose, Close);
        }
    }
}
