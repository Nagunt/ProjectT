using System.Collections.ObjectModel;
using UnityEngine;
using TP.Data;

namespace TP.VisualNovel {

    public class SpriteLoader : Global_DataLoader<SpriteID, Sprite> {
        private static SpriteLoader m_instance = null;
        private ReadOnlyDictionary<SpriteID, Sprite> m_data;
        public static ReadOnlyDictionary<SpriteID, Sprite> Data {
            get {
                if (m_instance != null) {
                    return m_instance.m_data;
                }
                return null;
            }
        }

        private void Awake() {
            m_instance = this;
            m_data = ToData();
        }
    }
}
