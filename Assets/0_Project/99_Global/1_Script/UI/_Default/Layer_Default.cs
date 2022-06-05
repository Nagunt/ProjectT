using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.UI {
    public class Layer_Default<T> : MonoBehaviour where T : UI_Default {
        [SerializeField] protected T _uiPrefab = default;
        protected T _uiObject = default;

        public bool IsOpen => _uiObject != null;

        protected virtual void Awake() {
            _uiObject = GetComponentInChildren<T>();
        }

        protected virtual void Start() {
            if (_uiObject != null) {
                OnUIOpened();
            }
        }

        protected virtual void Open() {
            if (_uiObject == null) {
                transform.SetAsLastSibling();
                _uiObject = Instantiate(_uiPrefab, transform);
                OnUIOpened();
            }
        }
        protected virtual void Close() {
            if (_uiObject != null) {
                T disposingObject = _uiObject;
                _uiObject = null;

                disposingObject.Dispose(() => {
                    if (disposingObject != null) {
                        Destroy(disposingObject.gameObject);
                        OnUIClosed();
                    }
                });
            }
        }
        protected virtual void OnUIOpened() {
            _uiObject.SetCloseFunction(Close);
        }
        protected virtual void OnUIClosed() {

        }
    }
}
