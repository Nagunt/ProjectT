using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TP.Data;
using TP.Event;
using TP.Sound;
using TP.UI;
using UnityEngine;
using UnityEngine.Events;
using static TP.Data.Global_LocalData.Save;

namespace TP.VisualNovel
{
    public class Master_VisualNovel : MonoBehaviour {

        private const string KEY_ID = "_id";
        private const string KEY_NAME = "name";
        private const string KEY_SENTENCE = "sentence";
        private const string KEY_COMMAND = "cmd";

        [SerializeField]
        private BackgroundLoader backgroundLoader;
        [SerializeField]
        private SpriteLoader spriteLoader;
        [SerializeField]
        private CharacterLoader characterLoader;
        [SerializeField]
        private VisualNovelLoader vnLoader;

        public static Master_VisualNovel Instance { get; private set; } = null;

        /// <summary>
        /// ��ŵ�� ���¸� ��Ÿ���� ����. false�� �� ��ŵ�� ���� �ʴ� �����Դϴ�.
        /// </summary>
        public bool IsSkip {
            get {
                return Event.Global_EventSystem.VisualNovel.Skip;
            }
        }

        /// <summary>
        /// ������ ���¸� ��Ÿ���� ����. false�� �� �Ͻ����� �����Դϴ�.
        /// </summary>
        public bool GameState {
            get {
                return Event.Global_EventSystem.VisualNovel.GameState;
            }
        }

        public SaveData Current {
            get {
                return Global_LocalData.Save.Current;
            }
        }

        private bool isFirst = true;
        private bool isOpening = false;
        private bool isClearDialogue = true;

        private bool isTouch = false;
        private bool isWaitForTouch = false;

        private WaitForSeconds waitForDelay;

        private ReadOnlyCollection<ReadOnlyDictionary<string, object>> vnData;
        private ReadOnlyDictionary<CharacterID, CharacterData> charData;
        private ReadOnlyDictionary<SpriteID, Sprite> spriteData;
        private ReadOnlyDictionary<BackgroundID, Sprite> backgroundData;

        private Dictionary<SoundID, int> channelInfo;

        // Start is called before the first frame update
        void Start() {
            StartCoroutine(VisualNovelRoutine());
        }

        IEnumerator VisualNovelRoutine() {

            yield return new WaitForEndOfFrame();

            // �ݹ� ���
            Global_EventSystem.Sound.onSoundEnd += OnSoundEnd;
            Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, OnSpeedValueChanged);

            Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);

            // ������ �ε�

            vnData = vnLoader.ToData();
            charData = characterLoader.ToDictionary();
            spriteData = spriteLoader.ToDictionary();
            backgroundData = backgroundLoader.ToDictionary();

            channelInfo = Global_SoundManager.Parse(Current.audioData);

            if (Current.lastLogData.Check())
            {
                Global_EventSystem.VisualNovel.CallOnLogDataAdded(Current.lastLogData);
                Global_EventSystem.UI.Call(UI.UIEventID.World_��ȭUI�̸�����, Current.lastLogData.name);
                Global_EventSystem.UI.Call<string, UnityAction>(UIEventID.World_��ȭUI���밻��, Current.lastLogData.content, null);
            }
            
            if(charData == null)
            {
                Debug.Log("tq");
            }
            Global_EventSystem.UI.Call(UIEventID.World_����UI�����ͼ���, charData);

            isFirst = true;
            isOpening = false;

            waitForDelay = new WaitForSeconds(Global_LocalData.Setting.Delay);

