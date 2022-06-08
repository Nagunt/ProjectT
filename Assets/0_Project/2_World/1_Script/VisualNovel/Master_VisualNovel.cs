using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using TP.Data;
using UnityEngine;
using UnityEngine.Events;
using static TP.Data.Global_LocalData.Save;

namespace TP.VisualNovel {
    public class Master_VisualNovel : MonoBehaviour {

        [SerializeField]
        private BackgroundLoader backgroundLoader;
        [SerializeField]
        private SpriteLoader spriteLoader;
        [SerializeField]
        private CharacterLoader characterLoader;

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

        private int cursor;

        private WaitForSeconds waitForDelay;

        private List<Dictionary<string, object>> vnData;


        // Start is called before the first frame update
        void Start() {
            StartCoroutine(VisualNovelRoutine());
        }

        IEnumerator VisualNovelRoutine() {
            yield return new WaitForEndOfFrame();

            // �ݹ� ���
            Event.Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Event.Global_EventSystem.VisualNovel.onGameStateChanged += OnGameStateChanged;
            Event.Global_EventSystem.UI.Register(UI.UIEventID.Global_����UIClose, OnSpeedValueChanged);

            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Event.Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);

            // ������ �ε�
            if (Current.lastLogData.Check())
            {
                Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(Current.lastLogData);
                Event.Global_EventSystem.UI.Call(UI.UIEventID.World_��ȭUI�̸�����, Current.lastLogData.name);
                Event.Global_EventSystem.UI.Call<string, UnityAction>(UI.UIEventID.World_��ȭUI���밻��, Current.lastLogData.content, null);
            }

            Event.Global_EventSystem.UI.Call(UI.UIEventID.World_����UI�����ͼ���, characterLoader.ToDictionary());

            string id = "_id";
            string name = "name";
            string sentence = "sentence";
            string command = "cmd";

            isFirst = true;
            isOpening = false;

            Time.timeScale = 1f;
            waitForDelay = new WaitForSeconds(Data.Global_LocalData.Setting.Delay);

            while (true) {
                string _id = vnData[Current.cursor][id].ToString();
                if (string.IsNullOrEmpty(_id)) {
                    Debug.Log("VN DATA ��");
                    break;
                }

                Debug.Log($"{Current.cursor} �� ����");

                isTouch = false;
                isClearDialogue = true;

                string cmdData = vnData[Current.cursor][command].ToString();
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
                if (isFirst && isOpening == false) {
                    Debug.Log("������ �̽���, ���̵� �� �����ؾߵǴµ�..");
                    //MyEffectManager.Instance.MakeFade(0, 1f, true);
                }

                Dictionary<string, object> currentData = vnData[Current.cursor];
                string nameText = currentData[name].ToString();
                string contentText = currentData[sentence].ToString().Replace("\"\"", "\"");

                if (isClearDialogue) {
                    TPLogData newLogData = new Data.TPLogData(nameText, contentText);
                    Current.lastLogData = newLogData;
                    Event.Global_EventSystem.UI.Call(UI.UIEventID.World_��ȭUI�̸�����, nameText);
                    Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(newLogData);
                }
                else {
                    TPLogData lastLogData = Current.lastLogData;
                    lastLogData.content += contentText;
                    Current.lastLogData = lastLogData;
                    Event.Global_EventSystem.UI.Call(UI.UIEventID.World_��ȭUI�̸�����, lastLogData.name);
                    Event.Global_EventSystem.VisualNovel.CallOnLogDataModified(lastLogData);
                }

                if (string.IsNullOrEmpty(contentText) == false) {
                    bool isComplete = false;
                    Event.Global_EventSystem.UI.Call<string, UnityAction>(
                        isClearDialogue ? UI.UIEventID.World_��ȭUI���뼳�� : UI.UIEventID.World_��ȭUI�������, 
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
        private void OnGameStateChanged(bool state) {
            Time.timeScale = state ? 1f : 0;
        }
        private void OnSpeedValueChanged() {
            waitForDelay = new WaitForSeconds(Data.Global_LocalData.Setting.Delay);
        }

        private Sound.SoundID GetSoundID(string name) {
            if (Enum.TryParse(name, out Sound.SoundID soundID)) {
                return soundID;
            }
            return default;
        }

        #endregion

        #region Command ��� ����
        // Command ��� ���� -----------------------------------------------------------------------------------

        public void Wait(float _time) {
            if (IsSkip == false) {
                Sequence sequence = DOTween.Sequence();
                sequence.
                    AppendInterval(_time).
                    OnComplete(() => {
                        Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
                    });
            }
            else {
                Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
            }
        }

        // _name ȿ������ ����մϴ�.
        public void PlaySFX(string name) {
            //MyAudioManager.Instance.PlaySFX(_name);
            Sound.Global_SoundManager.PlaySFX(GetSoundID(name));
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� ����մϴ�.
        public void PlayBGM(string name) {
            Sound.Global_SoundManager.PlayBGM(GetSoundID(name), Sound.Global_SoundManager.SoundOption.Loop | Sound.Global_SoundManager.SoundOption.Only);
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� �Ͻ������մϴ�.
        public void PauseBGM(string name) {

            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // ��� ��������� �Ͻ������մϴ�.
        public void PauseBGMAll() {
            Sound.Global_SoundManager.PauseAll();
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name ��������� _fadeTime �ʰ� ���̵� �ƿ��ϸ� ������ �����մϴ�.
        public void StopBGM(string name, float fadeTime) {
            //MyAudioManager.Instance.StopBGM(_name, _fadeTime);
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // ��� ��������� ������ �����մϴ�.
        public void StopBGMAll() {
            Sound.Global_SoundManager.StopAll();
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void DontEraseLastSentence() {
            isClearDialogue = false;
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void Jump(float index) {
            Current.cursor = (int)index;
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void UnlockBook(params float[] index)
        {
            for (int i = 0; i < index.Length; ++i)
            {
                Current.book |= (BookID)Enum.GetValues(typeof(BookID)).GetValue((int)index[i]);
            }
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // Command ��� ���� ----------------------------------------------------------------------------------- 

        #endregion

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onScreenTouched -= OnClick;
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
            Event.Global_EventSystem.VisualNovel.onGameStateChanged -= OnGameStateChanged;
            StopAllCoroutines();
        }
    }
}
