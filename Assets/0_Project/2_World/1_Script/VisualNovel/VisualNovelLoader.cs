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

        [SerializeField]
        private TextAsset m_vnData;

        public ReadOnlyCollection<ReadOnlyDictionary<string, object>> ToData()
        {
            string[] dataLines = Regex.Split(m_vnData.text, LINE_SPLIT_RE);
            if (dataLines.Length > 1)
            {
                string[] header = Regex.Split(dataLines[0], SPLIT_RE);
                List<ReadOnlyDictionary<string, object>> csvData = new List<ReadOnlyDictionary<string, object>>();
                for (int num = 1; num < dataLines.Length; ++num)
                {
                    string[] values = Regex.Split(dataLines[num], SPLIT_RE);
                    if (values.Length <= 0) continue;
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    for (int i = 0; i < header.Length && i < values.Length; ++i)
                    {
                        string value = Regex.Replace(values[i], "^\"|\"$", "");
                        entry[header[i]] = value;
                    }
                    csvData.Add(new ReadOnlyDictionary<string, object>(entry));
                }
                return csvData.AsReadOnly();
            }
            return null;
        }
    }
}
