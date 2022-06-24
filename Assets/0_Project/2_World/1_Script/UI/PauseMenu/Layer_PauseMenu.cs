using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_PauseMenu : Layer_Default<UI_PauseMenu> {
        protected override void Start() {
            base.Start();
            Event.Global_EventSystem.UI.Register(UIEventID.World_정지메뉴UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_정지메뉴UIClose, Close);
            Event.Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
        }

        protected override void Open()
        {
            base.Open();
            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(false);
        }

        protected override void Close()
        {
            base.Close();
            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
        }

        private void OnSceneReloaded()
        {
            base.Close();
        }

        private void OnDestroy()
        {
            Event.Global_EventSystem.VisualNovel.onSceneReloaded -= OnSceneReloaded;
        }
    }
}
