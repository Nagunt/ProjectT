namespace TP.UI {
    // 네이밍 규칙
    // (Scene 이름)_(UI이름)UI(Action)
    // 2개 이상의 Scene에서 사용될 이벤트의 경우 Global이라 지칭
    public enum UIEventID {

        Global_설정UIOpen,
        Global_설정UIClose,

        Global_저장UIOpen,
        Global_저장UIClose,

        Global_로드UIOpen,
        Global_로드UIClose,

        Global_툴팁UIOpen,
        Global_툴팁UIClose,

        Global_FadeIn,
        Global_FadeOut,

        Global_터치잠금설정,
        Global_터치잠금해제,

        Global_로딩UIOpen,
        Global_로딩UIClose,
        Global_로딩UI진행도설정,

        Title_제작진UIOpen,
        Title_제작진UIClose,

        World_대화UI이름설정,
        World_대화UI직책설정,
        World_대화UI내용설정,
        World_대화UI내용수정,

        World_비주얼노벨UI배경설정,
        World_비주얼노벨UI필터설정,
        World_비주얼노벨UI캐릭터설정,

        World_로그UIOpen,
        World_로그UIClose,

        World_도감UIOpen,
        World_도감UIClose,
        World_도감UI인덱스설정,

        World_이펙트UI검은화면설정,
        World_이펙트UI검은화면해제,

        World_이펙트UI타이틀생성,
        World_이펙트UI파티클생성,
        World_이펙트UI흔들림효과,

        World_선택지UI생성,
        World_선택지UI선택완료,

        World_정지메뉴UIOpen,
        World_정지메뉴UIClose,

    }
}