using System.IO;
using UnityEngine;
using TP.Event;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;
using TP.Sound;

namespace TP.Data {

    [System.Serializable]
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
        public string imageKey;
        public float posX;
        public float posY;
        public float posZ;
        public float scaleX;
        public float scaleY;
        public float scaleZ;
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

    public struct TPSelectionData
    {
        public string data;
        public UnityAction callback;
        public TPSelectionData(string _data, UnityAction _callback)
        {
            data = _data;
            callback = _callback;
        }
    }

    [Serializable]
    public struct TPAudioData
    {
        public SoundID id;
        public float posX;
        public float posY;
        public float posZ;
        public Global_SoundManager.SoundOption option;
        public bool isPause;
        public float fadeTime;
        public Vector3 Position
        {
            get
            {
                return new Vector3(posX, posY, posZ);
            }
            set
            {
                posX = value.x;
                posY = value.y;
                posZ = value.z;
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

            public const int SLOT_QUICKSAVE = 99;

            private static SaveData current;
            public static SaveData Current {
                get {
                    if (current == null) {
                        current = new SaveData();
                    }
                    return current;
                }
            }
            public static bool Check(int slot) {
                return File.Exists(Application.persistentDataPath + $"/Save/save_{slot}.TIGER");
            }
            public static void Load(int slot) {
                Load(slot, out current);
            }
            public static void Load(int slot, out SaveData data) {
                string path = Application.persistentDataPath + $"/Save/save_{slot}.TIGER";
                if (File.Exists(path)) {
                    FileStream fStream = new FileStream(path, FileMode.Open);
                    try
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        SaveData saveData = binaryFormatter.Deserialize(fStream) as SaveData;
                        fStream.Close();

                        data = saveData;
                    }
                    catch (Exception)
                    {
                        Debug.Log("세이브 파일 손상");
                        fStream.Close();
                        File.Delete(path);
                        data = new SaveData();
                    }
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

            [System.Serializable]
            public class SaveData {
                public int cursor;
                public FlagID flag;
                public BookID book;
                public string gameTime;
                public string realTime;
                public byte[] thumbnailData;
                public TPAudioData[] audioData;
                public TPSpriteData[] spriteData;
                public TPLogData lastLogData;

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
                    audioData = null;
                    spriteData = null;
                    lastLogData = default;
                }

                public void Save(int slot) {

                    Global_EventSystem.VisualNovel.CallOnSave(this);

                    realTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm");

                    Camera mainCamera = Camera.main;

                    mainCamera.cullingMask = ~(1 << LayerMask.NameToLayer("UI"));

                    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    mainCamera.targetTexture = renderTexture;

                    Texture2D screenShot = new Texture2D((int)mainCamera.pixelRect.width, (int)mainCamera.pixelRect.height, TextureFormat.RGBA32, false);

                    mainCamera.Render();
                    RenderTexture.active = renderTexture;

                    screenShot.ReadPixels(mainCamera.pixelRect, 0, 0);

                    mainCamera.targetTexture = null;
                    RenderTexture.active = null;
                    mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("UI"));

                    MonoBehaviour.Destroy(renderTexture);

                    SetThumbnail(ResizeTexture(screenShot, mainCamera.pixelRect.size * .1f).EncodeToPNG()); // 일단 용량 최적화를 위해서 줄이기는 하는데......

                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    string path = Application.persistentDataPath + "/Save";
                    Debug.Log(path);
                    Directory.CreateDirectory(path);
                    FileStream fStream = new FileStream(path + $"/save_{slot}.TIGER", FileMode.Create);
                    binaryFormatter.Serialize(fStream, this);
                    fStream.Close();

                    Texture2D ResizeTexture(Texture2D source, Vector2 size)
                    {
                        //*** Get All the source pixels
                        Color[] aSourceColor = source.GetPixels(0);
                        Vector2 vSourceSize = new Vector2(source.width, source.height);

                        //*** Calculate New Size
                        int xWidth = (int)size.x;
                        int xHeight = (int)size.y;

                        //*** Make New
                        Texture2D oNewTex = new Texture2D(xWidth, xHeight, TextureFormat.RGBA32, false);

                        //*** Make destination array
                        int xLength = xWidth * xHeight;
                        Color[] aColor = new Color[xLength];

                        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

                        //*** Loop through destination pixels and process
                        Vector2 vCenter = new Vector2();
                        for (int ii = 0; ii < xLength; ii++)
                        {
                            //*** Figure out x&y
                            float xX = (float)ii % xWidth;
                            float xY = Mathf.Floor((float)ii / xWidth);

                            //*** Calculate Center
                            vCenter.x = (xX / xWidth) * vSourceSize.x;
                            vCenter.y = (xY / xHeight) * vSourceSize.y;

                            //*** Average
                            //*** Calculate grid around point
                            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                            //*** Loop and accumulate
                            //Vector4 oColorTotal = new Vector4();
                            Color oColorTemp = new Color();
                            float xGridCount = 0;
                            for (int iy = xYFrom; iy < xYTo; iy++)
                            {
                                for (int ix = xXFrom; ix < xXTo; ix++)
                                {

                                    //*** Get Color
                                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                                    //*** Sum
                                    xGridCount++;
                                }
                            }

                            //*** Average Color
                            aColor[ii] = oColorTemp / (float)xGridCount;
                        }

                        //*** Set Pixels
                        oNewTex.SetPixels(aColor);
                        oNewTex.Apply();

                        //*** Return
                        return oNewTex;
                    }

                }
                public void Clear() {
                    cursor = 0;
                    flag = FlagID.None;
                    book = BookID.None;
                    gameTime = string.Empty;
                    realTime = string.Empty;
                    thumbnail = null;
                    thumbnailData = null;
                    audioData = null;
                    spriteData = null;
                    lastLogData = default;
                }
                public void QuickSave() {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    string path = Application.persistentDataPath + "/Save";
                    Directory.CreateDirectory(path);
                    FileStream fStream = new FileStream(path + $"/save_{SLOT_QUICKSAVE}.TIGER", FileMode.Create);

                    Global_EventSystem.VisualNovel.CallOnSave(this);

                    // 퀵세이브에 필요하지 않은 요소들은 제거
                    realTime = string.Empty;
                    thumbnail = null;
                    thumbnailData = null;

                    binaryFormatter.Serialize(fStream, this);
                    fStream.Close();
                }
                public void SetThumbnail(byte[] data) {
                    thumbnailData = data;
                    thumbnail = new Texture2D(2, 2);
                    thumbnail.LoadImage(thumbnailData);
                }
            }
        }
    }
}
