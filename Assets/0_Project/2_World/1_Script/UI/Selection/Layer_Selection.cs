using System.Collections;
using System.Collections.Generic;
using TP.Data;
using UnityEngine;

namespace TP.UI
{
    public class Layer_Selection : Layer_Default<UI_Selection>
    {

        protected override void Start()
        {
            base.Start();
            Event.Global_EventSystem.UI.Register<TPSelectionData[]>(UIEventID.World_선택지UI생성, MakeSelection);
            Event.Global_EventSystem.UI.Register(UIEventID.World_선택지UI선택완료, Close);
            Event.Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
        }

        


        private void MakeSelection(params TPSelectionData[] data)
        {
            base.Open();
            if(_uiObject != null)
            {
                _uiObject.MakeSelection(data);
            }
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


