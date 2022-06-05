using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TP.UI;
using TP.Event;


namespace TP.Scene {
    public class Global_SceneManager : MonoBehaviour {
        private static Global_SceneManager m_instance = null;

        private void Awake() {
            Debug.Log("씬매니저 초기화");
            m_instance = this;
        }

        public static void LoadScene(SceneID sceneID, float time = 1f) {
            int sceneIndex = (int)sceneID;
            if (time > 0f) {
                Global_EventSystem.UI.Call(UIEventID.Global_터치잠금설정);
                Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeOut, time, () => {
                    Global_EventSystem.Scene.onSceneChanged += OnSceneChanged;
                    SceneManager.LoadScene(sceneIndex);
                });
            }
            else {
                SceneManager.LoadScene(sceneIndex);
            }

            void OnSceneChanged(string scene1, string scene2) {
                Global_EventSystem.Scene.onSceneChanged -= OnSceneChanged;
                Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, time, () => {
                    Global_EventSystem.UI.Call(UIEventID.Global_터치잠금해제);
                });
            }
        }
    }
}

