using UnityEngine;

// 플레이어가 골 지점에 도달했을 때 동작을 관리하는 클래스
public class Goal : MonoBehaviour
{
    // 골 도달 여부를 저장하는 정적 변수 (다른 스크립트에서도 접근 가능)
    public static bool goal = false;

    // 골 지점의 라이트
    public Light redLight;

    // 골 도달 시 재생할 사운드 클립
    public AudioClip goalSound;

    // 오디오 재생에 사용할 AudioSource 컴포넌트
    private AudioSource audioSource;

    // 게임 오브젝트가 활성화될 때 한 번 실행되는 초기화 함수
    void Start()
    {
        goal = false; // 게임 시작 시 골 상태를 초기화
        if (redLight != null) redLight.enabled = true; // 라이트가 할당되어 있으면 켜기
        audioSource = GetComponent<AudioSource>(); // 오디오 소스 컴포넌트 가져오기
    }

    // 라이트를 빨간색으로 초기화하고 켜는 함수
    public void ResetLight()
    {
        if (redLight != null)
        {
            redLight.color = Color.red; // 라이트 색상을 빨간색으로 변경
            redLight.enabled = true;    // 라이트 켜기
        }
    }

    // 트리거 콜라이더에 충돌이 발생했을 때 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            Goal.goal = true; // 골 상태를 true로 설정

            // 라이트가 할당되어 있으면 초록색으로 변경 (골 성공 표시)
            if (redLight != null) redLight.color = Color.green;

            // 오디오 소스와 사운드가 모두 할당되어 있으면 사운드 재생
            if (audioSource != null && goalSound != null)
                audioSource.PlayOneShot(goalSound);

            // 플레이어의 조작을 비활성화
            CatController.instance.SetPlayerControl(false);

            // 게임 타이머 정지
            Time.timeScale = 0f;

            // 결과 UI 표시
            RoundResultManager.instance.ShowResultUI();

            // 라운드 완료 처리
            RoundProgressManager.instance.CompleteRound();
        }
    }
}
