using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace TP.UI {
    public class UI_Tooltip : UI_Default {

        [Header("- Tooltip")]
        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        [SerializeField]
        private RectTransform m_RectTransform;
        
        [SerializeField]
        private TextMeshProUGUI m_Text;

        protected override void Start() {
            showSequence = DOTween.Sequence();
            showSequence.
                Append(m_CanvasGroup.DOFade(1, 0.25f));

            hideSequence = DOTween.Sequence();
            hideSequence.
                OnStart(() => m_CanvasGroup.alpha = 1).
                Append(m_CanvasGroup.DOFade(0, 0.25f));
            m_CanvasGroup.alpha = 0;
            base.Start();
        }

        public void SetTooltip(string data, Vector2 position) {
            m_Text.SetText($"ÅøÆÁ : {data}");
            RectTransform rectTransform = m_Text.rectTransform;

            Vector2 sizeDelta = new Vector2(
                m_Text.preferredWidth + rectTransform.offsetMin.x - rectTransform.offsetMax.x,
                m_Text.preferredHeight + rectTransform.offsetMin.y - rectTransform.offsetMax.y);

            m_RectTransform.sizeDelta = new Vector2(
                Mathf.Max(m_RectTransform.sizeDelta.x, sizeDelta.x),
                Mathf.Max(m_RectTransform.sizeDelta.y, sizeDelta.y));

            m_RectTransform.anchoredPosition = Global_Canvas.ScreenToLocalPosition(position);
        }
    }
}
