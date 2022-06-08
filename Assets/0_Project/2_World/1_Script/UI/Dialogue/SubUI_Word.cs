using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TP.UI {

    [Flags]
    public enum TPTextStyleAttribute {
        Normal = 0,
        Bold = 1 << 1,              // º¼µåÃ¼
        Italic = 1 << 2,            // ÀÌÅÅ¸¯Ã¼
        Underline = 1 << 3,         // ¾Æ·¡¼±
        StrikeThrough = 1 << 4,     // Ãë¼Ò¼±
        SuperScript = 1 << 5,       // À§Ã·ÀÚ
        SubScript = 1 << 6,         // ¾Æ·¡Ã·ÀÚ
        Color = 1 << 7,             // »ö±ò ÀÖÀ½
        Big = 1 << 8,               // Å« ±Û¾¾
        Small = 1 << 9,             // ÀÛÀº ±Û¾¾
        Keyword = 1 << 10,           // Å°¿öµå ±Û¾¾ (²Ú ´©¸£¸é ¹¹¶óµµ µÇ°Ô)
    }

    public struct TPTextStyle {
        public TPTextStyleAttribute attribute;
        public Color color;
        public string keyword;
    }

    public class SubUI_Word : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public static int FONTSIZE_SMALL { get; private set; }
        public static int FONTSIZE_NORMAL { get; private set; }
        public static int FONTSIZE_BIG { get; private set; }
        public static void SetFontSizeData(int value) {
            FONTSIZE_BIG = value;
            FONTSIZE_NORMAL = (int)(value * .8f);
            FONTSIZE_SMALL = (int)(value * .6f);
        }
        [SerializeField]
        private RectTransform m_RectTransform;
        public RectTransform RectTransform { get => m_RectTransform; }
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private TextMeshProUGUI m_Text;
        public string Text { get => m_Text.text; }
        public TPTextStyle Style { get; private set; }
        public float PreferredWidth { get => m_Text.preferredWidth; }
        public float PreferredHeight { get => m_Text.preferredHeight; }
        public SubUI_Word SetText(string value) {
            m_Text.SetText(value);
            return this;
        }
        public SubUI_Word SetActive(bool state) {
            gameObject.SetActive(state);
            return this;
        }
        public SubUI_Word SetStyle(TPTextStyle style) {
            Style = style;
            m_Text.fontSize = FONTSIZE_NORMAL;
            m_Text.fontStyle = FontStyles.Normal;
            if (style.attribute.HasFlag(TPTextStyleAttribute.Bold)) {
                m_Text.fontStyle |= FontStyles.Bold;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Italic)) {
                m_Text.fontStyle |= FontStyles.Italic;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Underline)) {
                m_Text.fontStyle |= FontStyles.Underline;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.StrikeThrough)) {
                m_Text.fontStyle |= FontStyles.Strikethrough;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.SubScript)) {
                SetText($"<sub>{Text}</sub>");
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.SuperScript)) {
                SetText($"<sup>{Text}</sup>");
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Color)) {
                m_Text.color = style.color;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Big)) {
                m_Text.fontSize = FONTSIZE_BIG;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Small)) {
                m_Text.fontSize = FONTSIZE_SMALL;
            }
            if (style.attribute.HasFlag(TPTextStyleAttribute.Keyword)) {
                m_Text.color = Color.cyan;
                m_Text.fontStyle |= FontStyles.Underline;
                m_Image.raycastTarget = true;
            }
            return this;
        }
        public SubUI_Word SetFontSize(int value) {
            m_Text.fontSize = value;
            return this;
        }
        public SubUI_Word SetShape(float pos, float size) {
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos, size);
            return this;
        }
        public SubUI_Word SetCharacterCount(int value) {
            m_Text.maxVisibleCharacters = value;
            m_Text.ForceMeshUpdate();
            return this;
        }
        public void OnPointerDown(PointerEventData eventData) {
            if (Style.attribute.HasFlag(TPTextStyleAttribute.Keyword)) {
                Event.Global_EventSystem.UI.Call<string, Vector2>(UIEventID.Global_ÅøÆÁUIOpen, Style.keyword, eventData.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (Style.attribute.HasFlag(TPTextStyleAttribute.Keyword)) {
                Event.Global_EventSystem.UI.Call(UIEventID.Global_ÅøÆÁUIClose);
            }
        }
    }
}

