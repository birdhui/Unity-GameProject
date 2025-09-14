using UnityEngine;
using UnityEngine.UI;

public class RoundProgressManager : MonoBehaviour
{
    public static RoundProgressManager instance;
    public Image[] roundBoxes; // 박스 UI 3개
    public Sprite emptyBoxSprite; // 닫힌 박스 스프라이트
    public Sprite openBoxSprite;  // 열린 박스 스프라이트

    private int currentRound = 0;                // 현재 완료된 라운드 수

    void Awake()
    {
        if (instance == null)
        {
            instance = this;        // 인스턴스가 없으면 자신을 할당
            InitializeBoxes();      // 박스 초기화(모두 닫힌 상태)
        }
        else
        {
            Destroy(gameObject);    // 중복 인스턴스 방지
        }
    }

    // 모든 박스를 닫힌 상태로 초기화하는 함수
    void InitializeBoxes()
    {
        foreach (Image box in roundBoxes)
            box.sprite = emptyBoxSprite; // 각 박스 이미지를 닫힌 박스로 설정
    }

    // 라운드 완료 시 해당 박스를 열린 박스로 변경
    public void CompleteRound()
    {
        if (currentRound < roundBoxes.Length)
        {
            roundBoxes[currentRound].sprite = openBoxSprite; // 현재 라운드 박스를 열린 박스로 변경
            currentRound++;                                  // 다음 라운드로 인덱스 증가
        }
    }

    // 모든 박스를 초기 상태(닫힌 박스)로 리셋
    public void ResetBoxes()
    {
        currentRound = 0; // 라운드 인덱스 초기화
        foreach (Image box in roundBoxes)
            box.sprite = emptyBoxSprite; // 모든 박스를 닫힌 상태로 변경
    }
}
