using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// 게임의 최종 결과 화면과 등급, UI 애니메이션, 씬 전환을 관리하는 클래스
public class FinalResultManager : MonoBehaviour
{
    public static FinalResultManager instance;

    // UI 오브젝트들 (인스펙터에서 연결)
    public GameObject background;         // 결과 배경 화면
    public GameObject resultPanel;        // 결과 패널(팝업)
    public RectTransform resultPanelRect; // 결과 패널의 RectTransform (애니메이션용)
    public Text resultText;               // 결과 메시지 텍스트
    public Text resultHeartText;          // 남은 목숨 표시 텍스트
    public Button mainBtn;                // 메인 화면 이동 버튼
    public GameObject directionalArrow;   // 방향 화살표 UI
    public GameObject timerUI;            // 타이머 UI
    public GameObject heartUI;            // 하트(목숨) UI

    [Header("등급 UI")]
    // 등급별 UI 오브젝트 (A+, B+, C+, D+, F)
    public GameObject gradeAPlus;
    public GameObject gradeBPlus;
    public GameObject gradeCPlus;
    public GameObject gradeDPlus;
    public GameObject gradeF;

    public int totalHearts = 9;           // 전체 목숨 개수
    public float fadeDuration = 0.5f;     // 결과 패널 애니메이션 시간

    // 오브젝트가 생성될 때 호출, 초기 UI 비활성화
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            background.SetActive(false);      // 결과 배경 비활성화
            resultPanel.SetActive(false);     // 결과 패널 비활성화
            // resultPanelRect가 할당되지 않았으면 컴포넌트에서 가져오기
            if (resultPanelRect == null && resultPanel != null)
                resultPanelRect = resultPanel.GetComponent<RectTransform>();
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }

    // 최종 결과 화면을 표시하는 함수
    public void ShowFinalResult()
    {
        background.SetActive(true);         // 배경 활성화
        resultPanel.SetActive(true);        // 결과 패널 활성화
        directionalArrow.SetActive(false);  // 방향 화살표 비활성화
        timerUI.SetActive(false);           // 타이머 UI 비활성화
        heartUI.SetActive(false);           // 하트 UI 비활성화
        Time.timeScale = 0f;                // 게임 시간 정지

        // 남은 목숨 계산: 전체 목숨 - 잃은 목숨
        int totalLost = HeartManager.instance.totalLostHearts;
        int remainingHearts = totalHearts - totalLost;

        resultHeartText.text = $"남은 목숨 개수 : {remainingHearts}/{totalHearts}"; // 남은 목숨 표시
        SetGradeUI(remainingHearts); // 남은 목숨에 따라 등급 UI 표시

        // 메인 버튼 클릭 이벤트 초기화 및 등록
        mainBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(GoToMainScene);

        StartCoroutine(PopupAnimation()); // 결과 패널 애니메이션 시작
    }

    // 결과 화면을 닫을 때 호출 (화살표 다시 표시)
    public void HideFinalResult()
    {
        directionalArrow.SetActive(true);
    }

    // 결과 패널이 위로 올라오며 페이드 인되는 애니메이션 코루틴
    IEnumerator PopupAnimation()
    {
        // 초기 상태: 아래에 위치, 완전히 투명
        resultPanelRect.anchoredPosition = new Vector2(0, -200f);
        CanvasGroup canvasGroup = resultPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = resultPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / fadeDuration;

            // 패널을 위로 이동
            resultPanelRect.anchoredPosition = Vector2.Lerp(
                new Vector2(0, -200f),
                Vector2.zero,
                progress
            );

            // 페이드 인
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }

        // 애니메이션 종료 후 위치와 투명도 고정
        resultPanelRect.anchoredPosition = Vector2.zero;
        canvasGroup.alpha = 1f;
    }

    // 남은 목숨 수에 따라 등급 UI를 표시하는 함수
    void SetGradeUI(int remainingHearts)
    {
        // 모든 등급 비활성화
        gradeAPlus.SetActive(false);
        gradeBPlus.SetActive(false);
        gradeCPlus.SetActive(false);
        gradeDPlus.SetActive(false);
        gradeF.SetActive(false);

        // 남은 목숨에 따라 등급 활성화
        if (remainingHearts >= 8) gradeAPlus.SetActive(true);    // 8~9개: A+
        else if (remainingHearts >= 6) gradeBPlus.SetActive(true);  // 6~7개: B+
        else if (remainingHearts >= 4) gradeCPlus.SetActive(true);  // 4~5개: C+
        else if (remainingHearts >= 2) gradeDPlus.SetActive(true);  // 2~3개: D+
        else gradeF.SetActive(true);                               // 0~1개: F
    }

    // 게임을 다시 시작(현재 씬 재시작)
    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간 정지 해제
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 재로딩
    }

    // 메인 화면으로 이동하는 함수 (메인 버튼에서 호출)
    void GoToMainScene()
    {
        Time.timeScale = 1f; // 시간 정지 해제

        // 정적 변수 초기화 (오브젝트 파괴 없이)
        ResetStaticVariables();

        // 메인 씬으로 이동 (LoadSceneMode.Single: 기존 씬 언로드)
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    // 정적 변수들을 초기값으로 리셋하는 함수
    void ResetStaticVariables()
    {
        GameManager.isDialogueEnded = false;                  // 대사 종료 상태 초기화
        Goal.goal = false;                                    // 골 상태 초기화
        HeartManager.instance.totalLostHearts = 0;            // 잃은 목숨 수 초기화
    }
}
