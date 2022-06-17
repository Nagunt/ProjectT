using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
namespace TP.Data
{

    public class Global_DataLoader<K, V> : MonoBehaviour {

        [Header("- Data")]
        [SerializeField]
        private List<K> m_Keys;
        [SerializeField]
        private List<V> m_Values;

        protected ReadOnlyDictionary<K, V> m_data;

        public ReadOnlyDictionary<K, V> ToData() {
            Dictionary<K, V> dic = new Dictionary<K, V>();
            for(int i = 0; i < m_Keys.Count; ++i) {
                if (dic.ContainsKey(m_Keys[i])) {
                    dic[m_Keys[i]] = m_Values[i];
                }
                else {
                    dic.Add(m_Keys[i], m_Values[i]);
                }
            }
            return new ReadOnlyDictionary<K, V>(dic);
        }
    }
}
