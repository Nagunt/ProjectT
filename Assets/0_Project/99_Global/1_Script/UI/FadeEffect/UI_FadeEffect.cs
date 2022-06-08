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
            if (fadeTweener != null &&
                fadeTweener.IsPlaying()) {
                fadeTweener.Kill(true);
                fadeTweener = null;
                Debug.Log("�ٸ� ����Ʈ ������, kill");
            }
            if (time > 0) {
                m_CanvasGroup.alpha = 1;
                fadeTweener = m_CanvasGroup.
                    DOFade(0, time).
                    SetUpdate(true).
                    OnComplete(() => {
                        m_CanvasGroup.alpha = 0;
                    }).
                    OnKill(() =>
                    {
                        Debug.Log("���̵��� ��");
                        fadeTweener = null;
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
            if (fadeTweener != null &&
                fadeTweener.IsPlaying()) {
                fadeTweener.Kill(true);
                fadeTweener = null;
                Debug.Log("�ٸ� ����Ʈ ������, kill");
            }
            if (time > 0) {
                m_CanvasGroup.alpha = 0;
                fadeTweener = m_CanvasGroup.
                    DOFade(1, time).
                    SetUpdate(true).
                    OnComplete(() => {
                        m_CanvasGroup.alpha = 1;
                    }).
                    OnKill(() =>
                    {
                        Debug.Log("���̵�ƿ� ��");
                        fadeTweener = null;
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


