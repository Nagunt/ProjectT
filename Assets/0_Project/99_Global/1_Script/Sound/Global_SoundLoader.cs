using UnityEngine;

namespace TP.Sound {
    public class Global_SoundLoader : MonoBehaviour {
        private static Global_SoundLoader m_instance = null;
        [SerializeField] private SerializableDictionary<SoundID, AudioClip> m_SoundData;

        private void Awake() {
            m_instance = this;
        }

        public static AudioClip GetClip(string key) {
            if (m_instance == null) return null;
            if (System.Enum.TryParse(key, out SoundID id)) {
                return GetClip(id);
            }
            return null;
        }

        public static AudioClip GetClip(SoundID key) {
            if (m_instance == null) return null;
            if (m_instance.m_SoundData.TryGetValue(key, out AudioClip value)) {
                return value;
            }
            return null;
        }
    }
}

