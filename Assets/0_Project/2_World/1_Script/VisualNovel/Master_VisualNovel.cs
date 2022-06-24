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
        private bool isDebug = false;

        public static Master_VisualNovel Instance { get; private set; } = null;

        /// <summary>
        /// ��ŵ�� ���¸� ��Ÿ���� ����. false�� �� ��ŵ�� ���� �ʴ� �����Դϴ�.
        /// </summary>
        private bool IsSkip => Global_EventSystem.VisualNovel.Skip;

        /// <summary>
        /// ������ ���¸� ��Ÿ���� ����. false�� �� �Ͻ����� �����Դϴ�.
        /// </summary>
        private bool GameState => Global_EventSystem.VisualNovel.GameState;

        private SaveData Current => Global_LocalData.Save.Current;

        private bool isFirst = true;
        private bool isOpening = false;
        private bool isClearDialogue = true;

        private bool isTouch = false;
        private bool isWaitForTouch = false;

        private WaitForSeconds waitForDelay;
        private WaitUntil waitForTouch;
        private WaitUntil waitForGameState;

        private ReadOnlyDictionary<string, int> IDData => VisualNovelLoader.IDData;
        private ReadOnlyCollection<ReadOnlyDictionary<string, object>> VNData => VisualNovelLoader.VNData;

        // Start is called before the first frame update
        void Start() {
            if (isDebug) StartCoroutine(DebugRoutine());
            else StartCoroutine(VisualNovelRoutine());
        }

        IEnumerator DebugRoutine() {
            yield return new WaitForEndOfFrame();
            // �ݹ� ���
            Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
            Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, OnSpeedValueChanged);

            // �̺�Ʈ ȣ��
            Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(true);
            Global_EventSystem.VisualNovel.CallOnLoad(Current);

            isFirst = true;
            isOpening = false;

            waitForDelay = new WaitForSeconds(Global_LocalData.Setting.Delay);
            waitForTouch = new WaitUntil(() => isTouch);
            waitForGameState = new WaitUntil(() => GameState);

            while (true) {
                string _id = VNData[Current.cursor][KEY_ID].ToString();
                if (string.IsNullOrEmpty(_id)) {
                    Debug.Log("VN DATA ��");
                    break;
                }
                Debug.Log($"[{Current.cursor}] [{_id}] �� ����");

                string cmdData = VNData[Current.cursor][KEY_COMMAND].ToString();
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
                                yield return waitForGameState;
                            }
                        }
                    }
                }
                if (isFirst && isOpening == false) {
                    Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
                }
                if (isFirst) isFirst = false;
                Current.cursor += 1;
            }
        }

        IEnumerator VisualNovelRoutine() {

            yield return new WaitForEndOfFrame();

            // �ݹ� ���
            Global_EventSystem.VisualNovel.onScreenTouched += OnClick;
            Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChanged;
            Global_EventSystem.VisualNovel.onSceneReloaded += OnSceneReloaded;
            Global_EventSystem.UI.Register(UIEventID.Global_����UIClose, OnSpeedValueChanged);

            // �̺�Ʈ ȣ��
            Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);
            Global_EventSystem.VisualNovel.CallOnLoad(Current);

            isFirst = true;
            isOpening = false;

            waitForDelay = new WaitForSeconds(Global_LocalData.Setting.Delay);
            waitForTouch = new WaitUntil(() => isTouch);
            waitForGameState = new WaitUntil(() => GameState);

            while (true) {
                string _id = VNData[Current.cursor][KEY_ID].ToString();
                if (string.IsNullOrEmpty(_id)) {
                    Debug.Log("VN DATA ��");
                    break;
                }

                Debug.Log($"[{Current.cursor}] [{_id}] �� ����");

                isTouch = false;
                isClearDialogue = true;

                string cmdData = VNData[Current.cursor][KEY_COMMAND].ToString();
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
                                yield return waitForGameState;
                            }
                        }
                    }
                }
                if (isFirst && isOpening == false) {
                    Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
                }

                ReadOnlyDictionary<string, object> currentData = VNData[Current.cursor];
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
                        yield return waitForTouch;
                        isWaitForTouch = false;
                    }
                }

                if (GameState == false) {
                    yield return waitForGameState;
                }

                if (isFirst) isFirst = false;

                Current.cursor += 1;

            }
        }

        #region �ݹ�

        private void OnClick() {
            if (isWaitForTouch) {
                isTouch = true;
            }
            else {
                if (IsSkip) {
                    Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);
                }
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
        private void OnSceneReloaded()
        {
            StopAllCoroutines();
            if (isDebug) StartCoroutine(DebugRoutine());
            else StartCoroutine(VisualNovelRoutine());
        }

        #endregion

        #region Command ��� ����

        public void Wait(float time) {
            if (IsSkip == false &&
                isFirst == false) {
                Sequence sequence = DOTween.Sequence();
                sequence.
                    AppendInterval(time).
                    OnStart(() => {
                        Global_EventSystem.VisualNovel.onSkipStateChanged += OnSkipStateChangedDuringWait;
                        Global_EventSystem.VisualNovel.onGameStateChanged += OnGameStateChangedDuringWait;
                    }).
                    OnComplete(() => Global_EventSystem.VisualNovel.CallOnCommandEnd()).
                    OnKill(() => {
                        Global_EventSystem.VisualNovel.onSkipStateChanged -= OnSkipStateChangedDuringWait;
                        Global_EventSystem.VisualNovel.onGameStateChanged -= OnGameStateChangedDuringWait;
                    }).
                    Play();

                void OnSkipStateChangedDuringWait(bool state) {
                    if (state) {
                        sequence.Complete();
                    }
                }

                void OnGameStateChangedDuringWait(bool state) {
                    if (state) {
                        sequence.Play();
                    }
                    else {
                        sequence.Pause();
                    }
                }
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
            else {
                Debug.LogError(Current.cursor + "�� " + name + " ȿ���� �����. �Լ�Ÿ�� : PlaySFX");
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void PlayBGM(string name) {
            if(Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PlayBGM(id, Global_SoundManager.SoundOption.Loop | Global_SoundManager.SoundOption.Only);
            }
            else {
                Debug.LogError(Current.cursor + "�� " + name + " ������� �����. �Լ�Ÿ�� : PlayBGM");
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void PauseBGM(string name) {
            if (Enum.TryParse(name, out SoundID id))
            {
                Global_SoundManager.PauseBGM(id);
            }
            else {
                Debug.LogError(Current.cursor + "�� " + name + " ������� �����. �Լ�Ÿ�� : PauseBGM");
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
            else {
                Debug.LogError(Current.cursor + "�� " + name + " ������� �����. �Լ�Ÿ�� : StopBGM");
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
                if (string.IsNullOrEmpty(name) == false) Debug.LogError(Current.cursor + "�� "+ name + " ��������Ʈ �����. �Լ�Ÿ�� : SetSprite");
                Global_EventSystem.UI.Call<int, Sprite>(UIEventID.World_���־�뺧UIĳ���ͼ���, (int)index, null);
            }
            Global_EventSystem.VisualNovel.CallOnCommandEnd();
        }

        public void SetFilter(string name)
        {
            if (Enum.TryParse(name, out FilterID id) &&
                FilterLoader.Data.TryGetValue(id, out Sprite sprite)) {
                Global_EventSystem.UI.Call(UIEventID.World_���־�뺧UI���ͼ���, sprite);
            }
            else {
                if (string.IsNullOrEmpty(name) == false) Debug.LogError(Current.cursor + "�� " + name + " ��������Ʈ �����. �Լ�Ÿ�� : SetFilter");
                Global_EventSystem.UI.Call<Sprite>(UIEventID.World_���־�뺧UI���ͼ���, null);
            }
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
                if (string.IsNullOrEmpty(name) == false) Debug.LogError(Current.cursor + "�� " + name + " ���ȭ�� �����. �Լ�Ÿ�� : SetBackground");
                Global_EventSystem.UI.Call<Sprite>(UIEventID.World_���־�뺧UI��漳��, null);
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
            if (time > 0 && isFirst == false) {
                Global_EventSystem.UI.Call<float, UnityAction>(
                    UIEventID.World_����ƮUI��鸲ȿ��,
                    IsSkip ? Global_LocalData.Setting.Delay : time,
                    () => Global_EventSystem.VisualNovel.CallOnCommandEnd());
            }
            else {
                Global_EventSystem.VisualNovel.CallOnCommandEnd();
            }
        }

        public void MakeEffect(string name, float time) {
            if (time > 0 && isFirst == false) {
                if (Enum.TryParse(name, out ParticleID id) &&
                    ParticleLoader.Data.TryGetValue(id, out GameObject target)) {
                    Global_EventSystem.UI.Call<GameObject, float, UnityAction>(
                        UIEventID.World_����ƮUI��ƼŬ����,
                        target,
                        IsSkip ? Global_LocalData.Setting.Delay : time,
                        () => Global_EventSystem.VisualNovel.CallOnCommandEnd());
                }
                else {
                    if (string.IsNullOrEmpty(name) == false) Debug.LogError(Current.cursor + "�� " + name + " ����Ʈ �����. �Լ�Ÿ�� : MakeEffect");
                }
            }
            else {
                Global_EventSystem.VisualNovel.CallOnCommandEnd();
            }
        }

        public void MakeSelection(string select1, string select2, string nextID1, string nextID2) {
            Global_EventSystem.VisualNovel.CallOnSkipStateChanged(false);
            if (isFirst) {
                isOpening = true;
                Global_EventSystem.UI.Call(UIEventID.World_����ƮUI����ȭ������);
            }

            if (IDData.TryGetValue(nextID1, out int id1) == false) {
                if (int.TryParse(nextID1, out id1) == false) {
                    Debug.Log("���ڰ� ���� : " + nextID1);
                }
            }

            if (IDData.TryGetValue(nextID2, out int id2) == false) {
                if (int.TryParse(nextID2, out id2) == false) {
                    Debug.Log("���ڰ� ���� : " + nextID2);
                }
            }

            Global_EventSystem.UI.Call(
                UIEventID.World_������UI����,
                new TPSelectionData[] {
                    new TPSelectionData(select1, () =>
                    {
                        Current.cursor = id1;
                        Global_EventSystem.VisualNovel.CallOnCommandEnd();
                    }),
                    new TPSelectionData(select2, () =>
                    {
                        Current.cursor = id2;
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

        public void GameOver() {
            Global_SoundManager.StopAll(Global_SoundManager.SoundOption.FadeOut, 1f);
            Scene.Global_SceneManager.LoadScene(Scene.SceneID.Title);
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
            Global_EventSystem.VisualNovel.onSceneReloaded -= OnSceneReloaded;
            StopAllCoroutines();
        }
    }
}
