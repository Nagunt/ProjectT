using System.Collections.Generic;
using TP.Data;
using TP.VisualNovel;
using UnityEngine;
using UnityEngine.UI;

namespace TP {
    public static class Global_ExtensionMethod {

        /// <summary>
        /// 부모 안의 범위에서 해당 비율로 크기를 맞춥니다. 부모가 없을 경우 동작하지 않습니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="aspectRatio"></param>
        public static void SetShapeWithCurrentAspectRatio(this RectTransform target, Vector2 aspectRatio) {
            if (target.parent != null &&
                target.parent.TryGetComponent<RectTransform>(out RectTransform parent)) {
                target.SetShapeWithCurrentAspectRatio(parent, aspectRatio);
            }
        }

        /// <summary>
        /// area 안의 범위에서 해당 비율로 크기를 맞춥니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area"></param>
        /// <param name="aspectRatio"></param>
        public static void SetShapeWithCurrentAspectRatio(this RectTransform target, RectTransform area, Vector2 aspectRatio) {
            Vector2 pivot = target.pivot;

            float width = area.rect.size.x;
            float height = area.rect.size.y;
            float targetWidth = width * aspectRatio.y > height * aspectRatio.x ? height * (aspectRatio.x / aspectRatio.y) : width;
            float targetHeight = height * aspectRatio.x > width * aspectRatio.y ? width * (aspectRatio.y / aspectRatio.x) : height;

            bool isUseOffsetX = target.anchorMin.x != target.anchorMax.x;
            bool isUseOffsetY = target.anchorMin.y != target.anchorMax.y;

            if (pivot.x != 0.5f) {
                target.SetInsetAndSizeFromParentEdge(pivot.x > 0.5f ? RectTransform.Edge.Right : RectTransform.Edge.Left, (pivot.x > 0.5f ? -target.offsetMax.x : target.offsetMin.x), targetWidth);
            }
            else {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    targetWidth +
                    (isUseOffsetX ? target.offsetMax.x - target.offsetMin.x : 0));
            }
            if (pivot.y != 0.5f) {
                target.SetInsetAndSizeFromParentEdge(pivot.y > 0.5f ? RectTransform.Edge.Top : RectTransform.Edge.Bottom, (pivot.y > 0.5f ? -target.offsetMax.y : target.offsetMin.y), targetHeight);
            }
            else {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    targetHeight +
                    (isUseOffsetY ? target.offsetMax.y - target.offsetMin.y : 0));
            }
        }

        public static TPSpriteData ToData(this Image image) {
            Sprite sprite = image.sprite;
            string imageKey = string.Empty;
            foreach (KeyValuePair<BackgroundID, Sprite> kv in BackgroundLoader.Data) {
                if(kv.Value == sprite) {
                    imageKey = $"{kv.Key}";
                }
            }
            if (string.IsNullOrEmpty(imageKey)) {
                foreach (KeyValuePair<SpriteID, Sprite> kv in SpriteLoader.Data) {
                    if (kv.Value == sprite) {
                        imageKey = $"{kv.Key}";
                    }
                }
            }
            return new TPSpriteData() {
                imageKey = imageKey,
                posX = image.rectTransform.anchoredPosition.x,
                posY = image.rectTransform.anchoredPosition.y,
                posZ = image.rectTransform.position.z,
                scaleX = image.rectTransform.localScale.x,
                scaleY = image.rectTransform.localScale.y,
                scaleZ = image.rectTransform.localScale.z,
                r = image.color.r,
                g = image.color.g,
                b = image.color.b,
                a = image.color.a,
                flipX = image.rectTransform.eulerAngles.x != 0,
                flipY = image.rectTransform.eulerAngles.y != 0
            };
        }

        public static void Parse(this Image image, TPSpriteData data) {
            if (System.Enum.TryParse(data.imageKey, out BackgroundID backgroundID) &&
                BackgroundLoader.Data.TryGetValue(backgroundID, out Sprite backgroundData)) {
                image.sprite = backgroundData;
                image.gameObject.SetActive(true);
            }
            else if (System.Enum.TryParse(data.imageKey, out FilterID filterID) &&
                FilterLoader.Data.TryGetValue(filterID, out Sprite filterData)) {
                image.sprite = filterData;
                image.gameObject.SetActive(true);
            }
            else if (System.Enum.TryParse(data.imageKey, out SpriteID spriteID) &&
                SpriteLoader.Data.TryGetValue(spriteID, out Sprite spriteData)) {
                image.sprite = spriteData;
                image.gameObject.SetActive(true);
            }
            else {
                image.sprite = null;
                image.gameObject.SetActive(false);
            }
            image.rectTransform.anchoredPosition = new Vector2(data.posX, data.posY);
            image.rectTransform.localScale = data.Scale;
            image.rectTransform.eulerAngles = new Vector3(
                data.flipX ? 180 : 0,
                data.flipY ? 180 : 0,
                0);
            image.color = data.Color;
        }
    }
}

