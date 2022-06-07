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
            if (time > 0) {
                m_CanvasGroup.alpha = 1;
                fadeTweener = m_CanvasGroup.
                    DOFade(0, time).
                    OnComplete(() => {
                        m_CanvasGroup.alpha = 0;
                        Debug.Log("fadein end");
                        onComplete?.Invoke();
                    }).
                    Play();
            }
            else {
                m_CanvasGroup.alpha = 0;
                onComplete?.Invoke();
            }
        }
        public void FadeOut(float time, UnityAction onComplete) {
            if (fadeTweener != null) {
                fadeTweener.Kill(true);
                fadeTweener = null;
            }
            if (time > 0) {
                m_CanvasGroup.alpha = 0;
                fadeTweener = m_CanvasGroup.
                    DOFade(1, time).
                    OnComplete(() => {
                        m_CanvasGroup.alpha = 1;
                        Debug.Log("fadeout end");
                        onComplete?.Invoke();
                    }).
                    Play();
            }
            else {
                m_CanvasGroup.alpha = 1;
                onComplete?.Invoke();
            }
        }
    }
}


