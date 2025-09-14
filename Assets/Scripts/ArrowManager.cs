using UnityEngine;
using UnityEngine.UI;

// 플레이어가 목표(Goal) 위치를 쉽게 찾을 수 있도록 방향 화살표 UI를 관리하는 클래스
public class ArrowManager : MonoBehaviour
{
    public static ArrowManager instance;
    public Image arrowImage;                // 방향 화살표 UI 이미지
    public Transform player;                // 플레이어 트랜스폼
    public Camera mainCamera;               // 메인 카메라

    private Transform currentGoal;          // 현재 목표(골) 위치

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            arrowImage.gameObject.SetActive(false); // 초기엔 화살표 숨김
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }

    // 매 프레임마다 화살표 방향, 위치, 표시 여부 업데이트
    void Update()
    {
        // 목표, 플레이어, 카메라가 없으면 처리 중단
        if (currentGoal == null || player == null || mainCamera == null) return;

        // 대사가 끝나지 않았으면 화살표 숨김
        if (!GameManager.isDialogueEnded)
        {
            arrowImage.gameObject.SetActive(false);
            return;
        }

        // 1. 화살표 위치 고정 (화면 하단 중앙, y=150)
        arrowImage.rectTransform.anchoredPosition = new Vector2(0, 150);

        // 2. 플레이어에서 목표까지의 방향 (수평면 기준)
        Vector3 playerToGoal = (currentGoal.position - player.position).normalized;
        playerToGoal.y = 0;

        // 3. 카메라 기준의 전방/우측 (수평면 기준)
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // 4. 카메라 기준으로 목표가 어느 방향에 있는지 내적(dot)으로 계산
        float rightDot = Vector3.Dot(playerToGoal, cameraRight);
        float forwardDot = Vector3.Dot(playerToGoal, cameraForward);

        // 5. 각도 계산 후 화살표 회전 (z축 회전)
        float angle = Mathf.Atan2(rightDot, forwardDot) * Mathf.Rad2Deg;
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);

        // 6. 목표가 화면 중앙 근처에 보이면 화살표 숨김, 아니면 표시
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(currentGoal.position);
        bool isVisible = viewportPos.z > 0 &&
                         viewportPos.x > 0.2f && viewportPos.x < 0.8f &&
                         viewportPos.y > 0.2f && viewportPos.y < 0.8f;

        arrowImage.gameObject.SetActive(!isVisible);
    }

    // 목표 위치를 외부에서 설정하는 함수
    public void SetCurrentGoal(Transform goal)
    {
        currentGoal = goal;
        // 대사가 끝났고 목표가 있으면 화살표 표시, 아니면 숨김
        if (GameManager.isDialogueEnded && currentGoal != null)
        {
            arrowImage.gameObject.SetActive(true);
        }
        else
        {
            arrowImage.gameObject.SetActive(false);
        }
    }
}
