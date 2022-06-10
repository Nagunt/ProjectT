using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Event;
using UnityEngine.Events;
using TP.VisualNovel;
using TP.UI;
using System;
using TP.Data;
using System.Collections.ObjectModel;

public class dialogueTest : MonoBehaviour
{
    public SpriteLoader spriteLoader;
    public CharacterLoader characterLoader;
    [TextArea(3, 5)]
    public string test;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());

        IEnumerator Test() {
            yield return new WaitForEndOfFrame();
            Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);

            ReadOnlyDictionary<SpriteID, Sprite> spriteData = spriteLoader.ToData();

            TP.Sound.Global_SoundManager.PlayBGM(TP.Sound.SoundID.bgm01, TP.Sound.Global_SoundManager.SoundOption.FadeIn | TP.Sound.Global_SoundManager.SoundOption.Loop);

            UnlockBook(1, 2, 3, 4);
            Global_EventSystem.UI.Call<int, Sprite>(UIEventID.World_비주얼노벨UI캐릭터설정, 0, spriteData[SpriteID.adel_angry_1]);
            Global_EventSystem.UI.Call<int, Sprite>(UIEventID.World_비주얼노벨UI캐릭터설정, 1, spriteData[SpriteID.adel_angry_1]);
            Global_EventSystem.UI.Call<int, Sprite>(UIEventID.World_비주얼노벨UI캐릭터설정, 2, spriteData[SpriteID.adel_angry_1]);

            bool isComplete = false;

            Global_EventSystem.UI.Call<string, UnityAction, bool>(UIEventID.World_이펙트UI타이틀생성, "테스트 타이틀입니다.", () => isComplete = true, true);
            yield return new WaitUntil(() => isComplete);
            
            isComplete = false;
            Global_EventSystem.UI.Call<TPSelectionData[]>(UIEventID.World_선택지UI생성, new TPSelectionData[] {
                new TPSelectionData("선택지1", () =>
                {
                    Debug.Log("선택지1을 골랐습니다");
                    isComplete = true;
                }),
                new TPSelectionData("선택지2", () =>
                {
                    Debug.Log("선택지2을 골랐습니다");
                    isComplete = true;
                }),
                new TPSelectionData("선택지3", () =>
                {
                    Debug.Log("선택지3을 골랐습니다");
                    isComplete = true;
                }),
                new TPSelectionData("선택지4", () =>
                {
                    Debug.Log("선택지4을 골랐습니다");
                    isComplete = true;
                }),
            });
            Global_EventSystem.UI.Call(UIEventID.World_이펙트UI검은화면해제);
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_대화UI내용설정, test, () => isComplete = true);
            Global_EventSystem.VisualNovel.CallOnLogDataAdded(new TP.Data.TPLogData("테스트", test));
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<float, UnityAction>(TP.UI.UIEventID.World_이펙트UI흔들림효과, 5f, () => isComplete = true);
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_대화UI내용수정, "첫번째줄\\n두번째줄\\n세번째줄\\n네번째줄\\n다섯번째줄\\n", () => isComplete = true);
            Global_EventSystem.VisualNovel.CallOnLogDataModified(new TP.Data.TPLogData("테스트2", test + "첫번째줄\\n두번째줄\\n세번째줄\\n네번째줄\\n다섯번째줄\\n끝"));
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction, bool>(UIEventID.World_이펙트UI타이틀생성, "두번째 테스트 타이틀입니다.", () => isComplete = true, false);
            yield return new WaitUntil(() => isComplete);
        }
    }

    public void UnlockBook(params float[] index)
    {
        for (int i = 0; i < index.Length; ++i)
        {
            TP.Data.Global_LocalData.Save.Current.book |= (TP.Data.Global_LocalData.Save.BookID)Enum.GetValues(typeof(TP.Data.Global_LocalData.Save.BookID)).GetValue((int)index[i]);
        }
    }

}
