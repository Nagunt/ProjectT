using System.Collections;
using System.Collections.Generic;
using TP.Data;
using UnityEngine;

namespace TP.UI
{

    public class UI_Selection : UI_Default
    {
        [SerializeField]
        private SubUI_Selection subUI_Selection;

        public void MakeSelection(params TPSelectionData[] data)
        {
            float interval = 150f;
            float firstY = (interval * .5f) * (data.Length - 1);
            for(int i = 0; i < data.Length; ++i)
            {
                SubUI_Selection newSelection = Instantiate(subUI_Selection, transform);
                newSelection.Init(data[i]);
                newSelection.onSelect += () => Event.Global_EventSystem.UI.Call(UIEventID.World_선택지UI선택완료);
                newSelection.RectTransform.anchoredPosition = new Vector2(
                    newSelection.RectTransform.anchoredPosition.x,
                    firstY - interval * i);
            }
        }
    }
}
