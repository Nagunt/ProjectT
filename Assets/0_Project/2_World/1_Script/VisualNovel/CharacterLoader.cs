using UnityEngine;

namespace TP.VisualNovel {

    public class CharacterLoader : MonoBehaviour {
        [SerializeField] private SerializableDictionary<CharacterID, Sprite> m_CharacterData;
    }
}
