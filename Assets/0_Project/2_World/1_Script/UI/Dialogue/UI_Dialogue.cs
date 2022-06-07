using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using System.Linq;
using DG.Tweening;

namespace TP.UI {
    public class UI_Dialogue : UI_Default {
        [Header("- Dialogue")]
        [SerializeField]
        private Button button_Touch;
        [SerializeField]
        private TextMeshProUGUI text_Name;
        [SerializeField]
        private GameObject object_Name;
        [SerializeField]
        private TextMeshProUGUI text_Position;
        [SerializeField]
        private GameObject object_Position;
        [Space(20)]
        [SerializeField]
        private RectTransform area_Text;
        [SerializeField]
        private RectTransform anchor_Text;
        [SerializeField]
        private int m_lineCount = 3;
        [Header("SubUI")]
        [SerializeField]
        private SubUI_Word subUI_Word;
        [SerializeField]
        private SubUI_Line subUI_Line;
        [SerializeField]
        private SubUI_Cursor subUI_Cursor;

        private SubUI_Word m_calcWord;

        private Stack<SubUI_Line> m_lineStack;

        private int m_lineWidth = 0;
        private int m_lineHeight = 0;

        private bool isTouch = false;
        private bool isTyping = false;

        protected override void Start() {
            m_lineWidth = (int)area_Text.rect.width;
            m_lineHeight = (int)(area_Text.rect.height / m_lineCount);

            m_lineStack = new Stack<SubUI_Line>();

            SubUI_Line m_calcLine =
                Instantiate(subUI_Line, area_Text).SetShape(0, m_lineHeight);
            m_calcLine.CanvasGroup.interactable = false;
            m_calcLine.CanvasGroup.blocksRaycasts = false;
            m_calcLine.CanvasGroup.alpha = 0;

            m_calcWord =
                Instantiate(subUI_Word, m_calcLine.RectTransform);

            float topInsert = m_calcWord.RectTransform.offsetMax.y;
            int height = (int)(m_lineHeight + topInsert);
            m_calcWord.SetText("��<sup>��</sup>");
            for (int i = height; i >= 0; --i) {
                m_calcWord.SetFontSize(i);
                if (m_calcWord.PreferredHeight <= height) {
                    SubUI_Word.SetFontSizeData(i - 1);
                    break;
                }
            }
            m_calcWord.SetText(string.Empty);

            button_Touch.onClick.AddListener(OnClick_Touch);
        }

        private void OnClick_Touch() {
            if (isTyping) {
                isTouch = true;
            }
        }

        public void SetText_Name(string value) {
            object_Name.SetActive(!string.IsNullOrEmpty(value));
            text_Name.SetText(value);
        }

        public void SetText_Position(string value) {
            object_Position.SetActive(!string.IsNullOrEmpty(value));
            text_Name.SetText(value);
        }

