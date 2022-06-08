using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TP.Scene;

namespace TP.UI {

    public class UI_PauseMenu : UI_Default {
        [Header("- PauseMenu")]
        [SerializeField]
        private Button button_Book;
        [SerializeField]
        private Button button_Save;
        [SerializeField]
        private Button button_Load;
        [SerializeField]
        private Button button_Setting;
        [SerializeField]
        private Button button_Home;


        protected override void Start() {
            button_Book.onClick.AddListener(OnClick_Book);
            button_Save.onClick.AddListener(OnClick_Save);
            button_Load.onClick.AddListener(OnClick_Load);
            button_Setting.onClick.AddListener(OnClick_Setting);
            button_Home.onClick.AddListener(OnClick_Home);
        }

        private void OnClick_Book() {
            Event.Global_EventSystem.UI.Call(UIEventID.World_도감UIOpen);
        }

        private void OnClick_Save() {
            Event.Global_EventSystem.UI.Call(UIEventID.Global_저장UIOpen);
        }

        private void OnClick_Load() {
            Event.Global_EventSystem.UI.Call(UIEventID.Global_로드UIOpen);
        }

        private void OnClick_Setting() {
            Event.Global_EventSystem.UI.Call(UIEventID.Global_설정UIOpen);
        }

        private void OnClick_Home() {
            Sound.Global_SoundManager.StopAll(Sound.Global_SoundManager.SoundOption.FadeOut, 1f);
            Global_SceneManager.LoadScene(SceneID.Title);
        }
    }

}

