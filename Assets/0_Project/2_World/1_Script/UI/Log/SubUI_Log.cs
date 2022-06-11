using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

namespace TP.UI {
    public class SubUI_Log : MonoBehaviour {
        [SerializeField]
        private RectTransform m_RectTransform;
        public RectTransform RectTransform { get => m_RectTransform; }
        [SerializeField]
        private TextMeshProUGUI m_nameText;
        [SerializeField]
        private RectTransform m_contentArea;
        [SerializeField]
        private SubUI_LogWord subUI_LogWord;
        [SerializeField]
        private int m_LineHeight = 70;

        private void SetText_Name(string data) {
            m_nameText.SetText(data);
        }

        private void SetText_Content(string data) {
            
            float lineWidth = 0;
            int wordCount = 0;
            int lineCount = 1;
            float maxWidth = m_contentArea.rect.width;

            Regex rxHTML = new Regex(@"<(.|\n)*?>");
            Regex rxKeyword = new Regex(@"<\s*?keyword\s*?=\s*?(?<keyword>[a-z|A-Z|0-9|°¡-ÆR]*?)\s*?>");
            Regex rxKeywordSplit = new Regex(@"<\s*?keyword\s*?=\s*?[a-z|A-Z|0-9|°¡-ÆR]*?\s*?>");
            Regex rxEndKeyword = new Regex(@"<\s*?/keyword\s*?>");

            if (rxKeyword.IsMatch(data)) {
                List<string> keywordList = new List<string>();
                Stack<int> indexStack = new Stack<int>();
                keywordList.Add(string.Empty);
                foreach (Match m in rxKeyword.Matches(data)) {
                    keywordList.Add(m.Groups["keyword"].Value);
                }
                string[] splitData = rxKeywordSplit.Split(data);
                for (int i = 0; i < splitData.Length; ++i) {
                    indexStack.Push(i);
                    Debug.Log(splitData[i]);
                    if (indexStack.Peek() > 0 &&
                        rxEndKeyword.IsMatch(splitData[i])) {
                        string[] endSplitData = rxEndKeyword.Split(splitData[i]);

                        for (int j = 0; j < endSplitData.Length; ++j) {
                            Debug.Log(endSplitData[j]);
                            if (j > 0) {
                                if (indexStack.Peek() > 0) {
                                    indexStack.Pop();
                                }
                            }
                            MakeWord(endSplitData[j], keywordList[indexStack.Peek()]);
                        }

                        continue;
                    }
                    MakeWord(splitData[i], keywordList[indexStack.Peek()]);
                }
            }
            else {
                MakeWord(data);
            }
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lineCount * m_LineHeight + m_contentArea.offsetMin.y - m_contentArea.offsetMax.y);

            void MakeWord(string content, string keyword = "") {
                string[] splitData = content.Split(new string[] { "\\n" }, System.StringSplitOptions.None);
                for (int i = 0; i < splitData.Length; ++i) {
                    string wordData = rxHTML.Replace(splitData[i], string.Empty);
                    if (wordData.EndsWith(" ")) {
                        wordData += "<color=#00000000>.</color>";
                    }
                    if (string.IsNullOrEmpty(wordData) == false) {
                        SubUI_LogWord newWord = Instantiate(subUI_LogWord, m_contentArea);
                        newWord.Init(wordData, keyword);
                        newWord.SetShape(new Vector2(lineWidth, (lineCount - 1) * m_LineHeight));
                        if (lineWidth + newWord.PreferredWidth >= maxWidth) {
                            while (lineWidth + newWord.PreferredWidth >= maxWidth) {
                                if (wordCount <= 0) {
                                    string calcStr = string.Empty;
                                    string wordStr = string.Empty;
                                    string leftStr = string.Empty;

                                    for (int sIndex = 0; sIndex < wordData.Length; ++sIndex) {
                                        calcStr += wordData[sIndex];
                                        newWord.SetText(calcStr);
                                        if (newWord.PreferredWidth >= maxWidth) {
                                            wordStr = wordData.Substring(0, sIndex - 1);
                                            leftStr = wordData.Substring(sIndex - 1);
                                            break;
                                        }
                                    }
                                    SubUI_LogWord slicedWord = Instantiate(subUI_LogWord, m_contentArea);
                                    slicedWord.Init(string.IsNullOrEmpty(wordStr) ? wordData : wordStr, keyword);
                                    slicedWord.SetShape(new Vector2(lineWidth, (lineCount - 1) * m_LineHeight));

                                    lineWidth += slicedWord.PreferredWidth;

                                    wordData = leftStr;
                                    newWord.SetText(wordData);

                                    if (string.IsNullOrEmpty(wordData) == false) {
                                        lineCount++;
                                        lineWidth = 0;
                                        wordCount = 0;
                                    }
                                }
                                else {
                                    lineCount++;
                                    lineWidth = 0;
                                    wordCount = 0;
                                }
                            }
                            if (string.IsNullOrEmpty(wordData) == false) {
                                newWord.SetText(wordData);
                                newWord.SetShape(new Vector2(lineWidth, (lineCount - 1) * m_LineHeight));
                                lineWidth += newWord.PreferredWidth;
                            }
                            else {
                                Destroy(newWord.gameObject);
                            }
                        }
                        else {
                            lineWidth += newWord.PreferredWidth;
                        }
                    }

                    if (i < splitData.Length - 1) {
                        lineCount++;
                        lineWidth = 0;
                        wordCount = 0;
                    }
                }
            }
        }

        public void Init(string name, string content) {
            SetText_Name(name);
            SetText_Content(content);
        }
    }

}

