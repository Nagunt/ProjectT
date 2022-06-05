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
        Invoke(nameof(Test), 0.1f);
    }

    void Test() {
        Global_EventSystem.UI.Call<string, UnityAction>(TP.UI.UIEventID.World_대화UI내용설정, test, () => {
            TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
            TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
            TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
            TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
            TP.Data.Global_LocalData.Save.Current.AddTPLogData(new TP.Data.TPLogData("name", test));
        });
    }

}
