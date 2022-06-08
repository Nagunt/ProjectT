using System.IO;
using UnityEngine;
using TP.Event;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TP.Data {
    public struct TPLogData {
        public string name;
        public string content;
        public TPLogData(string _name, string _content) {
            name = _name;
            content = _content;
        }

        public bool Check() {
            return string.IsNullOrEmpty(name) == false || string.IsNullOrEmpty(content) == false;
        }
    }

    [System.Serializable]
    public struct TPSpriteData {
        public string name;
        public float posX;
        public float posY;
        public float posZ;
        public float scaleX;
        public float scaleY;
        public float scaleZ;
        public string imageKey;
        public float r;
        public float g;
        public float b;
        public float a;
        public bool flipX;
        public bool flipY;
        public int spriteType;

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
        public Vector3 Scale {
            get {
                return new Vector3(scaleX, scaleY, scaleZ);
            }
            set {
                scaleX = value.x;
                scaleY = value.y;
                scaleZ = value.z;
            }
        }
        public Color Color {
            get {
                return new Color(r, g, b, a);
            }
            set {
                r = value.r;
                g = value.g;
                b = value.b;
                a = value.a;
            }
        }
    }

    public static class Global_LocalData {
        public static class Setting {
            [System.Serializable]
            public class SettingData {
                public float bgm;
                public float sfx;
                public int speed;

                public SettingData() {
                    bgm = 0.5f;
                    sfx = 0.5f;
                    speed = 3;
                }

                public override string ToString() {
                    return $"BGM : {bgm}\nSFX : {sfx}\nSpeed : {speed}";
                }
            }

            private static SettingData data;

            public static float BGM {
                get {
                    if (data == null) Load();
                    return data.bgm;
                }
                set {
                    if (0 <= value && value <= 1f) {
                        if (data == null) Load();
                        if (data.bgm != value) {
                            data.bgm = value;
                            Global_EventSystem.Sound.CallOnBGMValueChanged(value);
                        }
                    }
                }
            }

            public static float SFX {
                get {
                    if (data == null) Load();
                    return data.sfx;
                }
                set {
                    if (0 <= value && value <= 1f) {
                        if (data == null) Load();
                        data.sfx = value;
                    }
                }
            }

            public static int Speed {
                get {
                    if (data == null) Load();
                    return data.speed;
                }
                set {
                    if (1 <= value && value <= 5) {
                        if (data == null) Load();
                        if (data.speed != value) {
                            data.speed = value;
                            delay = 1f / (2 * data.speed);
                        }
                    }
                }
            }

            private static float delay;
            public static float Delay {
                get {
                    if (data == null) Load();
                    return delay;
                }
            }

            public static void Save() {
                try {
                    string json = JsonUtility.ToJson(data, true);
                    string path = Application.persistentDataPath + "/" + "settings.dat";
                    File.WriteAllText(path, json);
                }
                catch (IOException) {
                    Debug.Log("설정값 세이브 에러.");
                }
            }

            public static void Load() {
                try {
                    string path = Application.persistentDataPath + "/" + "settings.dat";
                    if (File.Exists(path)) {
                        string json = File.ReadAllText(path);
                        data = JsonUtility.FromJson<SettingData>(json);

                    }
                    else {
                        data = new SettingData();
                        string json = JsonUtility.ToJson(data, true);
                        File.WriteAllText(path, json);
                    }
                    delay = 1f / (2 * data.speed);
                }
                catch (IOException) {
                    Debug.Log("설정값 로드 에러.");
                }
            }
        }

        public static class Save {
            [System.Flags]
            public enum FlagID {
                None = 0,
                Flag_1 = 1 << 0,
                Flag_2 = 1 << 1,
                Flag_3 = 1 << 2,
                Flag_4 = 1 << 3,
                Flag_5 = 1 << 4,
                Flag_6 = 1 << 5,
                Flag_7 = 1 << 6,
                Flag_8 = 1 << 7,
                All = int.MaxValue
            }
            [System.Flags]
            public enum BookID {
                None = 0,
                Book_1 = 1 << 0,
                Book_2 = 1 << 1,
                Book_3 = 1 << 2,
                Book_4 = 1 << 3,
                Book_5 = 1 << 4,
                Book_6 = 1 << 5,
                Book_7 = 1 << 6,
                Book_8 = 1 << 7,
                All = int.MaxValue
            }

            private static SaveData current;
            public static SaveData Current {
                get {
                    if (current == null) {
                        current = new SaveData();
                    }
                    return current;
                }
            }
            public static bool Check(int _slot) {
                return File.Exists(Application.persistentDataPath + $"/Save/save_{_slot}.TIGER");
            }
            public static void Load(int slot) {
                Load(slot, out current);
            }
            public static void Load(int slot, out SaveData data) {
                string path = Application.persistentDataPath + $"/Save/save_{slot}.TIGER";
                if (File.Exists(path)) {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileStream fStream = new FileStream(path, FileMode.Open);

                    SaveData saveData = binaryFormatter.Deserialize(fStream) as SaveData;
                    fStream.Close();

                    data = saveData;
                }
                else {
                    Debug.Log("File not Exist");
                    data = new SaveData();
                }
            }
            public static void Reset() {
                string path = Application.persistentDataPath + "/Save";
                DirectoryInfo di = new DirectoryInfo(path);
                di.Delete(true);
                Directory.CreateDirectory(path);
            }
            public static void Delete(int slot) {
                string path = Application.persistentDataPath + $"/Save/save_{slot}.TIGER";
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }
            public class SaveData {
                public int cursor;
                public FlagID flag;
                public BookID book;
                public string gameTime;
                public string realTime;
                public byte[] thumbnailData;
                //public MyAudioData[] audioData;
                //public MySpriteData[] spriteData;

                [System.NonSerialized]
                private Queue<TPLogData> tpLogData;

                public IReadOnlyCollection<TPLogData> TPLogData {
                    get {
                        if (tpLogData == null) {
                            tpLogData = new Queue<TPLogData>(15);
                        }
                        return tpLogData.ToList();
                    }
                }

                [System.NonSerialized]
                private Texture2D thumbnail;
                public Texture2D ThumbNail {
                    get {
                        if (thumbnailData == null) return null;
                        if (thumbnail == null) {
                            thumbnail = new Texture2D(2, 2);
                            thumbnail.LoadImage(thumbnailData);
                        }
                        return thumbnail;
                    }
                }
                public SaveData() {
                    cursor = 0;
                    flag = FlagID.None;
                    book = BookID.None;
                    gameTime = string.Empty;
                    realTime = string.Empty;
                    thumbnail = null;
                    thumbnailData = null;
                    //audioData = null;
                    //spriteData = null;
                    tpLogData = null;
                }
                public void Save(int _slot) {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    string path = Application.persistentDataPath + "/Save";
                    Directory.CreateDirectory(path);
                    FileStream fStream = new FileStream(path + $"/save_{_slot}.TIGER", FileMode.Create);
                    binaryFormatter.Serialize(fStream, this);
                    fStream.Close();
                }
                public void QuickSave(int _slot) {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    string path = Application.persistentDataPath + "/Save";
                    Directory.CreateDirectory(path);
                    FileStream fStream = new FileStream(path + $"/save_{_slot}.TIGER", FileMode.Create);

                    // 퀵세이브에 필요하지 않은 요소들은 제거
                    realTime = string.Empty;
                    thumbnail = null;
                    thumbnailData = null;

                    binaryFormatter.Serialize(fStream, this);
                    fStream.Close();
                }
                public void SetThumbnail(byte[] _data) {
                    thumbnailData = _data;
                    thumbnail = new Texture2D(2, 2);
                    thumbnail.LoadImage(thumbnailData);
                }

                public void AddTPLogData(TPLogData data) {
                    if (tpLogData == null) {
                        tpLogData = new Queue<TPLogData>(15);
                    }
                    if (tpLogData.Count > 15) {
                        for (int i = tpLogData.Count; i >= 15; --i) {
                            tpLogData.Dequeue();
                        }
                    }
                    tpLogData.Enqueue(data);
                }
            }
        }
    }
}
