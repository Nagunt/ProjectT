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
        [SerializeField]
        private GameObject object_Dialogue;
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

        private bool isSkip = false;
        private bool isTouch = false;
        private bool isTyping = false;
        private bool isComplete = false;

        private RectTransform m_nameTransform;
        private RectTransform m_positionTransform;
        private RectTransform m_dialogueTransform;

        private Sequence shakeSequence = null;

        private SubUI_Cursor cursor;

        protected override void Start() {
            m_lineWidth = (int)area_Text.rect.width;
            m_lineHeight = (int)(area_Text.rect.height / m_lineCount);

            m_lineStack = new Stack<SubUI_Line>();

            m_nameTransform = object_Name.GetComponent<RectTransform>();
            m_positionTransform = object_Position.GetComponent<RectTransform>();
            m_dialogueTransform = object_Dialogue.GetComponent<RectTransform>();

            SubUI_Line m_calcLine =
                Instantiate(subUI_Line, area_Text).SetShape(0, m_lineHeight);
            m_calcLine.CanvasGroup.interactable = false;
            m_calcLine.CanvasGroup.blocksRaycasts = false;
            m_calcLine.CanvasGroup.alpha = 0;

            m_calcWord =
                Instantiate(subUI_Word, m_calcLine.RectTransform);

            float topInsert = m_calcWord.RectTransform.offsetMax.y;
            int height = (int)(m_lineHeight + topInsert);
            m_calcWord.SetText("뭶<sup>뭶</sup>");
            for (int i = height; i >= 0; --i) {
                m_calcWord.SetFontSize(i);
                if (m_calcWord.PreferredHeight <= height) {
                    SubUI_Word.SetFontSizeData(i - 1);
                    break;
                }
            }
            m_calcWord.SetText(string.Empty);

            button_Touch.onClick.AddListener(OnClick_Touch);

            Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
        }

        private void OnClick_Touch() {
            if (isTyping) {
                isTouch = true;
            }
            else {
                Event.Global_EventSystem.VisualNovel.CallOnScreenTouched();
            }
        }

        public void SetText_Name(string value) {
            object_Name.SetActive(!string.IsNullOrEmpty(value));
            text_Name.SetText(value);
        }

        public void SetText_Position(string value) {
            object_Position.SetActive(!string.IsNullOrEmpty(value));
            text_Position.SetText(value);
        }

        public void SetText_Dialogue(string value, bool isClear, bool isRefresh, UnityAction callback) {
            TPTextStyle style = default;
            Stack<TPTextStyle> styleStack = new Stack<TPTextStyle>();
            Stack<Color> colorStack = new Stack<Color>();
            Stack<string> keywordStack = new Stack<string>();
            int prevLineCount = 0;
            int lineCount = 1;
            string content = string.Empty;

            if (cursor != null) {
                Destroy(cursor.gameObject);
            }

            if (isClear) {
                anchor_Text.anchoredPosition = new Vector2(0, 0);   // 앵커 초기화
                for (int i = anchor_Text.childCount - 1; i >= 0; --i) {
                    Destroy(anchor_Text.GetChild(i).gameObject);
                }
                m_lineStack.Clear();
            }
            else {
                if (m_lineStack.Count > 0) {
                    prevLineCount = m_lineStack.Count - 1;
                    SubUI_Line lastLine = m_lineStack.Peek();
                    m_lineStack.Clear();
                    SubUI_Line newLine = GetLine();
                    newLine.AddSpace(lastLine.Width);
                }
            }

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

                void MakeWord(string data, TPTextStyle wordStyle) {
                    string wordData = data;
                    m_calcWord.SetText(wordData).SetStyle(wordStyle);
                    SubUI_Line line = GetLine();
                    while (line.Width + m_calcWord.PreferredWidth >= m_lineWidth) {
                        if (line.Count <= 0) {
                            // 이 줄에 단어가 하나도 없는데 개행해야 할 경우, 길이를 재서 잘라서 이 줄에 추가시키고, 개행한다
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
                            slicedWord.SetShape(line.Width, slicedWord.PreferredWidth);
                            line.AddWord(slicedWord);

                            wordData = leftStr;
                            m_calcWord.SetText(wordData);

                            if (string.IsNullOrEmpty(wordData) == false) {
                                lineCount++;
                                line = GetLine();
                            }
                        }
                        else {
                            // 단어가 한 개 이상 있어서 그냥 개행해도 될 경우, 카운터를 하나 증가시킨다
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

                void AddContent(string data) {
                    content += data;
                    i += data.Length;
                }

            }

            StartCoroutine(EffectRoutine(m_lineStack.Reverse().ToList()));

            SubUI_Line GetLine() {
                for (int i = lineCount; i > m_lineStack.Count; i--) {
                    SubUI_Line newLine = Instantiate(subUI_Line, anchor_Text).SetShape((m_lineStack.Count + prevLineCount) * m_lineHeight, m_lineHeight);
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

            IEnumerator EffectRoutine(List<SubUI_Line> lineData) {
                if (isSkip == false &&
                    isRefresh == false) {
                    isTyping = true;
                    isComplete = false;
                    Tweener lineEffect = null;
                    for (int i = 0; i < lineData.Count; ++i) {
                        if (isTouch == false) {
                            if (i + prevLineCount >= m_lineCount) {
                                lineEffect = anchor_Text.
                                    DOAnchorPosY(m_lineHeight * ((i - m_lineCount) + 1 + prevLineCount), 0.5f).
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
                    isTouch = false;
                    isTyping = false;
                }
                else {
                    for(int i = 0; i < lineData.Count; ++i) {
                        lineData[i].Show();
                    }
                }
                if (lineData.Count + prevLineCount > m_lineCount) {
                    anchor_Text.anchoredPosition = new Vector2(anchor_Text.anchoredPosition.x, m_lineHeight * (lineData.Count - m_lineCount + prevLineCount));
                }

                if (isRefresh == false)
                {
                    cursor = Instantiate(subUI_Cursor, anchor_Text).
                        SetHeight(m_lineHeight).
                        SetPosition(new Vector2(lineData.Last().Width, -(lineData.Count + prevLineCount - 1) * m_lineHeight)).
                        Play();
                }
                callback?.Invoke();
            }
        }

        public void ClearText()
        {
            StopAllCoroutines();
            SetText_Name(string.Empty);
            SetText_Position(string.Empty);
            if (cursor != null)
            {
                Destroy(cursor.gameObject);
            }
            anchor_Text.anchoredPosition = new Vector2(0, 0);   // 앵커 초기화
            for (int i = anchor_Text.childCount - 1; i >= 0; --i)
            {
                Destroy(anchor_Text.GetChild(i).gameObject);
            }
            m_lineStack.Clear();
        }

        public void Effect_Shake(float time) {
            Vector2 nameFirstPos = m_nameTransform.anchoredPosition;
            Vector2 positionFirstPos = m_positionTransform.anchoredPosition;
            Vector2 dialogueFirstPos = m_dialogueTransform.anchoredPosition;

            Vector2 nameLastPos = Vector2.zero;
            Vector2 positionLastPos = Vector2.zero;
            Vector2 dialogueLastPos = Vector2.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.
                OnStart(() => {
                    Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
                    Event.Global_EventSystem.VisualNovel.onGameStateChanged += OnGameStateChanged;
                    if (shakeSequence.IsActive()) {
                        shakeSequence.Kill(true);
                    }
                    shakeSequence = sequence;
                });

            sequence.Append(m_nameTransform.DOShakeAnchorPos(time));
            sequence.Join(m_positionTransform.DOShakeAnchorPos(time));
            sequence.Join(m_dialogueTransform.DOShakeAnchorPos(time));

            sequence.
                OnComplete(() => {
                    m_nameTransform.anchoredPosition = nameFirstPos;
                    m_positionTransform.anchoredPosition = positionFirstPos;
                    m_dialogueTransform.anchoredPosition = dialogueFirstPos;
                }).
                OnKill(() => {
                    Event.Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
                    Event.Global_EventSystem.VisualNovel.onGameStateChanged -= OnGameStateChanged;
                    shakeSequence = null;
                }).
                Play();

            void OnSkipStateChanged(bool state) {
                if (state) {
                    shakeSequence.Complete();
                }
            }

            void OnGameStateChanged(bool state) {
                if (state) {
                    m_nameTransform.anchoredPosition = nameLastPos;
                    m_positionTransform.anchoredPosition = positionLastPos;
                    m_dialogueTransform.anchoredPosition = dialogueLastPos;
                    shakeSequence.Play();
                }
                else {
                    nameLastPos = m_nameTransform.anchoredPosition;
                    positionLastPos = m_positionTransform.anchoredPosition;
                    dialogueLastPos = m_dialogueTransform.anchoredPosition;
                    m_nameTransform.anchoredPosition = nameFirstPos;
                    m_positionTransform.anchoredPosition = positionFirstPos;
                    m_dialogueTransform.anchoredPosition = dialogueFirstPos;
                    shakeSequence.Pause();
                }
            }
        }

        private void OnSkipStateChanged(bool state) {
            isSkip = state;
            if (state && isTyping) {
                isTouch = true;
            }
        }

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
        }
    }
}


