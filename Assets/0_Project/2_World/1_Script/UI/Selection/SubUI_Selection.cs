using System.Collections;
using System.Collections.Generic;
using TMPro;
using TP.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TP.UI
{
    public class SubUI_Selection : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_RectTransform;

        public RectTransform RectTransform { get => m_RectTransform; }

        [SerializeField]
        private TextMeshProUGUI text_Data;
        [SerializeField]
        private Button m_Button;

        public UnityAction onSelect;

        public void Init(TPSelectionData data)
        {
            text_Data.SetText(data.data);
            onSelect += data.callback;
            m_Button.onClick.AddListener(OnClick_Selection);
        }

        private void OnClick_Selection()
        {
            Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(new TPLogData(string.Empty, text_Data.text));
            onSelect?.Invoke();
        }
    }
}

