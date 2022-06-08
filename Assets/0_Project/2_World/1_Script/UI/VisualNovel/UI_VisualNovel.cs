using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TP.UI {
    public class UI_VisualNovel : UI_Default {
        [Header("- Visual Novel")]
        [SerializeField]
        private Image image_Background;
        [SerializeField]
        private Image image_Filter;
        [SerializeField]
        private Image[] image_Character;

        protected override void Start() {
            SetFilter(image_Filter.sprite);
            SetBackground(image_Background.sprite);
            for (int i = 0; i < image_Character.Length; ++i) {
                SetSprite(i, image_Character[i].sprite);
            }
        }

        private void SortSprite() {
            List<int> enableIndex = new List<int>();
            for (int i = 0; i < image_Character.Length; ++i) {
                if (image_Character[i].gameObject.activeSelf) enableIndex.Add(i);
            }
            for (int i = 0; i < enableIndex.Count; ++i) {
                if (enableIndex.Count == 1) {
                    image_Character[i].rectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        public void SetSprite(int index, Sprite sprite) {
            if (0 <= index && index < image_Character.Length) {
                if(sprite == null) {
                    image_Character[index].sprite = null;
                    image_Character[index].gameObject.SetActive(false);
                }
                else {
                    image_Character[index].sprite = sprite;
                    image_Character[index].gameObject.SetActive(true);
                }
            }
        }

        public void SetFilter(Sprite sprite) {
            if (sprite == null) {
                image_Filter.sprite = null;
                image_Filter.gameObject.SetActive(false);
            }
            else {
                image_Filter.sprite = sprite;
                image_Filter.gameObject.SetActive(true);
            }
        }

        public void SetBackground(Sprite sprite) {
            if (sprite == null) {
                image_Background.sprite = null;
                image_Background.gameObject.SetActive(false);
            }
            else {
                image_Background.sprite = sprite;
                image_Background.gameObject.SetActive(true);
            }
        }

    }
}
