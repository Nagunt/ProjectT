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
            Event.Global_EventSystem.UI.Register<TPSelectionData[]>(UIEventID.World_������UI����, MakeSelection);
            Event.Global_EventSystem.UI.Register(UIEventID.World_������UI���ÿϷ�, Close);
        }


        private void MakeSelection(params TPSelectionData[] data)
        {
            base.Open();
            if(_uiObject != null)
            {
                _uiObject.MakeSelection(data);
            }
        }
    }


}


