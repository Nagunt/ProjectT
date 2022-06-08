using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI
{
    public class UI_Effect : UI_Default
    {
        [Header("- Effect")]
        [SerializeField]
        private GameObject subUI_Dark;
        [SerializeField]
        private SubUI_TitleEffect subUI_TitleEffect;

        public void SetDark()
        {
            subUI_Dark.SetActive(true);
        }

        public void RemoveDark()
        {
            subUI_Dark.SetActive(false);
        }

        public void MakeTitleEffect(string data, UnityAction callback, bool isDark, bool isSkip)
        {
            Instantiate(subUI_TitleEffect, transform).MakeTitle(data, callback, isDark, isSkip);
        }
    }
}

