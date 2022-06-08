using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI {
    public class SubUI_Line : MonoBehaviour {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        public CanvasGroup CanvasGroup { get => m_CanvasGroup; }
        [SerializeField]
        private RectTransform m_RectTransform;
        public RectTransform RectTransform { get => m_RectTransform; }
        public float Width { get; private set; }
        public int Count { get => words.Count; }

        private List<SubUI_Word> words = new List<SubUI_Word>();

        private bool isSkip = false;

        public void AddWord(SubUI_Word word) {
            words.Add(word);
            Width += word.PreferredWidth;
        }

        public void AddSpace(float space) {
            Width += space;
        }
        public SubUI_Line SetShape(float pos, float size) {
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos, size);
            return this;
        }
        public SubUI_Line Skip() {
            isSkip = true;
            return this;
        }
        public SubUI_Line Show() {
            CanvasGroup.alpha = 1;
            foreach (SubUI_Word word in words) {
                word.SetActive(true).SetCharacterCount(99999);
            }
            return this;
        }
        public void TypewriterEffect(UnityAction onComplete) {
            StartCoroutine(Routine(onComplete));

            IEnumerator Routine(UnityAction onComplete) {
                foreach (SubUI_Word word in words) {
                    word.SetActive(false).SetCharacterCount(0);
                }
                CanvasGroup.alpha = 1;
                foreach (SubUI_Word word in words) {
                    int count = 0;
                    float timer = 0f;
                    int maxCount = Regex.Replace(word.Text, @"<(.|\n)*?>", string.Empty).Length;
                    word.SetActive(true).SetCharacterCount(0);
                    while (count < maxCount && isSkip == false) {
                        word.SetCharacterCount(++count);
                        while (timer < Data.Global_LocalData.Setting.Delay &&
                            isSkip == false) {
                            timer += Time.deltaTime;
                            yield return null;
                        }
                        timer = 0;
                    }
                    word.SetCharacterCount(99999);
                }
                onComplete?.Invoke();
            }
        }
    }
}
