using UnityEngine;
using UnityEngine.UI;

// 플레이어의 하트(목숨) 관리 클래스
public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
    public Image[] heartImages; // 인스펙터에서 하트 이미지 3개를 연결
    public int initialHeartsPerRound = 3; // 라운드 시작 시 하트 개수
    [HideInInspector] public int currentLives; // 현재 남은 하트(목숨)
    [HideInInspector] public int totalLostHearts; // 전체 게임에서 잃은 하트의 누적 개수

    // 오브젝트가 생성될 때 실행
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // 하트 초기화
        currentLives = initialHeartsPerRound;
        totalLostHearts = 0;
        InitializeHearts();
    }

    // 모든 하트 이미지를 활성화(보이게)함
    void InitializeHearts()
    {
        foreach (Image heart in heartImages)
            heart.enabled = true;
    }

    // 하트 개수를 갱신하고 UI를 업데이트
    public void UpdateHearts(int newLives)
    {
        // 이번 라운드에서 잃은 하트 개수를 계산하여 누적
        int lostInThisRound = currentLives - newLives;
        totalLostHearts += lostInThisRound;

        // 현재 하트 개수 갱신
        currentLives = newLives;

        // 하트 이미지를 현재 하트 개수만큼만 보이게 설정
        for (int i = 0; i < heartImages.Length; i++)
            heartImages[i].enabled = (i < currentLives);
    }
}
