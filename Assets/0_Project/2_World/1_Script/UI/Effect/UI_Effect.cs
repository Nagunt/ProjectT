using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TP.UI {
    public class UI_Effect : UI_Default {
        [Header("- Effect")]
        [SerializeField]
        private GameObject subUI_Dark;
        [SerializeField]
        private SubUI_TitleEffect subUI_TitleEffect;

        private Sequence shakeSequence = null;

        public void SetDark() {
            subUI_Dark.SetActive(true);
        }

        public void RemoveDark() {
            subUI_Dark.SetActive(false);
        }

        public void Effect_Title(string data, UnityAction callback, bool isDark, bool isSkip) {
            Instantiate(subUI_TitleEffect, transform).MakeTitle(data, callback, isDark, isSkip);
        }

        public void Effect_Shake(float time, UnityAction callback) {
            Sequence sequence = DOTween.Sequence();
            sequence.
                OnStart(() => {
                    Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
                    Event.Global_EventSystem.VisualNovel.onGameStateChanged += OnGameStateChanged;
                    if (shakeSequence.IsActive()) {
                        shakeSequence.Kill(true);
                    }
                    shakeSequence = sequence;
                }).
                AppendCallback(() => Event.Global_EventSystem.UI.Call<float>(UIEventID.World_ÀÌÆåÆ®UIÈçµé¸²È¿°ú, time)).
                AppendInterval(time).
                OnComplete(() => callback?.Invoke()).
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
                    shakeSequence.Play();
                }
                else {
                    shakeSequence.Pause();
                }
            }
        }
    }
}

