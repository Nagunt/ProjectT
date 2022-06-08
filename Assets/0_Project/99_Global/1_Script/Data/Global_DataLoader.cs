using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
namespace TP.Data
{
    public class Global_DataLoader<K, V> : MonoBehaviour
    {
        [Serializable]
        public struct DataKeyValuePair
        {
            public K key;
            public V value;
        }

        [SerializeField]
        private DataKeyValuePair[] m_data;

        public ReadOnlyDictionary<K, V> ToDictionary()
        {
            Dictionary<K, V> dic = new Dictionary<K, V>(m_data.Length);

            for (int i = 0; i < m_data.Length; ++i)
            {
                if (dic.ContainsKey(m_data[i].key))
                {
                    dic[m_data[i].key] = m_data[i].value;
                }
                else
                {
                    dic.Add(m_data[i].key, m_data[i].value);
                }
            }
            return new ReadOnlyDictionary<K, V>(dic);
        }
    }
}
