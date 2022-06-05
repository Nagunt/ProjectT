using UnityEngine;
using TP.Scene;

namespace TP {
    public class Global_Instance : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            Global_SceneManager.LoadScene(SceneID.Title, 0f);
        }
    }
}
