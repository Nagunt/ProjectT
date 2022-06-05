using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {
    public class UI_SaveLoad : UI_Default {

        [Header("- SaveLoad")]
        [SerializeField]
        private GameObject m_SaveUITag;
        [SerializeField]
        private GameObject m_LoadUITag;
        [SerializeField]
        private SubUI_Slot[] m_Slots;

        public void SetUIMode(bool isSave) {
            m_SaveUITag.SetActive(isSave);
            m_LoadUITag.SetActive(!isSave);
            for(int i = 0; i < m_Slots.Length; ++i) {
                m_Slots[i].Init(isSave);
            }
        }
    }
}


