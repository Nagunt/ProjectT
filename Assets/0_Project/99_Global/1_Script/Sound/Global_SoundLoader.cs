using UnityEngine;

namespace TP.Sound {
    public class Global_SoundLoader : MonoBehaviour {

        [SerializeField] private bool isLocal = true;
        [SerializeField] private SerializableDictionary<SoundID, AudioClip> m_SoundData;

        private void Start() {
            Global_SoundManager.AddSoundData(m_SoundData, isLocal);
        }
    }
}

