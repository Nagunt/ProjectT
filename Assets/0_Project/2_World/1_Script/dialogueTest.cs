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

            Global_EventSystem.UI.Call(UIEventID.World_����UI�����ͼ���, characterLoader.ToDictionary());
            UnlockBook(1, 2, 3, 4);
            bool isComplete = false;

            Global_EventSystem.UI.Call<string, UnityAction, bool>(UIEventID.World_����ƮUIŸ��Ʋ����, "�׽�Ʈ Ÿ��Ʋ�Դϴ�.", () => isComplete = true, true);
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_��ȭUI���뼳��, test, () => isComplete = true);
            Global_EventSystem.VisualNovel.CallOnLogDataAdded(new TP.Data.TPLogData("�׽�Ʈ", test));
            yield return new WaitUntil(() => isComplete);
            yield return new WaitForSeconds(2f);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_��ȭUI�������, "ù��°��\\n�ι�°��\\n����°��\\n�׹�°��\\n�ټ���°��\\n", () => isComplete = true);
            Global_EventSystem.VisualNovel.CallOnLogDataModified(new TP.Data.TPLogData("�׽�Ʈ2", test + "ù��°��\\n�ι�°��\\n����°��\\n�׹�°��\\n�ټ���°��\\n��"));
            yield return new WaitUntil(() => isComplete);
            isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction, bool>(UIEventID.World_����ƮUIŸ��Ʋ����, "�ι�° �׽�Ʈ Ÿ��Ʋ�Դϴ�.", () => isComplete = true, false);
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
