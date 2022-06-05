using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace TP.UI {
    public class UI_Default : MonoBehaviour {
        [SerializeField]
        protected Button button_Close = null;

        protected Sequence showSequence = null;
        protected Sequence hideSequence = null;

        protected virtual void Awake() {
            if (button_Close != null) {
                button_Close.onClick.AddListener(OnClick_Close);
            }
        }

        protected virtual void Start() {
            PlayShowAnimation(() => { });
        }

        public virtual void Dispose(UnityAction onEndDispose) {
            PlayHideAnimation(() => onEndDispose?.Invoke());
        }

        private UnityAction m_closeFunc;
        public void SetCloseFunction(UnityAction unityAction) {
            m_closeFunc = unityAction;
        }
        private void OnClick_Close() {
            m_closeFunc();
        }

        private void PlayShowAnimation(UnityAction onEndAnimation) {
            if (showSequence != null) {
                showSequence.onComplete += () => {
                    onEndAnimation?.Invoke();
                };
                showSequence.Play();
            }
            else {
                onEndAnimation?.Invoke();
            }
        }

        private void PlayHideAnimation(UnityAction onEndAnimaton) {
            if (hideSequence != null) {
                hideSequence.onComplete += () => {
                    onEndAnimaton?.Invoke();
                };
                hideSequence.Play();
            }
            else {
                onEndAnimaton?.Invoke();
            }
        }
    }
}