using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TP.UI;
using TP.Event;
using System.Collections;

namespace TP.Scene {
    public class Global_SceneManager : MonoBehaviour {
        private static Global_SceneManager m_instance = null;

        private void Awake() {
            Debug.Log("씬매니저 초기화");
            m_instance = this;
        }

        public static void LoadScene(SceneID sceneID, float time = 1f) {
            int sceneIndex = (int)sceneID;
            string currentScene = SceneManager.GetActiveScene().name;
            if (time > 0f) {
                Global_EventSystem.UI.Call(UIEventID.Global_터치잠금설정);
                Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeOut, time, () => {
                    SceneManager.LoadScene(sceneIndex);
                    Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, time, () => {
                        Global_EventSystem.UI.Call(UIEventID.Global_터치잠금해제);
                    });
                });
            }
            else {
                SceneManager.LoadScene(sceneIndex);
            }
        }

        public static void LoadSceneAsync(SceneID sceneID, float time = 1f) {
            int sceneIndex = (int)sceneID;
            if (time > 0f) {
                Global_EventSystem.UI.Call(UIEventID.Global_터치잠금설정);
                Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeOut, time, () => {
                    Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, 0, null);
                    Global_EventSystem.Scene.onSceneChanged += OnSceneChanged;
                    m_instance.LoadSceneAtObject(sceneIndex);
                });
            }
            else {
                m_instance.LoadSceneAtObject(sceneIndex);
            }

            void OnSceneChanged(SceneID current, SceneID next) {
                Global_EventSystem.Scene.onSceneChanged -= OnSceneChanged;
                Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, time, () => {
                    Global_EventSystem.UI.Call(UIEventID.Global_터치잠금해제);
                });
            }
        }

        private void LoadSceneAtObject(int index) {
            StartCoroutine(LoadRoutine(index));
        }

        private IEnumerator LoadRoutine(int index) {
            Global_EventSystem.UI.Call(UIEventID.Global_로딩UIOpen);

            Debug.Log("현재 씬은 " + Global_EventSystem.Scene.Current);

            yield return null;

            Debug.Log("1프레임 대기");

            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            operation.allowSceneActivation = false;

            float timer = 0f;
            float progress = 0f;
            while (!operation.isDone) {
                yield return null;
                timer += Time.unscaledDeltaTime;

                if (operation.progress < 0.9f) {
                    progress = Mathf.Lerp(progress, operation.progress, timer);
                    if (progress >= operation.progress) {
                        timer = 0f;
                    }
                }
                else {
                    progress = Mathf.Lerp(progress, 1f, timer);
                    if (progress >= 1.0f) {
                        operation.allowSceneActivation = true;
                    }
                }

                Global_EventSystem.UI.Call<float>(UIEventID.Global_로딩UI진행도설정, progress);
            }

            Debug.Log("씬 로드 끝");

            Global_EventSystem.UI.Call<float>(UIEventID.Global_로딩UI진행도설정, 1f);

            yield return null;

            Global_EventSystem.UI.Call(UIEventID.Global_로딩UIClose);
        }
    }
}

