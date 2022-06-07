using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {

    public class UI_Loading : UI_Default {
        [Header("- Loading")]
        [SerializeField]
        private TextMeshProUGUI text_Progress;
        [SerializeField]
        private Image image_Progress;

        public void SetProgress(float value) {
            text_Progress.SetText($"{value * 100}%");
            image_Progress.fillAmount = value;
        }
    }
}
