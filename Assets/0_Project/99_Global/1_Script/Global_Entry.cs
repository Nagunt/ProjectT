using UnityEngine;
using UnityEngine.SceneManagement;

namespace TP {
    public class Global_Entry : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void FirstLoad() {
            Debug.Log("엔트리 초기화");
            Application.targetFrameRate = 60;
            Event.Global_EventSystem.Init();
            if (Event.Global_EventSystem.Scene.Current != Scene.SceneID.Entry) {
                SceneManager.LoadScene("Entry");
            }
        }
    }
}
