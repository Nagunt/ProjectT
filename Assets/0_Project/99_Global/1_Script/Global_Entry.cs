using UnityEngine;
using UnityEngine.SceneManagement;

namespace TP {
    public class Global_Entry : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void FirstLoad() {
            Debug.Log("이벤트 시스템 초기화");
            Event.Global_EventSystem.Init();
            if (SceneManager.GetActiveScene().name.CompareTo("Entry") != 0) {
                SceneManager.LoadScene("Entry");
            }
        }
    }
}
