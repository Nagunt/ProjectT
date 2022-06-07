using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {

    public class UI_Book : UI_Default {
        [Header("- Book")]
        [SerializeField]
        private Image image_Picture;
        [SerializeField]
        private Image image_Rank;
        [SerializeField]
        private TextMeshProUGUI text_Where;
        [SerializeField]
        private TextMeshProUGUI text_Name;
        [SerializeField]
        private TextMeshProUGUI text_Birth;
        [SerializeField]
        private TextMeshProUGUI text_Placement;
        [SerializeField]
        private Button[] button_Slot;

        private Vector2 aspectRatio = new Vector2(7, 9);

        protected override void Start() {
            image_Picture.GetComponent<RectTransform>().SetShapeWithCurrentAspectRatio(aspectRatio);
            image_Rank.GetComponent<RectTransform>().SetShapeWithCurrentAspectRatio(aspectRatio);
            for(int i = 0; i < button_Slot.Length; ++i) {
                button_Slot[i].GetComponent<RectTransform>().SetShapeWithCurrentAspectRatio(aspectRatio);
                button_Slot[i].onClick.AddListener(() => { OnClick_Slot(i); });
            }
        }

        private void OnClick_Slot(int index) {
            Debug.Log(index);
        }
    }
}

