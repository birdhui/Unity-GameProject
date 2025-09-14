using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 버스 도착까지 남은 시간을 관리하고 UI에 표시하는 타이머 클래스
public class BusTimer : MonoBehaviour
{
    public static BusTimer instance;

    public Text timerText;           // 남은 시간 표시용 UI 텍스트
    public GameObject timeTextUI;    // 시간 UI 전체 오브젝트 (활성/비활성 제어)
    public float minTimeLimit = 20f; // 최소 타이머 제한 (초)
    public float maxTimeLimit = 60f; // 최대 타이머 제한 (초)
    public float warningTime = 10f;  // 경고(깜빡임) 시작 시간 (초)
    public float blinkInterval = 0.5f; // 깜빡임 간격 (초)

    public static float remainingTime; // 남은 시간(정적, 외부 접근 가능)
    private float timeLimit;           // 이번 라운드의 총 시간 제한
    private bool isTimerRunning = false; // 타이머 동작 여부
    private bool isBlinking = false;     // 깜빡임 동작 여부

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject); // 중복 방지
    }

    // 게임 시작 시 타이머 초기화
    void Start()
    {
        Goal.goal = false; // 골 상태 초기화
        ResetTimer();      // 타이머 UI 및 변수 초기화
    }

    // 매 프레임마다 타이머 및 UI 상태 업데이트
    void Update()
    {
        // 대사가 끝나지 않았으면 타이머 비활성화 및 UI 숨김
        if (!GameManager.isDialogueEnded)
        {
            timeTextUI.SetActive(false);
            isTimerRunning = false;
            return;
        }

        // 대사 종료 후 타이머가 꺼져 있으면 시작
        if (!isTimerRunning)
        {
            StartTimer();
        }

        // 골에 도달하면 타이머 정지 및 UI 처리
        if (Goal.goal)
        {
            isTimerRunning = false;
            StopAllCoroutines(); // 깜빡임 등 코루틴 중지
            timerText.color = Color.red; // 텍스트 빨간색 표시
            return;
        }

        // 타이머가 동작 중이면 시간 감소 및 UI 업데이트
        if (isTimerRunning)
        {
            remainingTime -= Time.deltaTime;

            // 경고 시간 이하일 때 깜빡임 시작
            if (remainingTime <= warningTime && remainingTime > 0)
            {
                if (!isBlinking) StartCoroutine(BlinkTimerText());
            }

            // 남은 시간을 분:초 형식으로 표시
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"버스 도착까지: {minutes:00}:{seconds:00}";

            // 시간이 다 되면 타이머 정지 및 "시간 초과!" 표시
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isTimerRunning = false;
                timerText.color = Color.red;
                timerText.text = "시간 초과!";
                // 타임오버 시 추가 처리 필요
            }
        }
    }

    // 타이머 시작: 제한 시간 랜덤 설정, UI 표시, 변수 초기화
    public void StartTimer()
    {
        timeLimit = Random.Range(minTimeLimit, maxTimeLimit + 1); // 제한 시간 랜덤 설정
        remainingTime = timeLimit;
        isTimerRunning = true;
        timeTextUI.SetActive(true);      // 타이머 UI 표시
        timerText.color = Color.black;   // 기본 텍스트 색상
    }

    // 경고 시간에 타이머 텍스트 깜빡임 처리 코루틴
    IEnumerator BlinkTimerText()
    {
        isBlinking = true;
        bool isVisible = true;

        while (remainingTime > 0 && !Goal.goal)
        {
            timerText.color = isVisible ? Color.red : new Color(1, 0, 0, 0.5f); // 빨강/반투명 반복
            isVisible = !isVisible;
            yield return new WaitForSeconds(blinkInterval);
        }
        timerText.color = Color.white; // 깜빡임 종료 후 흰색
        isBlinking = false;
    }

    // 타이머 및 UI, 변수 초기화 (코루틴 중지 포함)
    public void ResetTimer()
    {
        isTimerRunning = false;
        timeTextUI.SetActive(false);
        timerText.color = Color.black;
        StopAllCoroutines();  // 진행 중인 깜빡임 코루틴 중지
        isBlinking = false;   // 깜빡임 상태 리셋
    }

    // 타이머 재시작: 초기화 후 다시 시작
    public void RestartTimer()
    {
        ResetTimer();
        StartTimer();
    }
}
