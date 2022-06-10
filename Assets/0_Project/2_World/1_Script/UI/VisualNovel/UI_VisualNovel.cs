using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TP.Data;
using TP.Event;
using TP.VisualNovel;
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

        private Sequence shakeSequence = null;

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

            float interval = 500f;
            float firstX = -(interval * .5f) * (enableIndex.Count - 1);

            for (int i = 0; i < enableIndex.Count; ++i) {
                image_Character[i].rectTransform.anchoredPosition = new Vector2(firstX + interval * i, 0);
                if (i > (enableIndex.Count - 1) * .5f) {
                    image_Character[i].rectTransform.eulerAngles = new Vector3(0, 180, 0);
                }
                else {
                    image_Character[i].rectTransform.eulerAngles = Vector3.zero;
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
                SortSprite();
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

        public void Effect_Shake(float time) {
            List<Vector2> firstPos = new List<Vector2>();
            List<Vector2> lastPos = new List<Vector2>();
            List<RectTransform> shakeTarget = new List<RectTransform>();
            for(int i = 0; i < image_Character.Length; ++i) {
                if (image_Character[i].gameObject.activeSelf) {
                    shakeTarget.Add(image_Character[i].rectTransform);
                    firstPos.Add(image_Character[i].rectTransform.anchoredPosition);
                    lastPos.Add(Vector2.zero);
                }
            }

            if (shakeTarget.Count <= 0) return;

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

            for (int i = 0; i < shakeTarget.Count; ++i) {
                if (i == 0) {
                    sequence.Append(shakeTarget[i].DOShakeAnchorPos(time));
                }
                else {
                    sequence.Join(shakeTarget[i].DOShakeAnchorPos(time));
                }
            }

            sequence.
                OnComplete(() => {
                    for (int i = 0; i < shakeTarget.Count; ++i) {
                        shakeTarget[i].anchoredPosition = firstPos[i];
                    }
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
                    for(int i = 0; i < shakeTarget.Count; ++i) {
                        shakeTarget[i].anchoredPosition = lastPos[i];
                    }
                    shakeSequence.Play();
                }
                else {
                    for (int i = 0; i < shakeTarget.Count; ++i) {
                        lastPos[i] = shakeTarget[i].anchoredPosition;
                        shakeTarget[i].anchoredPosition = firstPos[i];
                    }
                    shakeSequence.Pause();
                }
            }
        }

        public TPSpriteData[] ToData() {
            TPSpriteData[] data = new TPSpriteData[image_Character.Length + 2];
            for (int i = 0; i < data.Length; ++i) {
                switch (i) {
                    case 0:
                        data[i] = image_Background.ToData();
                        break;
                    case 1:
                        data[i] = image_Filter.ToData();
                        break;
                    default:
                        data[i] = image_Character[i - 2].ToData();
                        break;
                }
            }
            return data;
        }

        public void Parse(TPSpriteData[] data) {
            if (data != null) {
                image_Background.Parse(data[0]);
                image_Filter.Parse(data[1]);
                for (int i = 0; i < image_Character.Length; ++i) {
                    if (i + 2 < data.Length) {
                        image_Character[i].Parse(data[i + 2]);
                    }
                }
            }
        }
    }
}
