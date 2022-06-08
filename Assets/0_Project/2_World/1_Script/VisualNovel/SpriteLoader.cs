using UnityEngine;

namespace TP.VisualNovel {

    public class SpriteLoader : MonoBehaviour {
        [SerializeField] private SerializableDictionary<SpriteID, Sprite> m_SpriteData;
    }
}
