using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TP.Data;
using TP.Event;
using UnityEngine;

namespace TP.Sound {
    public class Global_SoundManager : MonoBehaviour {

        [Flags]
        public enum SoundOption {
            None = 0,
            FadeIn = 1,         // Pause, Stop에서 무시
            FadeOut = 2,        // Play에서 무시
            Loop = 4,           // Pause, Stop에서 무시
            Only = 8,           // Pause, Stop에서 무시
        }

        [Serializable]
        public struct AudioData {
            public SoundID id;
            public float posX;
            public float posY;
            public float posZ;
            public SoundOption option;
            public bool isPause;
            public Vector3 Position {
                get {
                    return new Vector3(posX, posY, posZ);
                }
                set {
                    posX = value.x;
                    posY = value.y;
                    posZ = value.z;
                }
            }
        }

        private class AudioObject {
            public SoundID ID { get; private set; }
            public SoundOption Option { get; private set; }
            public int Key { get; private set; }
            public Vector3 Position => GameObject.transform.position;
            public GameObject GameObject { get; private set; }
            public AudioSource AudioSource { get; private set; }

            public bool IsBGM => Key >= 0;
            public bool IsLoop => AudioSource.loop;
            public bool IsPause => AudioSource.isPlaying == false && (AudioSource.time < AudioSource.clip.length - 0.02f);
            public bool IsComplete => AudioSource.isPlaying == false && (AudioSource.time >= AudioSource.clip.length - 0.02f);
            public bool IsActiveAndEnabled => AudioSource.isActiveAndEnabled;

            public AudioObject(int key, SoundID id, SoundOption option, AudioSource source) {
                Key = key;
                ID = id;
                Option = option;
                GameObject = source.gameObject;
                AudioSource = source;
            }

            public AudioObject SetActive(bool state) {
                GameObject.SetActive(state);
                return this;
            }

            public AudioObject Pause() {
                AudioSource.Pause();
                Global_EventSystem.Sound.CallOnSoundPause(ID);
                return this;
            }

            public AudioObject UnPause() {
                AudioSource.UnPause();
                return this;
            }

            public void Complete() {
                AudioSource.time = AudioSource.clip.length - 0.01f;
                AudioSource.Pause();
            }

            public AudioSource Reset() {
                GameObject.SetActive(false);
                AudioSource.clip = null;
                AudioSource.time = 0;
                AudioSource.volume = 0;
                AudioSource.loop = false;
                AudioSource.playOnAwake = false;
                return AudioSource;
            }
        }

        private static Global_SoundManager m_instance = null;

        [Header("- Audio")]
        [SerializeField]
        private int MAX_COUNT = 20;
        [SerializeField]
        private float FADE_TIME = 1.5f;
        [Space(20)]
        [SerializeField]
        private AudioSource m_audioSource;
        [SerializeField]
        private Transform m_audioParent;

        private Dictionary<int, AudioObject> m_audio;
        private Queue<AudioSource> m_audioQueue;
        private Dictionary<int, Tweener> m_tweeners;
        private Dictionary<SoundID, AudioClip> m_localSoundData;
        private Dictionary<SoundID, AudioClip> m_globalSoundData;

        private void Awake() {
            Debug.Log("사운드매니저 초기화");
            m_instance = this;
            m_audio = new Dictionary<int, AudioObject>();
            m_audioQueue = new Queue<AudioSource>(MAX_COUNT);
            m_tweeners = new Dictionary<int, Tweener>(MAX_COUNT);
            for (int i = 0; i < MAX_COUNT; ++i) {
                AudioSource newAudio = Instantiate(m_audioSource, m_audioParent);
                newAudio.gameObject.SetActive(false);
                m_audioQueue.Enqueue(newAudio);
            }
            Global_EventSystem.Scene.onSceneChanged += OnSceneChanged;
            Global_EventSystem.Sound.onBGMValueChanged += OnBGMValueChanged;
        }

