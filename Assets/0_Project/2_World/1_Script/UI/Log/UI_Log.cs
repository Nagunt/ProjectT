using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TP.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {
    public class UI_Log : UI_Default {
        [SerializeField]
        private int m_logCount = 15;
        [SerializeField]
        private ScrollRect m_ScrollRect;
        [SerializeField]
        private RectTransform m_Deco;
        [SerializeField]
        private SubUI_Log subUI_Log;

        protected override void Start() {
            var logData = Global_LocalData.Save.Current.TPLogData;
            m_ScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            foreach (TPLogData currentData in logData) {
                SubUI_Log newLog = Instantiate(subUI_Log, m_ScrollRect.content);
                newLog.Init(currentData.name, currentData.content);
                newLog.RectTransform.anchoredPosition = new Vector2(0, -m_ScrollRect.content.rect.height);
                m_ScrollRect.content.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Vertical,
                    m_ScrollRect.content.rect.height + 
                    newLog.RectTransform.rect.height);
            }

            m_ScrollRect.content.localPosition = new Vector3(
                0,
                Mathf.Max(0, m_ScrollRect.content.rect.height - m_ScrollRect.viewport.rect.height));

            m_Deco.offsetMin = new Vector2(m_Deco.offsetMin.x, Mathf.Max(0, m_ScrollRect.viewport.rect.height - m_ScrollRect.content.rect.height));

            if(m_ScrollRect.viewport.rect.height > m_ScrollRect.content.rect.height) {
                m_ScrollRect.verticalScrollbar.gameObject.SetActive(false);
                m_ScrollRect.verticalScrollbar = null;
            }
        }
    }
}
