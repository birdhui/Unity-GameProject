using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    public GameObject[] goals;
    public Transform startPoint;

    private int currentRound = 1;              // 현재 라운드 번호
    private int currentGoalIndex = -1;         // 현재 활성화된 골인지점 인덱스

    void Awake() => instance = this ?? (instance = this); // 싱글턴 초기화

    void Start()
    {
        InitializeAllGoals(); // 모든 골인지점 초기화
        SetRandomGoal();      // 초기 골인지점 설정
    }

    // 모든 골인지점 비활성화 및 조명 초기화
    void InitializeAllGoals()
    {
        foreach (GameObject goal in goals)
        {
            goal.SetActive(false);
            Goal goalScript = goal.GetComponent<Goal>();
            if (goalScript != null) goalScript.ResetLight(); // 조명 컴포넌트 리셋
        }
    }

    // 다음 라운드 시작 시 호출되는 메서드
    public void NextRound()
    {
        Goal.goal = false; // 골인 상태 초기화
        Time.timeScale = 1f; // 게임 시간 정상화

        // 플레이어 상태 리셋
        CatController.instance.SetPlayerControl(true);
        CatController.instance.transform.position = startPoint.position;

        // 생명 시스템 리셋
        HeartManager.instance.currentLives = 3;
        HeartManager.instance.UpdateHearts(3);

        // 모든 골인지점 비활성화
        foreach (GameObject goal in goals) goal.SetActive(false);

        // 새로운 골인지점 설정
        currentGoalIndex = GetNewRandomGoalIndex();
        ActivateCurrentGoal(currentGoalIndex);

        // UI 관련 처리
        RoundResultManager.instance.HideResultUI();
        BusTimer.instance.RestartTimer();

        // 라운드 카운트 업데이트
        currentRound++;
        if (currentRound > 3) // 3라운드 종료 시 최종 결과 표시
        {
            FinalResultManager.instance.ShowFinalResult();
            RoundProgressManager.instance.ResetBoxes();
            currentRound = 1; // 라운드 카운터 초기화
        }
    }

    // 현재 골인지점과 중복되지 않는 새 인덱스 생성
    int GetNewRandomGoalIndex()
    {
        int newIndex;
        do
        {
            newIndex = Random.Range(0, goals.Length);
        } while (newIndex == currentGoalIndex); // 이전 골인지점과 다른 값 보장
        return newIndex;
    }

    // 랜덤 골인지점 설정
    void SetRandomGoal()
    {
        currentGoalIndex = GetNewRandomGoalIndex();
        ActivateCurrentGoal(currentGoalIndex);
    }

    // 특정 인덱스의 골인지점 활성화
    void ActivateCurrentGoal(int index)
    {
        goals[index].SetActive(true);
        Light currentLight = goals[index].GetComponentInChildren<Light>(true);
        if (currentLight != null) currentLight.enabled = true; // 조명 활성화
        ArrowManager.instance.SetCurrentGoal(goals[index].transform); // 화살표 표시 업데이트
    }

    // 현재 골인지점 위치 정보 반환
    public Transform GetCurrentGoalTransform()
    {
        if (currentGoalIndex >= 0 && currentGoalIndex < goals.Length)
            return goals[currentGoalIndex].transform;
        return null;
    }

    // 골인지점 충돌 처리
    public void CheckGoalCollision(GameObject collidedGoal)
    {
        if (collidedGoal == goals[currentGoalIndex]) HandleCorrectGoal();
        else HandleWrongGoal();
    }

    // 정확한 골인지점 도착 처리
    void HandleCorrectGoal()
    {
        Debug.Log("정확한 골인지점 도착!");
        Goal.goal = true;
        RoundResultManager.instance.ShowResultUI(); // 결과 UI 표시
    }

    // 잘못된 골인지점 도착 처리
    void HandleWrongGoal()
    {
        Debug.Log("잘못된 골인지점 도착!");
        // (추가 패널티 로직 구현 가능)
    }
}
