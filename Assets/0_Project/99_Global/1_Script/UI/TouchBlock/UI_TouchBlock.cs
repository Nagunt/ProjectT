using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {

    public class UI_TouchBlock : UI_Default {

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        public void Lock() {
            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.interactable = true;
        }

        public void Unlock() {
            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.interactable = false;
        }
    }
}

