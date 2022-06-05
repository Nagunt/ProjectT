using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TP.UI {
    public class SubUI_Cursor : MonoBehaviour {
        [SerializeField]
        private RectTransform m_RectTransform;
        [SerializeField]
        private Image m_image;

        public SubUI_Cursor SetHeight(float height) {
            m_RectTransform.sizeDelta = new Vector2(m_RectTransform.sizeDelta.x, height);
            return this;
        }
        public SubUI_Cursor SetPosition(Vector2 position) {
            m_RectTransform.anchoredPosition = position;
            return this;
        }
        public SubUI_Cursor Play() {
            m_image.color = new Color(1, 1, 1, 0);
            Sequence sequence = DOTween.Sequence();
            sequence.
                SetLink(gameObject, LinkBehaviour.KillOnDestroy).
                Append(m_image.DOFade(1, 0.5f)).
                Append(m_image.DOFade(0, 0.5f)).
                SetLoops(-1);
            sequence.Play();
            return this;
        }
    }
}

