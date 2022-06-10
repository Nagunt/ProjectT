using UnityEngine;
using UnityEngine.SceneManagement;

namespace TP {
    public class Global_Entry : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void FirstLoad() {
            Debug.Log("이벤트 시스템 초기화");
            Application.targetFrameRate = 60;
            Event.Global_EventSystem.Init();
            Debug.Log(Event.Global_EventSystem.Scene.Current);
            if (Event.Global_EventSystem.Scene.Current != Scene.SceneID.Entry) {
                Debug.Log(SceneManager.GetActiveScene().name + " 이라서 엔트리로 ㄱ");
                SceneManager.LoadScene("Entry");
            }
        }
    }
}
