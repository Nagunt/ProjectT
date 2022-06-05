using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

namespace TP.UI {
    public class UI_FadeEffect : UI_Default {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        private Tweener fadeTweener = null;
        public void FadeIn(float time, UnityAction onComplete) {
            if (fadeTweener != null) {
                fadeTweener.Kill(true);
                fadeTweener = null;
            }
            fadeTweener = m_CanvasGroup.
                DOFade(0, time).
                OnPlay(() => {
                    m_CanvasGroup.alpha = 1;
                })
                .OnComplete(() => {
                    m_CanvasGroup.alpha = 0;
                    onComplete?.Invoke();
                }).
                Play();
        }
        public void FadeOut(float time, UnityAction onComplete) {
            if (fadeTweener != null) {
                fadeTweener.Kill(true);
                fadeTweener = null;
            }
            fadeTweener = m_CanvasGroup.
                DOFade(1, time).
                OnPlay(() => {
                    m_CanvasGroup.alpha = 0;
                })
                .OnComplete(() => {
                    m_CanvasGroup.alpha = 1;
                    onComplete?.Invoke();
                }).
                Play();
        }
    }
}


