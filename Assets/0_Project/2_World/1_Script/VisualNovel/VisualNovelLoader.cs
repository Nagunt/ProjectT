using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TP.VisualNovel
{
    public class VisualNovelLoader : MonoBehaviour
    {
        private const string SPLIT_RE = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        private const string LINE_SPLIT_RE = "\r\n|\n\r|\n|\r";
        private static char[] TRIM_CHARS = { '\"' };

        private static VisualNovelLoader m_instance = null;

        public static ReadOnlyDictionary<string, int> IDData {
            get {
                if (m_instance == null) return null;
                if (m_instance.m_idData == null) {
                    m_instance.LoadData();
                }
                return m_instance.m_idData;
            }
        }

        public static ReadOnlyCollection<ReadOnlyDictionary<string, object>> VNData {
            get {
                if (m_instance == null) return null;
                if (m_instance.m_vnData == null) {
                    m_instance.LoadData();
                }
                return m_instance.m_vnData;
            }
        }


        [SerializeField]
        private TextAsset m_data;

        private ReadOnlyDictionary<string, int> m_idData;
        private ReadOnlyCollection<ReadOnlyDictionary<string, object>> m_vnData;

        private void Awake() {
            if (m_instance != null) {
                Destroy(gameObject);
                return;
            }
            m_instance = this;
            m_instance.LoadData();
            DontDestroyOnLoad(m_instance.gameObject);
        }

        private void LoadData() {
            string[] dataLines = Regex.Split(m_data.text, LINE_SPLIT_RE);
            Dictionary<string, int> idData = new Dictionary<string, int>(dataLines.Length);
            if (dataLines.Length > 1) {
                string[] header = Regex.Split(dataLines[0], SPLIT_RE);
                List<ReadOnlyDictionary<string, object>> csvData = new List<ReadOnlyDictionary<string, object>>();
                for (int num = 1; num < dataLines.Length; ++num) {
                    string[] values = Regex.Split(dataLines[num], SPLIT_RE);
                    if (values.Length <= 0) continue;
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    for (int i = 0; i < header.Length && i < values.Length; ++i) {
                        string value = Regex.Replace(values[i], "^\"|\"$", "");
                        entry[header[i]] = value;
                        if (header[i].Equals("_id") &&
                            idData.ContainsKey(value) == false) {
                            idData.Add(value, num);
                        }
                    }
                    csvData.Add(new ReadOnlyDictionary<string, object>(entry));
                }
                m_idData = new ReadOnlyDictionary<string, int>(idData);
                m_vnData = csvData.AsReadOnly();
            }
            else Debug.Log("Data Error");
        }
    }
}
