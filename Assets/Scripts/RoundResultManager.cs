using UnityEngine;
using UnityEngine.UI;

public class RoundResultManager : MonoBehaviour
{
    public static RoundResultManager instance;
    public GameObject roundresultUI;
    public Button nextButton;

    void Awake()
    {
        if (instance == null) instance = this; // 인스턴스가 없으면 자신을 할당
        else Destroy(gameObject);              // 이미 있으면 중복 방지 위해 파괴
    }

    void Start()
    {
        roundresultUI.SetActive(false); // 시작 시 결과 UI를 숨김
        nextButton.onClick.AddListener(() => RoundManager.instance.NextRound()); // '다음' 버튼 클릭 시 다음 라운드로 이동
    }

    // 라운드 결과 UI를 표시하는 함수
    public void ShowResultUI()
    {
        roundresultUI.SetActive(true);        // 결과 UI 표시
        nextButton.gameObject.SetActive(true); // '다음' 버튼 표시
    }

    // 라운드 결과 UI를 숨기는 함수
    public void HideResultUI()
    {
        roundresultUI.SetActive(false);        // 결과 UI 숨김
        nextButton.gameObject.SetActive(false); // '다음' 버튼 숨김
    }
}
