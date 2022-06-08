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
        /// 스킵의 상태를 나타내는 변수. false일 시 스킵이 되지 않는 상태입니다.
        /// </summary>
        public bool IsSkip {
            get {
                return Event.Global_EventSystem.VisualNovel.Skip;
            }
        }

        /// <summary>
        /// 게임의 상태를 나타내는 변수. false일 시 일시정지 상태입니다.
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

            // 콜백 등록
            Event.Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Event.Global_EventSystem.VisualNovel.onGameStateChanged += OnGameStateChanged;
            Event.Global_EventSystem.UI.Register(UI.UIEventID.Global_설정UIClose, OnSpeedValueChanged);

            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Event.Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);

            // 데이터 로드
            if (Current.lastLogData.Check())
            {
                Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(Current.lastLogData);
                Event.Global_EventSystem.UI.Call(UI.UIEventID.World_대화UI이름설정, Current.lastLogData.name);
                Event.Global_EventSystem.UI.Call<string, UnityAction>(UI.UIEventID.World_대화UI내용갱신, Current.lastLogData.content, null);
            }

            Event.Global_EventSystem.UI.Call(UI.UIEventID.World_도감UI데이터설정, characterLoader.ToDictionary());

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
                    Debug.Log("VN DATA 끝");
                    break;
                }

                Debug.Log($"{Current.cursor} 행 실행");

                isTouch = false;
                isClearDialogue = true;

                string cmdData = vnData[Current.cursor][command].ToString();
                if (string.IsNullOrEmpty(cmdData) == false) {
                    List<TPCommand> cmds = TPCommand.Build(cmdData);
                    for (int i = 0; i < cmds.Count; ++i) {
                        Debug.Log("실행: " + cmds[i].ToString());
                        cmds[i].Execute(this);
                        if (TPCommand.runningCmd != null) {
                            yield return new WaitUntil(() => TPCommand.runningCmd.IsEnd);
                            TPCommand.runningCmd = null;
                            yield return new WaitUntil(() => GameState);
                        }
                    }
                }
                if (isFirst && isOpening == false) {
                    Debug.Log("오프닝 미실행, 페이드 인 실행해야되는데..");
                    //MyEffectManager.Instance.MakeFade(0, 1f, true);
                }

                Dictionary<string, object> currentData = vnData[Current.cursor];
                string nameText = currentData[name].ToString();
                string contentText = currentData[sentence].ToString().Replace("\"\"", "\"");

                if (isClearDialogue) {
                    TPLogData newLogData = new Data.TPLogData(nameText, contentText);
                    Current.lastLogData = newLogData;
                    Event.Global_EventSystem.UI.Call(UI.UIEventID.World_대화UI이름설정, nameText);
                    Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(newLogData);
                }
                else {
                    TPLogData lastLogData = Current.lastLogData;
                    lastLogData.content += contentText;
                    Current.lastLogData = lastLogData;
                    Event.Global_EventSystem.UI.Call(UI.UIEventID.World_대화UI이름설정, lastLogData.name);
                    Event.Global_EventSystem.VisualNovel.CallOnLogDataModified(lastLogData);
                }

                if (string.IsNullOrEmpty(contentText) == false) {
                    bool isComplete = false;
                    Event.Global_EventSystem.UI.Call<string, UnityAction>(
                        isClearDialogue ? UI.UIEventID.World_대화UI내용설정 : UI.UIEventID.World_대화UI내용수정, 
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

        #region 콜백

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

        #region Command 기능 구현
        // Command 기능 구현 -----------------------------------------------------------------------------------

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

        // _name 효과음을 재생합니다.
        public void PlaySFX(string name) {
            //MyAudioManager.Instance.PlaySFX(_name);
            Sound.Global_SoundManager.PlaySFX(GetSoundID(name));
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name 배경음악을 재생합니다.
        public void PlayBGM(string name) {
            Sound.Global_SoundManager.PlayBGM(GetSoundID(name), Sound.Global_SoundManager.SoundOption.Loop | Sound.Global_SoundManager.SoundOption.Only);
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name 배경음악을 일시정지합니다.
        public void PauseBGM(string name) {

            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // 모든 배경음악을 일시정지합니다.
        public void PauseBGMAll() {
            Sound.Global_SoundManager.PauseAll();
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // _name 배경음악을 _fadeTime 초간 페이드 아웃하며 완전히 정지합니다.
        public void StopBGM(string name, float fadeTime) {
            //MyAudioManager.Instance.StopBGM(_name, _fadeTime);
            Event.Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        // 모든 배경음악을 완전히 정지합니다.
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

        // Command 기능 구현 ----------------------------------------------------------------------------------- 

        #endregion

        private void OnDestroy() {
            Event.Global_EventSystem.VisualNovel.onScreenTouched -= OnClick;
            Event.Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChanged;
            Event.Global_EventSystem.VisualNovel.onGameStateChanged -= OnGameStateChanged;
            StopAllCoroutines();
        }
    }
}
