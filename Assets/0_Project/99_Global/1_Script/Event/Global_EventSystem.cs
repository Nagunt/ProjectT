using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TP.UI;
using TP.Sound;
using TP.Data;
using TP.Scene;
using System;

namespace TP.Event {
    public static class Global_EventSystem {

        public static void Init() {
            Scene.Init();
            UI.Init();
        }

        public static class Scene {

            public delegate void OnSceneChangedByID(SceneID current, SceneID next);

            public static OnSceneChangedByID onSceneChanged;
            public static SceneID Current { get; private set; }

            public static void CallOnSceneChanged(SceneID current, SceneID next) {
                Debug.Log($"SceneChanged 호출 : {current} => {next}");
                onSceneChanged?.Invoke(current, next);
            }

            public static void Init() {
                Current = (SceneID)Enum.Parse(typeof(SceneID), SceneManager.GetActiveScene().name);
                SceneManager.activeSceneChanged += (a, b) => {
                    SceneID nextScene = (SceneID)Enum.Parse(typeof(SceneID), b.name);
                    CallOnSceneChanged(Current, nextScene);
                    Current = nextScene;
                };
            }
        }

        public static class Sound {
            public delegate void SoundEvent(SoundID id);
            public delegate void SoundEvent_ValueChanged(float value);

            public static SoundEvent onSoundStart;
            public static SoundEvent onSoundPause;
            public static SoundEvent onSoundEnd;

            public static SoundEvent_ValueChanged onBGMValueChanged;

            public static void CallOnSoundStart(SoundID id) {
                onSoundStart?.Invoke(id);
            }

            public static void CallOnSoundPause(SoundID id) {
                onSoundPause?.Invoke(id);
            }

            public static void CallOnSoundEnd(SoundID id) {
                onSoundEnd?.Invoke(id);
            }

            public static void CallOnBGMValueChanged(float value) {
                onBGMValueChanged?.Invoke(value);
            }
        }

        public static class VisualNovel {

            public delegate void VoidEvent();
            public delegate void BoolEvent(bool state);
            public delegate void LogEvent(TPLogData data);
            public delegate void SaveEvent(Global_LocalData.Save.SaveData saveData);

            public static SaveEvent onSave;
            public static SaveEvent onLoad;
            public static VoidEvent onCommandEnd;
            public static VoidEvent onScreenTouched;
            public static VoidEvent onSceneReloaded;
            public static BoolEvent onSkipStateChanged;
            public static BoolEvent onGameStateChanged;
            public static LogEvent onLogDataAdded;
            public static LogEvent onLogDataModified;

            public static bool Skip { get; private set; }
            public static bool GameState { get; private set; }

            public static void CallOnSave(Global_LocalData.Save.SaveData saveData) {
                onSave?.Invoke(saveData);
            }

            public static void CallOnLoad(Global_LocalData.Save.SaveData saveData) {
                onLoad?.Invoke(saveData);
            }

            public static void CallOnCommandEnd() {
                onCommandEnd?.Invoke();
            }

            public static void CallOnScreenTouched() {
                onScreenTouched?.Invoke();
            }

            public static void CallOnSceneReloaded()
            {
                onSceneReloaded?.Invoke();
            }

            public static void CallOnLogDataAdded(TPLogData data)
            {
                onLogDataAdded?.Invoke(data);
            }

            public static void CallOnLogDataModified(TPLogData data)
            {
                onLogDataModified?.Invoke(data);
            }

            public static void CallOnSkipStateChanged(bool state) {
                Skip = state;
                onSkipStateChanged?.Invoke(state);
            }

            public static void CallOnGameStateChanged(bool state) {
                GameState = state;
                Time.timeScale = GameState ? 1f : 0;
                onGameStateChanged?.Invoke(state);
            }
        }

        public static class UI {

            public delegate void UIEvent_0차원();
            public delegate void UIEvent_1차원<T>(T t);
            public delegate void UIEvent_2차원<T, K>(T t, K k);
            public delegate void UIEvent_3차원<T, K, L>(T t, K k, L l);

            private class UIEvent {
                public UIEvent_0차원 callback;
            }
            private class UIEvent<T> : UIEvent {
                public new UIEvent_1차원<T> callback;
            }

            private class UIEvent<T, K> : UIEvent {
                public new UIEvent_2차원<T, K> callback;
            }

            private class UIEvent<T, K, L> : UIEvent
            {
                public new UIEvent_3차원<T, K, L> callback;
            }

            private static Dictionary<UIEventID, UIEvent> localUIEvent_0;
            private static Dictionary<UIEventID, UIEvent> localUIEvent_1;
            private static Dictionary<UIEventID, UIEvent> localUIEvent_2;
            private static Dictionary<UIEventID, UIEvent> localUIEvent_3;


            private static Dictionary<UIEventID, UIEvent> globalUIEvent_0;
            private static Dictionary<UIEventID, UIEvent> globalUIEvent_1;
            private static Dictionary<UIEventID, UIEvent> globalUIEvent_2;
            private static Dictionary<UIEventID, UIEvent> globalUIEvent_3;



