using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP.Event;
using UnityEngine.Events;

public class dialogueTest : MonoBehaviour
{
    [TextArea(3, 5)]
    public string test;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());

        IEnumerator Test() {
            yield return new WaitForEndOfFrame();
            bool isComplete = false;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_��ȭUI���뼳��, test, () => isComplete = true);
            yield return new WaitUntil(() => isComplete);
            yield return new WaitForSeconds(2f);
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_��ȭUI�������, "ù��°��\\n�ι�°��\\n����°��\\n�׹�°��\\n�ټ���°��\\n", () => isComplete = true);
            yield return new WaitUntil(() => isComplete);
            yield break;
            Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_��ȭUI���뼳��, test, () => {
                TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
                TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
                TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
                TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
                TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
            });
        }
    }

}
