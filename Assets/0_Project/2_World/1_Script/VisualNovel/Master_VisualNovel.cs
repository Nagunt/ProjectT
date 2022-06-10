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
                return Global_EventSystem.VisualNovel.Skip;
            }
        }

        /// <summary>
        /// ������ ���¸� ��Ÿ���� ����. false�� �� �Ͻ����� �����Դϴ�.
        /// </summary>
        public bool GameState {
            get {
                return Global_EventSystem.VisualNovel.GameState;
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

        // Start is called before the first frame update
        void Start() {
            StartCoroutine(VisualNovelRoutine());
        }

        IEnumerator VisualNovelRoutine() {

            yield return new WaitForEndOfFrame();

            // �ݹ� ���
            Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, OnSpeedValueChanged);

            // �̺�Ʈ ȣ��
            Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);
            Global_EventSystem.VisualNovel.CallOnLoad(Current);

            // ������ �ε�
            vnData = vnLoader.ToData();

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
                            if (TPCommand.runningCmd.IsEnd == false) {
                                yield return new WaitUntil(() => TPCommand.runningCmd.IsEnd);
                            }
                            TPCommand.runningCmd = null;
                            if (GameState == false) {
                                yield return new WaitUntil(() => GameState);
                            }
                        }
                    }
                }
                if (isFirst && isOpening == false) {
                    Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
                }

                ReadOnlyDictionary<string, object> currentData = vnData[Current.cursor];
                string nameText = currentData[KEY_NAME].ToString();
                string contentText = currentData[KEY_SENTENCE].ToString().Replace("\"\"", "\"");

                if (isClearDialogue) {
                    TPLogData newLogData = new TPLogData(nameText, contentText);
                    Current.lastLogData = newLogData;
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI�̸�����, nameText);
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI��å����, CharacterLoader.GetPlacement(nameText));
                    if (isFirst) {
                        Global_EventSystem.VisualNovel.CallOnLogDataModified(newLogData);
                    }
                    else {
                        Global_EventSystem.VisualNovel.CallOnLogDataAdded(newLogData);
                    }
                }
                else {
                    TPLogData lastLogData = Current.lastLogData;
                    lastLogData.content += contentText;
                    Current.lastLogData = lastLogData;
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI�̸�����, lastLogData.name);
                    Global_EventSystem.UI.Call(UIEventID.World_��ȭUI��å����, CharacterLoader.GetPlacement(lastLogData.name));
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

        public void PlaySFX(string name) {
            if (Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PlaySFX(id);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void PlayBGM(string name) {
            if(Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PlayBGM(id, Global_SoundManager.SoundOption.Loop | Global_SoundManager.SoundOption.Only);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void PauseBGM(string name) {
            if (Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PauseBGM(id);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void PauseBGMAll() {
            Global_SoundManager.PauseAll();
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void StopBGM(string name, float fadeTime) {
            if (Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.StopBGM(id, fadeTime > 0 ? Global_SoundManager.SoundOption.FadeOut : Global_SoundManager.SoundOption.None, fadeTime);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

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
                SpriteLoader.Data.TryGetValue(id, out Sprite sprite))
            {
                Global_EventSystem.UI.Call(UIEventID.World_���־�뺧UIĳ���ͼ���, (int)index, sprite);
            }
            else
            {
                Debug.Log(name + "��������Ʈ �����.");
                Global_EventSystem.UI.Call<int, Sprite>(UIEventID.World_���־�뺧UIĳ���ͼ���, (int)index, null);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetFilter(string name)
        {
            //MySpriteManager.Instance.SetFilter(_name);
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetBackground(string name)
        {
            if (Enum.TryParse(name, out BackgroundID id) &&
                BackgroundLoader.Data.TryGetValue(id, out Sprite background))
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

        public void MakeShake(float time) {
            if (time > 0) {
                Global_EventSystem.UI.Call<float, UnityAction>(
                    UIEventID.World_����ƮUI��鸲ȿ��,
                    IsSkip ? Global_LocalData.Setting.Delay : time,
                    () => Global_EventSystem.VisualNovel.CallOnCommandEnd());
            }
            else {
                Global_EventSystem.VisualNovel.CallOnCommandEnd();
            }
        }

        public void MakeSelection(string select1, string select2, float nextID1, float nextID2) {
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);

            Global_EventSystem.UI.Call(
                UIEventID.World_������UI����,
                new TPSelectionData[] {
                    new TPSelectionData(select1, () =>
                    {
                        Current.cursor = (int)nextID1;
                        Global_EventSystem.VisualNovel.CallOnCommandEnd();
                    }),
                    new TPSelectionData(select2, () =>
                    {
                        Current.cursor = (int)nextID2;
                        Global_EventSystem.VisualNovel.CallOnCommandEnd();
                    })
            });

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
            Global_EventSystem.VisualNovel.onScreenTouched -= OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
            StopAllCoroutines();
        }
    }
}