        public static void AddSoundData(SerializableDictionary<SoundID, AudioClip> soundData, bool isLocal = true) {
            if (isLocal) {
                if (m_instance.m_localSoundData == null) {
                    m_instance.m_localSoundData = new Dictionary<SoundID, AudioClip>();
                }
                foreach (SerializableDictionary<SoundID, AudioClip>.Pair kv in soundData) {
                    if (m_instance.m_localSoundData.ContainsKey(kv.Key)) {
                        m_instance.m_localSoundData[kv.Key] = kv.Value;
                    }
                    else {
                        m_instance.m_localSoundData.Add(kv.Key, kv.Value);
                    }
                }
            }
            else {
                if (m_instance.m_globalSoundData == null) {
                    m_instance.m_globalSoundData = new Dictionary<SoundID, AudioClip>();
                }
                foreach (SerializableDictionary<SoundID, AudioClip>.Pair kv in soundData) {
                    if (m_instance.m_globalSoundData.ContainsKey(kv.Key)) {
                        m_instance.m_globalSoundData[kv.Key] = kv.Value;
                    }
                    else {
                        m_instance.m_globalSoundData.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        public static void PlaySFX(SoundID id) {
            PlaySFXAtLocation(id, Vector3.zero);
        }

        public static void PlaySFX(SoundID id, float volume) {
            PlaySFXAtLocation(id, Global_LocalData.Setting.SFX, Vector3.zero);
        }

        public static void PlaySFXAtLocation(SoundID id, Vector3 position) {
            PlaySFXAtLocation(id, 1f, position);
        }

        public static void PlaySFXAtLocation(SoundID id, float volume, Vector3 position) {
            if (m_instance.m_audioQueue.Count <= 0) {
                Debug.Log("오디오 동시 송출 최대치를 초과했습니다.");
                return;
            }

            AudioClip audioClip = GetClip(id);

            if (audioClip == null) {
                Debug.Log($"{id} : 해당하는 파일이 존재하지 않습니다.");
                return;
            }

            AudioSource newAudio = m_instance.m_audioQueue.Dequeue();

            newAudio.clip = audioClip;
            newAudio.loop = false;
            newAudio.playOnAwake = false;
            newAudio.volume = volume;

            newAudio.gameObject.SetActive(true);
            newAudio.transform.position = position;

            newAudio.Play();
            m_instance.StartCoroutine(m_instance.CheckRoutine(new AudioObject(-1, id, SoundOption.None, newAudio)));
            Global_EventSystem.Sound.CallOnSoundStart(id);
        }

        public static int PlayBGM(SoundID id, SoundOption option = SoundOption.None) {
            return PlayBGMAtLocation(id, Vector3.zero, option);
        }

        public static int PlayBGMAtLocation(SoundID id, Vector3 position, SoundOption option = SoundOption.None) {
            // Only 프로퍼티가 있으면 해당 BGM은 반드시 하나만 존재해야 한다.
            if (option.HasFlag(SoundOption.Only)) {
                int key = -1;
                var list = m_instance.m_audio.Where((kv) => kv.Value.ID == id).ToList();
                if (list.Count > 0) {
                    key = list[0].Value.Key;
                    if (list[0].Value.IsPause) {
                        ResumeBGM(key, option);
                    }
                }
                return key;
            }

            if (m_instance.m_audioQueue.Count <= 0) {
                Debug.Log("오디오 동시 송출 최대치를 초과했습니다.");
                return -1;
            }

            AudioClip audioClip = GetClip(id);

            if (audioClip == null) {
                Debug.Log($"{id} : 해당하는 파일이 존재하지 않습니다.");
                return -1;
            }

            float volume = Global_LocalData.Setting.BGM;

            int channel = m_instance.MAX_COUNT - m_instance.m_audioQueue.Count;
            AudioSource newAudio = m_instance.m_audioQueue.Dequeue();

            newAudio.clip = audioClip;
            newAudio.loop = option.HasFlag(SoundOption.Loop);
            newAudio.volume = 0;
            newAudio.playOnAwake = false;

            newAudio.gameObject.SetActive(true);
            newAudio.transform.position = position;

            if (m_instance.m_tweeners.TryGetValue(channel, out Tweener lastTweener)) {
                lastTweener.Kill(false);
            }
            if (option.HasFlag(SoundOption.FadeIn)) {
                Tweener tweener = newAudio.DOFade(volume, m_instance.FADE_TIME);
                tweener.
                    SetLink(newAudio.gameObject, LinkBehaviour.KillOnDisable).
                    OnStart(() => {
                        newAudio.Play();
                        m_instance.m_tweeners.Add(channel, tweener);
                    }).
                    OnComplete(() => {
                        newAudio.volume = volume;
                    }).
                    OnKill(() => {
                        m_instance.m_tweeners.Remove(channel);
                    }).
                    Play();
            }
            else {
                newAudio.volume = volume;
                newAudio.Play();
            }
            AudioObject audioObject = new AudioObject(channel, id, option, newAudio);
            m_instance.StartCoroutine(m_instance.CheckRoutine(audioObject));
            m_instance.m_audio.Add(channel, audioObject);
            Global_EventSystem.Sound.CallOnSoundStart(id);

            return channel;
        }

        public static void PauseBGM(int id, SoundOption option = SoundOption.None) {
            if (m_instance.m_audio.TryGetValue(id, out AudioObject audioObject)) {
                if (m_instance.m_tweeners.TryGetValue(id, out Tweener lastTweener)) {
                    lastTweener.Kill(false);
                }
                AudioSource audioSource = audioObject.AudioSource;
                if (option.HasFlag(SoundOption.FadeOut)) {
                    Tweener tweener = audioSource.DOFade(0, m_instance.FADE_TIME);
                    tweener.
                        SetLink(audioSource.gameObject, LinkBehaviour.KillOnDisable).
                        OnStart(() => {
                            m_instance.m_tweeners.Add(id, tweener);
                        }).
                        OnComplete(() => {
                            audioSource.volume = 0;
                        }).
                        OnKill(() => {
                            m_instance.m_tweeners.Remove(id);
                            audioObject.Pause();
                        }).
                        Play();
                }
                else {
                    audioObject.Pause();
                }
            }
        }

        public static void ResumeBGM(int id, SoundOption option = SoundOption.None) {
            if (m_instance.m_audio.TryGetValue(id, out AudioObject audioObject) &&
                audioObject.IsPause) {
                if (m_instance.m_tweeners.TryGetValue(id, out Tweener lastTweener)) {
                    lastTweener.Kill(false);
                }
                AudioSource audioSource = audioObject.AudioSource;
                if (option.HasFlag(SoundOption.FadeIn)) {
                    float value = 1f;
                    audioSource.volume = 0;
                    Tweener tweener = audioSource.DOFade(value, m_instance.FADE_TIME);
                    tweener.
                        SetLink(audioSource.gameObject, LinkBehaviour.KillOnDisable).
                        OnStart(() => {
                            audioObject.UnPause();
                            m_instance.m_tweeners.Add(id, tweener);
                        }).
                        OnComplete(() => {
                            audioSource.volume = value;
                        }).
                        OnKill(() => {
                            m_instance.m_tweeners.Remove(id);
                        }).
                        Play();
                    return;
                }
                else {
                    audioObject.UnPause();
                    return;
                }
            }
        }

        public static void StopBGM(int id, SoundOption option = SoundOption.None) {
            if (m_instance.m_audio.TryGetValue(id, out AudioObject audioObject)) {
                if (m_instance.m_tweeners.TryGetValue(id, out Tweener lastTweener)) {
                    lastTweener.Kill(false);
                }
                AudioSource audioSource = audioObject.AudioSource;
                if (option.HasFlag(SoundOption.FadeOut)) {
                    Tweener tweener = audioSource.DOFade(0, m_instance.FADE_TIME);
                    tweener.
                        SetLink(audioSource.gameObject, LinkBehaviour.KillOnDisable).
                        OnStart(() => {
                            m_instance.m_tweeners.Add(id, tweener);
                        }).
                        OnComplete(() => {
                            audioSource.volume = 0;
                        }).
                        OnKill(() => {
                            m_instance.m_tweeners.Remove(id);
                            audioObject.Complete();
                        }).
                        Play();
                }
                else {
                    audioObject.Complete();
                }
            }
        }

        public static void PauseAll(SoundOption option = SoundOption.None) {
            foreach (KeyValuePair<int, AudioObject> kv in m_instance.m_audio) {
                PauseBGM(kv.Key, option);
            }
        }

        public static void ResumeAll(SoundOption option = SoundOption.None) {
            foreach (KeyValuePair<int, AudioObject> kv in m_instance.m_audio) {
                ResumeBGM(kv.Key, option);
            }
        }

        public static void StopAll(SoundOption option = SoundOption.None) {
            foreach (KeyValuePair<int, AudioObject> kv in m_instance.m_audio) {
                StopBGM(kv.Key, option);
            }
        }

        public static void Parse(AudioData[] data) {
            if (data == null) return;
            foreach (AudioData audioData in data) {
                int channel = PlayBGMAtLocation(audioData.id, audioData.Position, audioData.option);
                if (audioData.isPause) {
                    PauseBGM(channel);
                }
            }
        }

        public static AudioData[] ToData() {
            AudioData[] audioData = new AudioData[m_instance.m_audio.Count];
            int index = 0;
            foreach (KeyValuePair<int, AudioObject> kv in m_instance.m_audio) {
                audioData[index++] = new AudioData() {
                    id = kv.Value.ID,
                    option = kv.Value.Option,
                    Position = kv.Value.Position,
                    isPause = kv.Value.IsPause
                };
            }
            return audioData;
        }

        IEnumerator CheckRoutine(AudioObject audioObject) {
            if (audioObject.IsActiveAndEnabled == false) yield break;
            yield return new WaitUntil(() => audioObject.IsComplete);
            if (audioObject.IsActiveAndEnabled == false) yield break;

            if (audioObject.IsBGM) m_audio.Remove(audioObject.Key);
            m_audioQueue.Enqueue(audioObject.Reset());

            Global_EventSystem.Sound.CallOnSoundEnd(audioObject.ID);
        }

        public static AudioClip GetClip(string key) {
            if (m_instance == null) return null;
            if (Enum.TryParse(key, out SoundID id)) {
                return GetClip(id);
            }
            return null;
        }

        public static AudioClip GetClip(SoundID key) {
            if (m_instance == null) return null;
            if (m_instance.m_globalSoundData.TryGetValue(key, out AudioClip globalValue)) {
                return globalValue;
            }
            else {
                if (m_instance.m_localSoundData.TryGetValue(key, out AudioClip localValue)) {
                    return localValue;
                }
            }
            return null;
        }

        private void OnSceneChanged(string current, string next) {
            if (m_localSoundData != null &&
                m_localSoundData.Count > 0) {
                m_localSoundData.Clear();
            }
        }
        private void OnBGMValueChanged(float value) {
            foreach (KeyValuePair<int, AudioObject> kv in m_audio) {
                if (m_tweeners.ContainsKey(kv.Key) == false) {
                    kv.Value.AudioSource.volume = value;
                }
            }
        }
    }
}