        public void SetText_Dialogue(string value, UnityAction callback) {
            isTyping = true;

            anchor_Text.anchoredPosition = new Vector2(0, 0);   // ��Ŀ �ʱ�ȭ
            for(int i = anchor_Text.childCount - 1; i >= 0; ++i) {
                Destroy(anchor_Text.GetChild(i).gameObject);
            }
            m_lineStack.Clear();

            TPTextStyle style = default;
            Stack<TPTextStyle> styleStack = new Stack<TPTextStyle>();
            Stack<Color> colorStack = new Stack<Color>();
            Stack<string> keywordStack = new Stack<string>();
            int lineCount = 1;
            string content = string.Empty;

            for (int i = 0; i < value.Length; ++i) {
                if (value[i].Equals('<')) {
                    string tagData = "<";
                    for (int j = i + 1; j < value.Length; ++j) {
                        if (value[j].Equals('<')) {
                            AddContent(tagData);
                            tagData = string.Empty;
                        }
                        tagData += value[j];
                        if (value[j].Equals('>')) break;
                    }
                    string tag = tagData.TrimStart('<').TrimEnd('>').Replace(" ", "");
                    string tagContent = string.Empty;
                    bool isEndTag = tag.Contains("/");
                    if (isEndTag == false && tag.Contains("=")) {
                        string[] data = tag.Split('=');
                        tag = data[0];
                        tagContent = data[1];
                    }
                    switch (tag.TrimStart('/').ToUpper()) {
                        case "B":
                        case "STRONG":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.Bold;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Bold;
                            }
                            break;
                        case "I":
                        case "EM":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.Italic;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Italic;
                            }
                            break;
                        case "U":
                        case "UNDERLINE":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.Underline;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Underline;
                            }
                            break;
                        case "S":
                        case "STRIKE":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.StrikeThrough;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.StrikeThrough;
                            }
                            break;
                        case "BIG":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.Big;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Big;
                            }
                            break;
                        case "SMALL":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.Small;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Small;
                            }
                            break;
                        case "SUB":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.SubScript;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.SubScript;
                            }
                            break;
                        case "SUP":
                            if (isEndTag) {
                                style.attribute &= ~TPTextStyleAttribute.SuperScript;
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.SuperScript;
                            }
                            break;
                        case "COLOR":
                            if (isEndTag) {
                                if (colorStack.Count > 0) {
                                    colorStack.Pop();
                                    if (colorStack.Count == 0) {
                                        style.attribute &= ~TPTextStyleAttribute.Color;
                                        style.color = default;
                                    }
                                    else {
                                        style.color = colorStack.Peek();
                                    }
                                }
                                else {
                                    AddContent(tagData);
                                    continue;
                                }
                            }
                            else {
                                if (ColorUtility.TryParseHtmlString(tagContent, out Color color)) {
                                    style.attribute |= TPTextStyleAttribute.Color;
                                    style.color = color;
                                    colorStack.Push(color);
                                }
                            }
                            break;
                        case "KEYWORD":
                            if (isEndTag) {
                                if (keywordStack.Count > 0) {
                                    keywordStack.Pop();
                                    if (keywordStack.Count == 0) {
                                        style.attribute &= ~TPTextStyleAttribute.Keyword;
                                        style.keyword = string.Empty;
                                    }
                                    else {
                                        style.keyword = keywordStack.Peek();
                                    }
                                }
                                else {
                                    AddContent(tagData);
                                    continue;
                                }
                            }
                            else {
                                style.attribute |= TPTextStyleAttribute.Keyword;
                                style.keyword = tagContent;
                                keywordStack.Push(tagContent);
                            }
                            break;
                        default:
                            AddContent(tagData);
                            continue;
                    }
                    MakeWord(content, GetStyle());
                    styleStack.Push(style);

                    i += tagData.Length - 1;
                }
                else if (value[i].Equals('&')) {
                    string charData = "&";
                    for (int j = i + 1; j < value.Length; ++j) {
                        if (value[j].Equals('&')) {
                            AddContent(charData);
                            charData = string.Empty;
                        }
                        charData += value[j];
                        if (value[j].Equals(';')) break;
                    }
                    switch (charData.TrimStart('&').TrimEnd(';').ToUpper()) {
                        case "LT":
                            content += '<';
                            break;
                        case "RT":
                            content += '>';
                            break;
                        case "AMP":
                            content += '&';
                            break;
                        default:
                            AddContent(charData);
                            continue;
                    }
                    i += charData.Length;
                }
                else if (value[i].Equals(' ')) {
                    content += ' ';
                    string charData = string.Empty;
                    for (int j = i + 1; j < value.Length; ++j) {
                        if (value[j].Equals(' ') == false) {
                            break;
                        }
                        charData += " ";
                    }
                    if (string.IsNullOrEmpty(charData) == false) {
                        AddContent(charData);
                    }
                    content += "<color=#00000000>.</color>";
                    MakeWord(content, GetStyle());
                }
                else if (value[i].Equals('\\')) {
                    if (i + 1 < value.Length &&
                        (value[i + 1].Equals('n') || value[i + 1].Equals('N'))) {
                        i++;
                        MakeWord(content, GetStyle());
                        lineCount++;
                    }
                }
                else {
                    content += value[i];
                }

                if (i == value.Length - 1 && string.IsNullOrEmpty(content) == false) {
                    MakeWord(content, GetStyle());
                }

                

                void MakeWord(string data, TPTextStyle style) {
                    string wordData = data;
                    TPTextStyle wordStyle = style;
                    m_calcWord.SetText(wordData).SetStyle(wordStyle);
                    SubUI_Line line = GetLine();
                    while (line.Width + m_calcWord.PreferredWidth >= m_lineWidth) {
                        if (line.Count <= 0) {
                            // �� �ٿ� �ܾ �ϳ��� ���µ� �����ؾ� �� ���, ���̸� �缭 �߶� �� �ٿ� �߰���Ű��, �����Ѵ�
                            string calcStr = string.Empty;
                            string wordStr = string.Empty;
                            string leftStr = string.Empty;

                            for (int sIndex = 0; sIndex < wordData.Length; ++sIndex) {
                                calcStr += wordData[sIndex];
                                m_calcWord.SetText(calcStr);
                                if (m_calcWord.PreferredWidth >= m_lineWidth) {
                                    wordStr = wordData.Substring(0, sIndex - 1);
                                    leftStr = wordData.Substring(sIndex);
                                    break;
                                }
                            }

                            SubUI_Word slicedWord =
                                    Instantiate(subUI_Word, line.RectTransform).
                                            SetText(string.IsNullOrEmpty(wordStr) ? wordData : wordStr).
                                            SetStyle(wordStyle);
                            slicedWord.SetShape(0, slicedWord.PreferredWidth);
                            line.AddWord(slicedWord);

                            wordData = leftStr;
                            m_calcWord.SetText(wordData);

                            if (string.IsNullOrEmpty(wordData) == false) {
                                lineCount++;
                                line = GetLine();
                            }
                        }
                        else {
                            // �ܾ �� �� �̻� �־ �׳� �����ص� �� ���, ī���͸� �ϳ� ������Ų��
                            lineCount++;
                            line = GetLine();
                        }
                    }

                    if (string.IsNullOrEmpty(wordData) == false) {
                        SubUI_Word newWord =
                                        Instantiate(subUI_Word, line.RectTransform).
                                                SetText(wordData).
                                                SetStyle(wordStyle);
                        newWord.SetShape(line.Width, newWord.PreferredWidth);
                        line.AddWord(newWord);
                    }
                    content = string.Empty;
                }

                SubUI_Line GetLine() {
                    for (int i = lineCount; i > m_lineStack.Count; i--) {
                        SubUI_Line newLine = Instantiate(subUI_Line, anchor_Text).SetShape(m_lineStack.Count * m_lineHeight, m_lineHeight);
                        newLine.CanvasGroup.alpha = 0;
                        m_lineStack.Push(newLine);
                    }
                    return m_lineStack.Peek();
                }

                TPTextStyle GetStyle() {
                    if (styleStack.Count > 0) {
                        return styleStack.Peek();
                    }
                    return default;
                }

                void AddContent(string value) {
                    content += value;
                    i += value.Length;
                }

            }

            StartCoroutine(TypewriteEffect(m_lineStack.Reverse().ToList()));

            IEnumerator TypewriteEffect(List<SubUI_Line> lineData) {
                bool isComplete = false;
                Tweener lineEffect = null;
                for (int i = 0; i < lineData.Count; ++i) {
                    if (isTouch == false) {
                        if (i >= m_lineCount) {
                            lineEffect = anchor_Text.
                                DOAnchorPosY(m_lineHeight * ((i - m_lineCount) + 1), 0.5f).
                                OnComplete(() => isComplete = true).
                                Play();
                        }
                        else isComplete = true;
                    }
                    if (isTouch == false) yield return new WaitUntil(() => isComplete || isTouch);
                    if (isTouch &&
                        lineEffect != null &&
                        lineEffect.IsPlaying()) {
                        lineEffect.Complete();
                    }
                    isComplete = false;
                    lineData[i].TypewriterEffect(() => isComplete = true);
                    if (isTouch == false) yield return new WaitUntil(() => isComplete || isTouch);
                    if (isTouch) {
                        lineData[i].Skip();
                    }
                    isComplete = false;
                }
                if (lineData.Count > m_lineCount) {
                    anchor_Text.anchoredPosition = new Vector2(anchor_Text.anchoredPosition.x, m_lineHeight * (lineData.Count - m_lineCount));
                }

                Instantiate(subUI_Cursor, anchor_Text).
                    SetHeight(m_lineHeight).
                    SetPosition(new Vector2(lineData.Last().Width, -(lineData.Count - 1) * m_lineHeight)).
                    Play();

                isTouch = false;
                isTyping = false;
                callback?.Invoke();
            }
        }
    }
}

