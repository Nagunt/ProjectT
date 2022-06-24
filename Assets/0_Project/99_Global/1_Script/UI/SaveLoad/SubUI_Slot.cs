using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace TP.UI {
    public class SubUI_Slot : MonoBehaviour {
        [SerializeField]
        private int m_slot;
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private Button m_Button;
        [SerializeField]
        private TextMeshProUGUI m_gameTimeText;
        [SerializeField]
        private TextMeshProUGUI m_realTimeText;

        private void SetInfo(string gameTime, string realTime) {
            m_gameTimeText.SetText(gameTime);
            m_realTimeText.SetText(realTime);
        }

        private void SetThumbnail(Texture2D source) {
            if (source == null) {
                m_Image.color = Color.black;
            }
            else {
                m_Image.color = Color.white;
                m_Image.sprite = Sprite.Create(source, new Rect(0, 0, source.width, source.height), new Vector2(0.5f, 0.5f));
            }
        }

        public void OnClick(bool isSave) {
            if (isSave) {
                // 여기서 저장하던지
                Data.Global_LocalData.Save.SaveData saveData = Data.Global_LocalData.Save.Current;
                saveData.Save(m_slot);
                SetInfo(saveData.gameTime, saveData.realTime);
                SetThumbnail(saveData.ThumbNail);
                Canvas.ForceUpdateCanvases();
            }
            else {
                if (Data.Global_LocalData.Save.Check(m_slot)) {
                    Debug.Log($"{m_slot}번 세이브 파일 로드");
                    if (Event.Global_EventSystem.Scene.Current.Equals(Scene.SceneID.World))
                    {
                        Event.Global_EventSystem.UI.Call(UIEventID.Global_터치잠금설정);
                        Sound.Global_SoundManager.StopAll(Sound.Global_SoundManager.SoundOption.FadeOut, 1f);
                        Event.Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeOut, 1f, () => {
                            Data.Global_LocalData.Save.Load(m_slot);
                            Event.Global_EventSystem.VisualNovel.CallOnSceneReloaded();
                            Event.Global_EventSystem.UI.Call<float, UnityAction>(UIEventID.Global_FadeIn, 1f, () => {
                                Event.Global_EventSystem.UI.Call(UIEventID.Global_터치잠금해제);
                            });
                        });
                    }
                    else
                    {
                        Sound.Global_SoundManager.StopAll(Sound.Global_SoundManager.SoundOption.FadeOut, 1f);
                        Data.Global_LocalData.Save.Load(m_slot);
                        Scene.Global_SceneManager.LoadSceneAsync(Scene.SceneID.World, 1f);
                    }
                }
            }
        }

        public void Init(bool isSave) {
            if (Data.Global_LocalData.Save.Check(m_slot)) {
                Data.Global_LocalData.Save.Load(m_slot, out Data.Global_LocalData.Save.SaveData saveData);
                SetInfo(saveData.gameTime, saveData.realTime);
                SetThumbnail(saveData.ThumbNail);
                m_Button.interactable = true;
                m_Button.onClick.AddListener(() => { OnClick(isSave); });
            }
            else {
                SetInfo(string.Empty, string.Empty);
                SetThumbnail(null);
                m_Button.interactable = isSave;
                m_Button.onClick.AddListener(() => { OnClick(isSave); });
            }
            m_Button.GetComponent<RectTransform>().SetShapeWithCurrentAspectRatio(Global_CameraCrop.ScreenRatio);
        }
    }
}

