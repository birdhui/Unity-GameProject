using UnityEngine;
using System.Collections;

// 오브젝트가 직선으로 움직이고, 특정 존에 닿으면 정해진 순서로 다시 나타나는(리스폰) 클래스
public class StraightLooper : MonoBehaviour
{
    [Header("이동 설정")]
    public float speed = 8f; // 오브젝트가 움직이는 속도
    public Transform[] respawnPoints; // 리스폰 위치들(순서대로 넣어야 함)

    [Header("리스폰 패턴")]
    public int currentRespawnIndex = 0; // 지금 몇 번째 리스폰 존에서 시작하는지(0~3)

    private bool isRespawning = false; // 리스폰 중인지 체크(중복 리스폰 방지)
    private Rigidbody rb;
    private Vector3 moveDirection; // 앞으로 나아가는 방향
    private float originalSpeed; // 원래 속도 값 저장

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기

        // Rigidbody 물리 설정(더 무겁고, 회전 덜 하고, 중력 받게 함)
        rb.mass = 1000f; // 무겁게 만들어서 튕김 방지
        rb.drag = 0.1f; // 약간의 저항(더 자연스럽게 멈춤)
        rb.angularDrag = 0.5f; // 회전 저항
        rb.useGravity = true; // 중력 적용
        rb.isKinematic = false; // 물리 엔진으로 움직임
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 빠른 이동 시 충돌 감지 정확도 높임

        // X, Z축 회전은 막고 Y축(좌우 회전)만 허용해서 직진 유지
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        moveDirection = transform.forward; // 처음엔 오브젝트가 바라보는 방향으로 이동
        originalSpeed = speed; // 원래 속도 저장해둠
    }

    void FixedUpdate()
    {
        if (isRespawning) return; // 리스폰 중이면 이동 안 함

        // 앞으로 계속 이동(물리 기반)
        Vector3 movement = moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // 항상 같은 방향(직진)으로 바라보게 함
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isRespawning) return; // 리스폰 중엔 충돌 무시

        // Respawn 존에 닿으면 리스폰 시작
        if (other.CompareTag("Respawn"))
        {
            StartCoroutine(RespawnVehicle());
        }

        // Player와 부딪히면 데미지 주기
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 플레이어 체력 1 깎기
            }
        }
    }

    // 리스폰 처리 코루틴(잠깐 멈췄다가 위치/방향 바꿔서 다시 시작)
    IEnumerator RespawnVehicle()
    {
        isRespawning = true; // 리스폰 중으로 표시

        // 다음 리스폰 위치 고르기(패턴대로)
        Transform targetRespawn = GetNextRespawnPoint();

        // 이동/회전 멈추기
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 위치를 새 리스폰 존으로 옮기고, 약간 위로 띄우기
        Vector3 newPos = targetRespawn.position;
        newPos.y += 0.5f; // 바닥에 박히지 않게 살짝 띄움

        transform.SetPositionAndRotation(newPos, targetRespawn.rotation);
        moveDirection = transform.forward; // 새 방향으로 이동하도록 설정
        speed = originalSpeed; // 속도 원래대로

        yield return new WaitForSeconds(0.5f); // 0.5초 쉬었다가
        isRespawning = false; // 리스폰 끝
    }

    // 리스폰 순서에 따라 다음 위치 반환(0↔1, 2↔3)
    Transform GetNextRespawnPoint()
    {
        switch (currentRespawnIndex)
        {
            case 0: return respawnPoints[1]; // 0번 → 1번 존
            case 1: return respawnPoints[0]; // 1번 → 0번 존
            case 2: return respawnPoints[3]; // 2번 → 3번 존
            case 3: return respawnPoints[2]; // 3번 → 2번 존
            default: return respawnPoints[0]; // 혹시 잘못된 값이면 0번 존
        }
    }
}
