using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TP.UI {
    public class SubUI_LogWord : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [SerializeField]
        private RectTransform m_RectTransform;
        public RectTransform RectTransform { get => m_RectTransform; }
        [SerializeField]
        private TextMeshProUGUI m_Text;
        [SerializeField]
        private Image m_Image;
        public float PreferredWidth { get => m_Text.preferredWidth; }
        public float PreferredHeight { get => m_Text.preferredHeight; }

        private string keyword = string.Empty;

        public void Init(string data) {
            Init(data, string.Empty);
        }

        public void Init(string data, string keywordData) {
            m_Text.SetText(data);
            if (string.IsNullOrEmpty(keywordData) == false) {
                m_Text.color = Color.cyan;
                m_Text.fontStyle |= FontStyles.Underline;
                m_Image.raycastTarget = true;
                keyword = keywordData;
            }
        }

        public void SetText(string data) {
            m_Text.SetText(data);
        }

        public void SetShape(Vector2 pos) {
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, PreferredWidth);
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos.y, PreferredHeight);
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (string.IsNullOrEmpty(keyword) == false) {
                Event.Global_EventSystem.UI.Call<string, Vector2>(UIEventID.Global_ÅøÆÁUIOpen, keyword, eventData.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (string.IsNullOrEmpty(keyword) == false) {
                Event.Global_EventSystem.UI.Call(UIEventID.Global_ÅøÆÁUIClose);
            }
        }
    }
}


