using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP.VisualNovel {
    public class BackgroundLoader : MonoBehaviour {
        [SerializeField] private SerializableDictionary<BackgroundID, Sprite> m_BackgroundData;
    }
}