            public static void Call(UIEventID id) {
                Dictionary<UIEventID, UIEvent> targetDictionary;
                targetDictionary = globalUIEvent_0;
                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent).callback?.Invoke();
                }
                targetDictionary = localUIEvent_0;
                if (targetDictionary.TryGetValue(id, out UIEvent value2)) {
                    (value2 as UIEvent).callback?.Invoke();
                }
            }
            public static void Call<T>(UIEventID id, T t) {
                Dictionary<UIEventID, UIEvent> targetDictionary;
                targetDictionary = globalUIEvent_1;
                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent<T>).callback?.Invoke(t);
                }
                targetDictionary = localUIEvent_1;
                if (targetDictionary.TryGetValue(id, out UIEvent value2)) {
                    (value2 as UIEvent<T>).callback?.Invoke(t);
                }
            }
            public static void Call<T, K>(UIEventID id, T t, K k) {
                Dictionary<UIEventID, UIEvent> targetDictionary;
                targetDictionary = globalUIEvent_2;
                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent<T, K>).callback?.Invoke(t, k);
                }
                targetDictionary = localUIEvent_2;
                if (targetDictionary.TryGetValue(id, out UIEvent value2)) {
                    (value2 as UIEvent<T, K>).callback?.Invoke(t, k);
                }
            }
            public static void Call<T, K, L>(UIEventID id, T t, K k, L l)
            {
                Dictionary<UIEventID, UIEvent> targetDictionary;
                targetDictionary = globalUIEvent_3;
                if (targetDictionary.TryGetValue(id, out UIEvent value))
                {
                    (value as UIEvent<T, K, L>).callback?.Invoke(t, k, l);
                }
                targetDictionary = localUIEvent_3;
                if (targetDictionary.TryGetValue(id, out UIEvent value2))
                {
                    (value2 as UIEvent<T, K, L>).callback?.Invoke(t, k, l);
                }
            }

            public static void Register(UIEventID id, UIEvent_0차원 action, bool isGlobal = false) {
                Dictionary<UIEventID, UIEvent> targetDictionary;

                if (isGlobal) {
                    targetDictionary = globalUIEvent_0;
                }
                else {
                    targetDictionary = localUIEvent_0;
                }

                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent).callback += action;
                }
                else {
                    UIEvent newEvent = new UIEvent();
                    newEvent.callback += action;
                    targetDictionary.Add(id, newEvent);
                }
            }
            public static void Register<T>(UIEventID id, UIEvent_1차원<T> action, bool isGlobal = false) {
                Dictionary<UIEventID, UIEvent> targetDictionary;

                if (isGlobal) {
                    targetDictionary = globalUIEvent_1;
                }
                else {
                    targetDictionary = localUIEvent_1;
                }

                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent<T>).callback += action;
                }
                else {
                    UIEvent<T> newEvent = new UIEvent<T>();
                    newEvent.callback += action;
                    targetDictionary.Add(id, newEvent);
                }
            }
            public static void Register<T, K>(UIEventID id, UIEvent_2차원<T, K> action, bool isGlobal = false) {
                Dictionary<UIEventID, UIEvent> targetDictionary;

                if (isGlobal) {
                    targetDictionary = globalUIEvent_2;
                }
                else {
                    targetDictionary = localUIEvent_2;
                }

                if (targetDictionary.TryGetValue(id, out UIEvent value)) {
                    (value as UIEvent<T, K>).callback += action;
                }
                else {
                    UIEvent<T, K> newEvent = new UIEvent<T, K>();
                    newEvent.callback += action;
                    targetDictionary.Add(id, newEvent);
                }
            }
            public static void Register<T, K, L>(UIEventID id, UIEvent_3차원<T, K, L> action, bool isGlobal = false)
            {
                Dictionary<UIEventID, UIEvent> targetDictionary;

                if (isGlobal)
                {
                    targetDictionary = globalUIEvent_3;
                }
                else
                {
                    targetDictionary = localUIEvent_3;
                }

                if (targetDictionary.TryGetValue(id, out UIEvent value))
                {
                    (value as UIEvent<T, K, L>).callback += action;
                }
                else
                {
                    UIEvent<T, K, L> newEvent = new UIEvent<T, K, L>();
                    newEvent.callback += action;
                    targetDictionary.Add(id, newEvent);
                }
            }

            public static void Init() {
                localUIEvent_0 = new Dictionary<UIEventID, UIEvent>();
                localUIEvent_1 = new Dictionary<UIEventID, UIEvent>();
                localUIEvent_2 = new Dictionary<UIEventID, UIEvent>();
                localUIEvent_3 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_0 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_1 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_2 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_3 = new Dictionary<UIEventID, UIEvent>();

                Scene.onSceneChanged += (current, next) => {
                    localUIEvent_0.Clear();
                    localUIEvent_1.Clear();
                    localUIEvent_2.Clear();
                    localUIEvent_3.Clear();
                };
            }
        }

        public static class Input {
            public delegate void OnInputByClass(PlayerInput data);
            public delegate void OnInputByContext(InputAction.CallbackContext data);

            public static OnInputByClass onDeviceLost;
            public static OnInputByClass onDeviceRegained;
            public static OnInputByContext onInput;

            public static bool inputState;

            public static PlayerInput playerInput { get; private set; }

            public static void SetInput(PlayerInput target) {
                inputState = true;
                playerInput = target;
                playerInput.onActionTriggered += CallOnInput;
                playerInput.onDeviceLost += CallOnDeviceLost;
                playerInput.onDeviceRegained += CallOnDeviceRegained;
            }

            public static void CallOnInput(InputAction.CallbackContext data) {
                if (inputState) {
                    onInput?.Invoke(data);
                }
            }
            public static void CallOnDeviceLost(PlayerInput data) { onDeviceLost?.Invoke(data); }
            public static void CallOnDeviceRegained(PlayerInput data) { onDeviceRegained?.Invoke(data); }
        }
    }
}