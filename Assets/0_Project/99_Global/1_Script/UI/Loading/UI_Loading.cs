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
            float data = Mathf.Clamp(value, 0, 1f);
            text_Progress.SetText($"{data * 100}%");
            image_Progress.fillAmount = data;
        }
    }
}
