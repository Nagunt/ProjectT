using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Event;
using UnityEngine.Events;
using TP.VisualNovel;
using TP.UI;
using System;

public class dialogueTest : MonoBehaviour
{
    public CharacterLoader characterLoader;
    [TextArea(3, 5)]
    public string test;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());

        IEnumerator Test() {
            yield return new WaitForEndOfFrame();

            TP.Sound.Global_SoundManager.PlayBGM(TP.Sound.SoundID.bgm01, TP.Sound.Global_SoundManager.SoundOption.FadeIn | TP.Sound.Global_SoundManager.SoundOption.Loop);

            Global_EventSystem.UI.Call(UIEventID.World_도감UI데이터설정, characterLoader.ToDictionary());
            UnlockBook(1, 2, 3, 4);
            bool isComplete = false;

            Global_EventSystem.UI.Call<string, UnityAction, bool>(UIEventID.World_이펙트UI타이틀생성, "테스트 타이틀입니다.", () => isComplete = true, true);
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_대화UI내용설정, test, () => isComplete = true);
            Global_EventSystem.VisualNovel.CallOnLogDataAdded(new TP.Data.TPLogData("테스트", test));
            yield return new WaitUntil(() => isComplete);
            yield return new WaitForSeconds(2f);
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
