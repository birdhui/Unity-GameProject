using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 게임의 주요 흐름과 UI, 대사, 결과 화면 등을 관리하는 클래스
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 대사 텍스트를 표시할 UI 텍스트 컴포넌트
    public Text talkText;

    // 대사 풍선 이미지를 표시할 오브젝트
    public GameObject balloonImage;

    // 하트 UI(예: 생명 표시 등)를 관리하는 CanvasGroup
    public CanvasGroup heartUIGroup;

    [Header("UI 설정")]
    // 결과 UI(게임 종료 또는 성공 시 표시)
    public GameObject resultUI;

    // 게임 내에서 표시할 대사 목록
    private List<string> talks = new List<string>
    {
        "아 벌써 월요일이야 ㅠㅠ",
        "지각하면 안되니까 얼른 가자!",
        "게임 방법은 간단해!",
        "WASD나 방향키로 조작이 가능하고, \nW키나 ↑키를 연타하면 달릴 수 있어.",
        "Q키로 시작지점으로 돌아갈 수 있어. 참고해~",
        "방해물은 스페이스바로 JUMP!",
        "버스 도착까지 시간 내에 \n정류장에 도착하지 않으면 지각이야~!",
        "달리는 차에 3번 닿으면\n강의 시간 내에 도착 못해 ㅠㅠ",
        "참고로 차에 부딪히면 처음 장소로 되돌아가니까 조심해",
        "아 뭐야 버스 오는 거 같은데???;;;;",
        "달려!!!!!"
    };

    // 현재 대사 인덱스
    private int talkIndex = 0;

    // 대사가 모두 끝났는지 여부
    public static bool isDialogueEnded = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // 인스턴스 할당
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 중복 방지
        }
    }

    // 게임 시작 시 한 번 실행되는 초기화 함수
    void Start()
    {
        // 하트 UI가 할당되어 있으면 처음엔 보이지 않게 설정
        if (heartUIGroup != null)
        {
            heartUIGroup.alpha = 0f; // 투명하게
            heartUIGroup.interactable = false; // 상호작용 비활성화
            heartUIGroup.blocksRaycasts = false; // 클릭 등 이벤트 차단
        }
        NextTalk(); // 첫 번째 대사 출력
    }

    // 다음 대사를 출력하는 함수 (버튼 등에서 호출)
    public void NextTalk()
    {
        if (talkIndex < talks.Count)
        {
            talkText.text = talks[talkIndex]; // 현재 인덱스 대사 표시
            talkIndex++; // 다음 대사로 인덱스 증가
        }
        else
        {
            talkText.text = ""; // 대사가 끝나면 텍스트 비우기

            if (balloonImage != null)
                balloonImage.SetActive(false); // 대사 풍선 숨기기

            isDialogueEnded = true; // 대사 종료 상태로 변경

            // 하트 UI를 활성화 (투명도 및 상호작용 가능)
            if (heartUIGroup != null)
            {
                heartUIGroup.alpha = 1f;
                heartUIGroup.interactable = true;
                heartUIGroup.blocksRaycasts = true;
            }

            // 대사 종료 후 화살표 표시 및 목표 위치 설정
            ArrowManager.instance.SetCurrentGoal(RoundManager.instance.GetCurrentGoalTransform());
        }
    }

    // 결과 UI를 표시하는 함수 (게임 종료 시 등에서 호출)
    public void ShowResultUI()
    {
        if (resultUI != null)
            resultUI.SetActive(true); // 결과 UI 활성화
        Time.timeScale = 0f; // 게임 시간 정지
    }

    // 게임을 재시작하는 함수
    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간 정지 해제
        // 현재 씬을 다시 로드하여 게임 재시작
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
