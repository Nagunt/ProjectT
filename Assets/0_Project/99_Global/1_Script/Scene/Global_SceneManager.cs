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
                    Global_EventSystem.Scene.CallOnSceneChanged(currentScene, sceneID.ToString());
                    Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, time, () => {
                        Global_EventSystem.UI.Call(UIEventID.Global_터치잠금해제);
                    });
                });
            }
            else {
                SceneManager.LoadScene(sceneIndex);
                Global_EventSystem.Scene.CallOnSceneChanged(currentScene, sceneID.ToString());
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

            void OnSceneChanged(string scene1, string scene2) {
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
            Debug.Log("로딩 시작");
            Global_EventSystem.UI.Call(UIEventID.Global_로딩UIOpen);
            string currentScene = SceneManager.GetActiveScene().name;

            yield return null;

            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            operation.allowSceneActivation = false;

            while (!operation.isDone) {
                Debug.Log(operation.progress);
                Global_EventSystem.UI.Call<float>(UIEventID.Global_로딩UI진행도설정, operation.progress);
                if (operation.progress >= 0.9f) {
                    if (operation.allowSceneActivation == false) {
                        Global_EventSystem.UI.Call<float>(UIEventID.Global_로딩UI진행도설정, 0.99f);
                        operation.allowSceneActivation = true;
                        yield return new WaitForEndOfFrame();
                        Debug.Log("allowSecenActivation true");
                        Global_EventSystem.Scene.CallOnSceneChanged(currentScene, ((SceneID)index).ToString());
                    }
                    else {
                        Debug.Log("asdf");
                    }
                }
                yield return null;
            }
            Debug.Log("op true");

            Global_EventSystem.UI.Call<float>(UIEventID.Global_로딩UI진행도설정, 1f);

            yield return null;

            Global_EventSystem.UI.Call(UIEventID.Global_로딩UIClose);
        }
    }
}

