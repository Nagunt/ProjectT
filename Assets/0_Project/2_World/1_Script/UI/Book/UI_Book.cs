using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using TP.VisualNovel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static TP.Data.Global_LocalData.Save;

namespace TP.UI {

    public class UI_Book : UI_Default {
        [Header("- Book")]
        [SerializeField]
        private Image image_Profile;
        [SerializeField]
        private Image image_Rank;
        [SerializeField]
        private TextMeshProUGUI text_Where;
        [SerializeField]
        private TextMeshProUGUI text_Name;
        [SerializeField]
        private TextMeshProUGUI text_Birth;
        [SerializeField]
        private TextMeshProUGUI text_Placement;
        [SerializeField]
        private Button[] button_Slot;

        private int cursor = 1;
        private List<int> buttonIndex;
        private Vector2 aspectRatio = new Vector2(7, 9);
        private ReadOnlyDictionary<CharacterID, CharacterData> charData;

        protected override void Start() {
            charData = CharacterLoader.Data;
            image_Profile.rectTransform.SetShapeWithCurrentAspectRatio(aspectRatio);
            image_Rank.rectTransform.SetShapeWithCurrentAspectRatio(aspectRatio);
            for(int i = 0; i < button_Slot.Length; ++i) {
                button_Slot[i].GetComponent<RectTransform>().SetShapeWithCurrentAspectRatio(aspectRatio);
            }
            button_Slot[0].onClick.AddListener(() => { OnClick_Slot(0); });
            button_Slot[1].onClick.AddListener(() => { OnClick_Slot(1); });
            button_Slot[2].onClick.AddListener(() => { OnClick_Slot(2); });
            button_Slot[3].onClick.AddListener(() => { OnClick_Slot(3); });
            button_Slot[4].onClick.AddListener(() => { OnClick_Slot(4); });
            button_Slot[5].onClick.AddListener(() => { OnClick_Slot(5); });
            buttonIndex = new List<int>();
            for(int i = 0; i < 6; ++i)
            {
                buttonIndex.Add(i + 2);
            }
            if (Current.book.HasFlag((BookID)System.Enum.GetValues(typeof(BookID)).GetValue(cursor)))
                SetInfo(cursor);
            else
                SetInfo(0);
        }

        public override void Dispose(UnityAction onEndDispose)
        {
            Event.Global_EventSystem.UI.Call(UIEventID.World_도감UI인덱스설정, cursor);
            base.Dispose(onEndDispose);
        }

        private void OnClick_Slot(int index) {
            if (Data.Global_LocalData.Save.Current.book.HasFlag((BookID)System.Enum.GetValues(typeof(BookID)).GetValue(buttonIndex[index])))
            {
                SetInfo(buttonIndex[index]);
            }
        }

        private void SetInfo(int index)
        {
            CharacterID charID = (CharacterID)index;
            if(charData.TryGetValue(charID, out CharacterData data))
            {
                text_Where.SetText(data.where);
                text_Birth.SetText(data.birth);
                text_Name.SetText(data.name);
                text_Placement.SetText(data.placement);
                image_Rank.sprite = data.rank;
                image_Profile.sprite = data.profile;

                image_Rank.rectTransform.SetShapeWithCurrentAspectRatio(aspectRatio);

                if (data.rank != null) {
                    image_Rank.gameObject.SetActive(true);

                    Vector2 rankSize = new Vector2(image_Rank.rectTransform.rect.width, image_Rank.rectTransform.rect.height);
                    Vector2 sourceSize = new Vector2(data.rank.rect.width, data.rank.rect.height);

                    Vector2 rankAspectRatio = new Vector2(1, rankSize.y / rankSize.x);
                    Vector2 sourceAspectRatio = new Vector2(1, sourceSize.y / sourceSize.x);

                    if (sourceAspectRatio.y > rankAspectRatio.y) {
                        // 이미지의 y비율이 공간의 y비율보다 크다, 즉 x방향으로 두껍다
                        float fixedWidth = rankSize.y * (1 / sourceAspectRatio.y);
                        image_Rank.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fixedWidth);
                    }
                    else {
                        // 이미지의 y비율이 공간의 y비율보다 작다, 즉 y방향으로 두껍다
                        float fixedHeight = rankSize.x * sourceAspectRatio.y;
                        image_Rank.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -image_Rank.rectTransform.offsetMax.y, fixedHeight);
                    }
                }
                else {
                    image_Rank.gameObject.SetActive(false);
                }

                if (index > 0) cursor = index;

                buttonIndex.Clear();
                int k = 0;
                for (int i = 1; i < charData.Count; ++i) {
                    if (index == i || k >= button_Slot.Length) continue;
                    buttonIndex.Add(i);
                    if (Data.Global_LocalData.Save.Current.book.HasFlag((BookID)System.Enum.GetValues(typeof(BookID)).GetValue(i))) {
                        button_Slot[k].image.sprite = charData[(CharacterID)i].profile;
                    }
                    else {
                        button_Slot[k].image.sprite = charData[CharacterID.None].profile;
                    }
                    k += 1;
                }
            }
        }

        public void SetCursor(int data)
        {
            cursor = data;
        }
    }
}

