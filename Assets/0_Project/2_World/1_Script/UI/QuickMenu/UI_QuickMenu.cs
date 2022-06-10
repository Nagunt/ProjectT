using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP.UI {


    public class UI_QuickMenu : UI_Default {
        [Header("- QuickMenu")]
        [SerializeField]
        private Button button_Pause;
        [SerializeField]
        private Button button_QuickSave;
        [SerializeField]
        private Button button_QuickLoad;
        [SerializeField]
        private Button button_Log;
        [SerializeField]
        private Toggle toggle_Skip;

        protected override void Start() {
            button_Pause.onClick.AddListener(OnClick_Pause);
            button_QuickSave.onClick.AddListener(OnClick_QuickSave);
            button_QuickLoad.onClick.AddListener(OnClick_QuickLoad);
            button_Log.onClick.AddListener(OnClick_Log);
            toggle_Skip.onValueChanged.AddListener(OnClick_Skip);

            Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipValueChanged;
        }

        private void OnClick_Pause() {
            Event.Global_EventSystem.UI.Call(UIEventID.World_정지메뉴UIOpen);
        }

        private void OnClick_QuickSave() {
            Data.Global_LocalData.Save.Current.QuickSave();
        }

        private void OnClick_QuickLoad() {
            if (Data.Global_LocalData.Save.Check(Data.Global_LocalData.Save.SLOT_QUICKSAVE)) {
                Debug.Log($"퀵 세이브 파일 로드");
                Sound.Global_SoundManager.StopAll();
                Data.Global_LocalData.Save.Load(Data.Global_LocalData.Save.SLOT_QUICKSAVE);
                Scene.Global_SceneManager.LoadScene(Scene.SceneID.World, 1f);
            }
        }

        private void OnClick_Log() {
            Event.Global_EventSystem.UI.Call(UIEventID.World_로그UIOpen);
        }

        private void OnClick_Skip(bool state) {
            Event.Global_EventSystem.VisualNovel.CallOnSkipStateChanged(state);
        }

        private void OnSkipValueChanged(bool state) {
            toggle_Skip.isOn = state;
        }

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipValueChanged;
        }
    }
}