            while (true) {
                string _id = vnData[Current.cursor][KEY_ID].ToString();
                if (string.IsNullOrEmpty(_id)) {
                    Debug.Log("VN DATA ��");
                    break;
                }

                Debug.Log($"{Current.cursor} �� ����");

                isTouch = false;
                isClearDialogue = true;

                string cmdData = vnData[Current.cursor][KEY_COMMAND].ToString();
                if (string.IsNullOrEmpty(cmdData) == false) {
                    List<TPCommand> cmds = TPCommand.Build(cmdData);
                    for (int i = 0; i < cmds.Count; ++i) {
                        Debug.Log("����: " + cmds[i].ToString());
                        cmds[i].Execute(this);
                        if (TPCommand.runningCmd != null) {
                            yield return new WaitUntil(() => TPCommand.runningCmd.IsEnd);
                            TPCommand.runningCmd = null;
                            yield return new WaitUntil(() => GameState);
                        }
                    }
                }
                if(isFirst && isOpening == false)
                {
                    Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
                }

                ReadOnlyDictionary<string, object> currentData = vnData[Current.cursor];
                string nameText = currentData[KEY_NAME].ToString();
                string contentText = currentData[KEY_SENTENCE].ToString().Replace("\"\"", "\"");

                if (isClearDialogue) {
                    TPLogData newLogData = new TPLogData(nameText, contentText);
                    Current.lastLogData = newLogData;
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI�̸�����, nameText);
                    Global_EventSystem.VisualNovel.CallOnLogDataAdded(newLogData);
                }
                else {
                    TPLogData lastLogData = Current.lastLogData;
                    lastLogData.content += contentText;
                    Current.lastLogData = lastLogData;
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI�̸�����, lastLogData.name);
                    Global_EventSystem.VisualNovel.CallOnLogDataModified(lastLogData);
                }

                if (string.IsNullOrEmpty(contentText) == false) {
                    bool isComplete = false;
                    Global_EventSystem.UI.Call<string, UnityAction>(
                        isClearDialogue ? UIEventID.World_��ȭUI���뼳�� : UIEventID.World_��ȭUI�������, 
                        contentText, 
                        () => isComplete = true);
                    yield return new WaitUntil(() => isComplete);
                    if (IsSkip) {
                        yield return waitForDelay;
                    }
                    else {
                        isWaitForTouch = true;
                        yield return new WaitUntil(() => isTouch);
                        isWaitForTouch = false;
                    }
                }
                yield return new WaitUntil(() => GameState);

                if (isFirst) isFirst = false;

                Current.cursor += 1;
            }
        }

        #region �ݹ�

        private void OnClick() {
            if (isWaitForTouch) {
                isTouch = true;
            }
        }

        private void OnSoundEnd(SoundID id)
        {
            if (channelInfo.ContainsKey(id))
            {
                channelInfo.Remove(id);
            }
        }
        private void OnSkipStateChanged(bool state) {
            if (isWaitForTouch) {
                if (state) {
                    isTouch = true;
                }
            }
        }

        private void OnSpeedValueChanged() {
            waitForDelay = new WaitForSeconds(Data.Global_LocalData.Setting.Delay);
        }

        #endregion

        #region Command ��� ����

        public void Wait(float time) {
            if (IsSkip == false) {
                Sequence sequence = DOTween.Sequence();
                sequence.
                    AppendInterval(time).
                    OnComplete(() => {
                        Global_EventSystem.VisualNovel.CallOnCommandEnd();
                    }).
                    Play();
            }
            else {
                Global_EventSystem.VisualNovel.CallOnCommandEnd();
            }
        }

        // _name ȿ������ ����մϴ�.
        public void PlaySFX(string name) {
            if (Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PlaySFX(id);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� ����մϴ�.
        public void PlayBGM(string name) {
            if(Enum.TryParse(name, out SoundID id))
            {
                int channel = Global_SoundManager.PlayBGM(id, Global_SoundManager.SoundOption.Loop | Global_SoundManager.SoundOption.Only);
                if (channelInfo.ContainsKey(id) == false)
                {
                    channelInfo.Add(id, channel);
                }
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� �Ͻ������մϴ�.
        public void PauseBGM(string name) {
            if (Enum.TryParse(name, out SoundID id) &&
                channelInfo.TryGetValue(id, out int channel))
            {
                Global_SoundManager.PauseBGM(channel);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // ��� ��������� �Ͻ������մϴ�.
        public void PauseBGMAll() {
            Global_SoundManager.PauseAll();
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� _fadeTime �ʰ� ���̵� �ƿ��ϸ� ������ �����մϴ�.
        public void StopBGM(string name, float fadeTime) {
            if (Enum.TryParse(name, out SoundID id) &&
                channelInfo.TryGetValue(id, out int channel))
            {
                Global_SoundManager.StopBGM(channel, fadeTime > 0 ? Global_SoundManager.SoundOption.FadeOut : Global_SoundManager.SoundOption.None, fadeTime);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // ��� ��������� ������ �����մϴ�.
        public void StopBGMAll() {
            Global_SoundManager.StopAll();
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetFlag(float index, bool state)
        {
            FlagID newFlag = (FlagID)Enum.GetValues(typeof(FlagID)).GetValue((int)index);
            if (state)
            {
                Current.flag |= newFlag;
            }
            else
            {
                Current.flag &= ~newFlag;
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetSprite(float index, string name)
        {
            if (Enum.TryParse(name, out SpriteID id) &&
                spriteData.TryGetValue(id, out Sprite sprite))
            {
                Global_EventSystem.UI.Call(UIEventID.World_���־�뺧UIĳ���ͼ���, (int)index, sprite);
            }
            else
            {
                Debug.Log(name + "��������Ʈ �����.");
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _index ��������Ʈ�� ���͸� _name ���� �����մϴ�.
        public void SetFilter(string name)
        {
            //MySpriteManager.Instance.SetFilter(_name);
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // ���ȭ���� _name ���� �����մϴ�.
        public void SetBackground(string name)
        {
            if (Enum.TryParse(name, out BackgroundID id) &&
                backgroundData.TryGetValue(id, out Sprite background))
            {
                Global_EventSystem.UI.Call(UIEventID.World_���־�뺧UI��漳��, background);
            }
            else
            {
                Debug.Log(name + "���ȭ�� �����.");
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void MakeTitle(string content)
        {
            Global_EventSystem.UI.Call<string, UnityAction, bool>(
                    UIEventID.World_����ƮUIŸ��Ʋ����,
                    content,
                    () => Global_EventSystem.VisualNovel.CallOnCommandEnd(),
                    isFirst);
            Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
            isOpening = true;
        }

        public void DontEraseLastSentence() {
            isClearDialogue = false;
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void Jump(float index) {
            Current.cursor = (int)index;
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetDateTime(string data)
        {
            Current.gameTime = data;
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void UnlockBook(params float[] index)
        {
            for (int i = 0; i < index.Length; ++i)
            {
                Current.book |= (BookID)Enum.GetValues(typeof(BookID)).GetValue((int)index[i]);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // Command ��� ���� ----------------------------------------------------------------------------------- 

        #endregion

        private void OnDestroy() {
            Global_EventSystem.Sound.onSoundEnd -= OnSoundEnd;
            Global_EventSystem.VisualNovel.onScreenTouched -= OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
            StopAllCoroutines();
        }
    }
}
