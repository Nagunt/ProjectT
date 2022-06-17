using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Data;
using System.Collections.ObjectModel;

namespace TP.VisualNovel {
    public class ParticleLoader : Global_DataLoader<ParticleID, GameObject> {
        private static ParticleLoader m_instance = null;

        public static ReadOnlyDictionary<ParticleID, GameObject> Data {
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
