using UnityEngine;

namespace TP {
    public static class Global_ExtensionMethod {

        /// <summary>
        /// �θ� ���� �������� �ش� ������ ũ�⸦ ����ϴ�. �θ� ���� ��� �������� �ʽ��ϴ�.
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
        /// area ���� �������� �ش� ������ ũ�⸦ ����ϴ�.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area"></param>
        /// <param name="aspectRatio"></param>
        public static void SetShapeWithCurrentAspectRatio(this RectTransform target, RectTransform area, Vector2 aspectRatio) {
            Vector2 pivot = target.pivot;

            float width = area.rect.size.x;
            float height = area.rect.size.y;
            Debug.Log($"{area.name} : width = {width}, height = {height}");
            float targetWidth = width * aspectRatio.y > height * aspectRatio.x ? height * (aspectRatio.x / aspectRatio.y) : width;
            float targetHeight = height * aspectRatio.x > width * aspectRatio.y ? width * (aspectRatio.y / aspectRatio.x) : height;

            if (pivot.x != 0.5f) {
                target.SetInsetAndSizeFromParentEdge(pivot.x > 0.5f ? RectTransform.Edge.Right : RectTransform.Edge.Left, (pivot.x > 0.5f ? -target.offsetMax.x : target.offsetMin.x), targetWidth);
            }
            else {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
            }
            if (pivot.y != 0.5f) {
                target.SetInsetAndSizeFromParentEdge(pivot.y > 0.5f ? RectTransform.Edge.Top : RectTransform.Edge.Bottom, (pivot.y > 0.5f ? -target.offsetMax.y : target.offsetMin.y), targetHeight);
            }
            else {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            }
        }
    }
}

