using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Data;
using System.Collections.ObjectModel;

namespace TP.VisualNovel
{
    public class CharacterLoader : Global_DataLoader<CharacterID, CharacterData>
    {
        private static CharacterLoader m_instance = null;
        private ReadOnlyDictionary<CharacterID, CharacterData> m_data;
        public static ReadOnlyDictionary<CharacterID, CharacterData> Data {
            get {
                if (m_instance != null) {
                    return m_instance.m_data;
                }
                return null;
            }
        }

        public static string GetPlacement(string name) {
            string placeData = string.Empty;
            foreach (KeyValuePair<CharacterID, CharacterData> kv in Data) {
                if (kv.Value.name.Contains(name)) {
                    placeData = kv.Value.placement;
                    break;
                }
            }
            return placeData;
        }

        private void Awake() {
            m_instance = this;
            m_data = ToData();
        }
    }
}
