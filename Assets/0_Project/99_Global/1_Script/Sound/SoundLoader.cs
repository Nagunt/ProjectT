using UnityEngine;
using TP.Data;

namespace TP.Sound {
    public class SoundLoader : Global_DataLoader<SoundID, AudioClip> {

        [Header("- Sound")]

        [SerializeField] private bool isLocal = true;

        private void Start() {
            Global_SoundManager.AddSoundData(ToData(), isLocal);
        }
    }
}

