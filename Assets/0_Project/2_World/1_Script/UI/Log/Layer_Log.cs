using System;
using System.Collections;
using System.Collections.Generic;
using TP.Data;
using TP.VisualNovel;
using UnityEngine;

namespace TP.UI {
    public class Layer_Log : Layer_Default<UI_Log> {

        [SerializeField]
        private int MAX_COUNT = 15;
        private Queue<TPLogData> logData;

        protected override void Start() {
            base.Start();
            logData = new Queue<TPLogData>();
            Event.Global_EventSystem.UI.Register(UIEventID.World_로그UIOpen, Open);
            Event.Global_EventSystem.UI.Register(UIEventID.World_로그UIClose, Close);
            Event.Global_EventSystem.VisualNovel.onLoad += OnLoad;
            Event.Global_EventSystem.VisualNovel.onLogDataAdded += OnLogDataAdded;
            Event.Global_EventSystem.VisualNovel.onLogDataModified += OnLogDataModified;
        }

        protected override void Open()
        {
            base.Open();
            if(_uiObject != null)
            {
                _uiObject.SetLogData(logData);
            }
            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(false);
        }

        protected override void Close()
        {
            base.Close();
            Event.Global_EventSystem.VisualNovel.CallOnGameStateChanged(true);
        }
        private void OnLoad(Global_LocalData.Save.SaveData saveData) {
            if (saveData.lastLogData.Check()) {
                Event.Global_EventSystem.VisualNovel.CallOnLogDataAdded(saveData.lastLogData);
            }
        }

        private void OnLogDataAdded(TPLogData data)
        {
            if (data.Check())
            {
                if (logData.Count >= MAX_COUNT)
                {
                    logData.Dequeue();
                }
                logData.Enqueue(data);
            }
        }

        private void OnLogDataModified(TPLogData data)
        {
            if (data.Check())
            {
                TPLogData[] tempData = new TPLogData[logData.Count];
                logData.CopyTo(tempData, 0);
                logData.Clear();
                for (int i = 0; i < tempData.Length; ++i)
                {
                    logData.Enqueue(i < tempData.Length - 1 ? tempData[i] : data);
                }
            }
        }

        private void OnDestroy()
        {
            Event.Global_EventSystem.VisualNovel.onLoad -= OnLoad;
            Event.Global_EventSystem.VisualNovel.onLogDataAdded -= OnLogDataAdded;
            Event.Global_EventSystem.VisualNovel.onLogDataModified -= OnLogDataModified;
        }
    }
}

