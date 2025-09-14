using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;      // 따라갈 대상 (캐릭터)
    public Vector3 offset = new Vector3(0, 3, -5); // 카메라 위치 오프셋
    public float smoothSpeed = 5f;

    // LateUpdate는 모든 Update가 끝난 후에 호출되어, 캐릭터 이동 후 카메라가 따라오도록 함
    void LateUpdate()
    {
        if (target == null) return; // 따라갈 대상이 없으면 함수 종료

        Vector3 desiredPosition = target.position + offset; // 대상 위치에 오프셋을 더해 카메라의 목표 위치 계산
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = smoothedPosition; // 카메라 위치를 부드럽게 갱신

        transform.LookAt(target); // 카메라가 항상 대상을 바라보도록 회전
    }
}
