using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Data;
using System.Collections.ObjectModel;

namespace TP.VisualNovel {
    public class FilterLoader : Global_DataLoader<FilterID, Sprite> {
        private static FilterLoader m_instance = null;

        public static ReadOnlyDictionary<FilterID, Sprite> Data {
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
