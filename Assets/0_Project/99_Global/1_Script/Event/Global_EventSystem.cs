using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TP.UI;
using TP.Sound;

namespace TP.Event {
    public static class Global_EventSystem {

        public static void Init() {
            Scene.Init();
            UI.Init();
        }

        public static class Scene {

            public delegate void OnSceneChangedByName(string current, string next);

            public static OnSceneChangedByName onSceneChanged;

            public static void CallOnSceneChanged(string currentSceneName, string nextSceneName) {
                Debug.Log($"SceneChanged 호출 : {currentSceneName} => {nextSceneName}");
                onSceneChanged?.Invoke(currentSceneName, nextSceneName);
            }

            public static void Init() {
                SceneManager.activeSceneChanged += (a, b) => {
                    Debug.Log($"ActiveSceneChanged 호출 : {a} => {b}");
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

            public static VoidEvent onCommandEnd;
            public static VoidEvent onScreenTouched;
            public static BoolEvent onSkipStateChanged;
            public static BoolEvent onGameStateChanged;

            public static bool Skip { get; private set; }
            public static bool GameState { get; private set; }

            public static void CallOnCommandEnd() {
                onCommandEnd?.Invoke();
            }

            public static void CallOnScreenTouched() {
                onScreenTouched?.Invoke();
            }

            public static void CallOnSkipStateChanged(bool state) {
                Skip = state;
                onSkipStateChanged?.Invoke(state);
            }

            public static void CallOnGameStateChanged(bool state) {
                GameState = state;
                onGameStateChanged?.Invoke(state);
            }
        }

        public static class UI {

            public delegate void UIEvent_0차원();
            public delegate void UIEvent_1차원<T>(T t);
            public delegate void UIEvent_2차원<T, K>(T t, K k);

            private class UIEvent {
                public UIEvent_0차원 callback;
            }
            private class UIEvent<T> : UIEvent {
                public new UIEvent_1차원<T> callback;
            }

            private class UIEvent<T, K> : UIEvent {
                public new UIEvent_2차원<T, K> callback;
            }

            private static Dictionary<UIEventID, UIEvent> localUIEvent_0;
            private static Dictionary<UIEventID, UIEvent> localUIEvent_1;
            private static Dictionary<UIEventID, UIEvent> localUIEvent_2;


            private static Dictionary<UIEventID, UIEvent> globalUIEvent_0;
            private static Dictionary<UIEventID, UIEvent> globalUIEvent_1;
            private static Dictionary<UIEventID, UIEvent> globalUIEvent_2;

            

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

            public static void Init() {
                localUIEvent_0 = new Dictionary<UIEventID, UIEvent>();
                localUIEvent_1 = new Dictionary<UIEventID, UIEvent>();
                localUIEvent_2 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_0 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_1 = new Dictionary<UIEventID, UIEvent>();
                globalUIEvent_2 = new Dictionary<UIEventID, UIEvent>();

                Scene.onSceneChanged += (current, next) => {
                    localUIEvent_0.Clear();
                    localUIEvent_1.Clear();
                    localUIEvent_2.Clear();
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