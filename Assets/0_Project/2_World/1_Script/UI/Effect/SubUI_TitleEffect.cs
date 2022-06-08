using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TP.UI
{
    public class SubUI_TitleEffect : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        [SerializeField]
        private TextMeshProUGUI text_TitleEffect;

        public SubUI_TitleEffect MakeTitle(string data, UnityAction onComplete, bool isDark = false, bool isSkip = false)
        {
            Sequence sequence = DOTween.Sequence();
            text_TitleEffect.SetText(data);
            text_TitleEffect.maxVisibleCharacters = 0;
            int maxCount = Regex.Replace(data, @"<(.|\n)*?>", string.Empty).Length;

            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;

            if (isSkip == false &&
                isDark == false)
            {
                sequence.Append(m_CanvasGroup.DOFade(1, 1.5f));
            }

            if (isDark)
            {
                Event.Global_EventSystem.UI.Call(UIEventID.Global_FadeIn, 0);
                m_CanvasGroup.alpha = 1;
                m_CanvasGroup.interactable = true;
                m_CanvasGroup.blocksRaycasts = true;
            }

            if (isSkip)
            {
                text_TitleEffect.maxVisibleCharacters = maxCount;
                sequence.
                    AppendInterval(TP.Data.Global_LocalData.Setting.Delay).
                    AppendCallback(() => onComplete?.Invoke());
            }
            else
            {
                sequence.
                    Append(
                        DOTween.To(
                            x => text_TitleEffect.maxVisibleCharacters = (int)x,
                            0,
                            maxCount,
                            maxCount / (float)(TP.Data.Global_LocalData.Setting.Speed * 12)).SetEase(Ease.Linear)).
                    AppendInterval(2f).
                    AppendCallback(() => onComplete?.Invoke()).
                    Append(m_CanvasGroup.DOFade(0, 1.5f));
            }

            sequence.AppendCallback(() =>
            {
                m_CanvasGroup.alpha = 0;
                m_CanvasGroup.interactable = false;
                m_CanvasGroup.blocksRaycasts = false;
            }).
            OnKill(() => Destroy(gameObject)).
            Play();

            return this;
        }

    }
}

