using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TP.UI {
    public class UI_Setting : UI_Default {

        [Header("- Setting")]
        [SerializeField]
        private Slider slider_BGM;
        [SerializeField]
        private Slider slider_SFX;
        [SerializeField]
        private Slider slider_Speed;

        protected override void Start() {
            slider_BGM.value = Data.Global_LocalData.Setting.BGM;
            slider_SFX.value = Data.Global_LocalData.Setting.SFX;
            slider_Speed.value = Data.Global_LocalData.Setting.Speed;
        }

        public override void Dispose(UnityAction onEndDispose) {
            Data.Global_LocalData.Setting.BGM = slider_BGM.value;
            Data.Global_LocalData.Setting.SFX = slider_SFX.value;
            Data.Global_LocalData.Setting.Speed = (int)slider_Speed.value;
            Data.Global_LocalData.Setting.Save();
            base.Dispose(onEndDispose);
        }
    }
}

