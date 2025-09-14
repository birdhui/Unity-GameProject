using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
// 플레이어(고양이) 캐릭터의 이동, 점프, 회전, 사운드, 리셋 등 제어를 담당 클래스
public class CatController : MonoBehaviour
{
    // 이동 및 점프 속도 설정
    public float moveSpeed = 2.0f;      // 걷기 속도
    public float runSpeed = 4.0f;       // 달리기 속도 (더블탭)
    public float jumpForce = 5.0f;      // 점프 힘
    public Transform startPoint;        // 시작 위치(리셋용)

    // 점프 사운드
    public AudioClip jumpClip;
    private AudioSource audioSource;    // 오디오 소스 컴포넌트

    // 컴포넌트 참조
    private Rigidbody rb;               // 물리 엔진용 리지드바디
    private Animator animator;          // 애니메이터
    private Transform cameraTransform;  // 메인 카메라 트랜스폼

    // 상태 변수
    private bool isJumping = false;     // 점프 중 여부
    private bool isRunning = false;     // 달리기 상태 여부
    private float lastForwardTapTime = -1f;   // 마지막 앞 방향키 입력 시간
    private float doubleTapThreshold = 0.3f;  // 더블탭 인식 시간 간격

    public static CatController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject); // 중복 방지
    }

    // 컴포넌트 초기화 및 기본 상태 설정
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();

        rb.freezeRotation = true; // 물리 회전 고정
        animator.SetBool("IsJumping", false); // 점프 초기화
    }

    // 매 프레임마다 입력 및 이동 처리
    void Update()
    {
        if (Time.timeScale == 0) return; // 게임이 일시정지면 입력 무시

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.Q)) ResetPosition(); // Q키로 위치 리셋
        HandleDoubleTap(); // 더블탭(달리기) 처리

        Vector3 moveDirection = CalculateCameraRelativeDirection(inputDirection); // 카메라 기준 방향 변환
        HandleMovement(moveDirection); // 이동 처리
        HandleRotation(moveDirection); // 회전 처리
        HandleJump(); // 점프 처리
    }

    // 외부에서 플레이어 제어 활성/비활성화 (예: 골 도달 시)
    public void SetPlayerControl(bool isActive)
    {
        enabled = isActive; // 스크립트 활성/비활성
        StartCoroutine(SetRigidbodySettings(isActive)); // 리지드바디 설정 병행
    }

    // 리지드바디의 물리 설정(kinematic, CCD) 변경 코루틴
    IEnumerator SetRigidbodySettings(bool isActive)
    {
        yield return new WaitForFixedUpdate();

        // 비활성화 시: 충돌 감지 모드 변경 후 kinematic 설정
        if (!isActive)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // 활성화 시: kinematic 해제 후 충돌 감지 모드 변경
        else
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    // 입력 방향을 카메라 기준 방향으로 변환하는 함수
    Vector3 CalculateCameraRelativeDirection(Vector3 input)
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        return (camForward.normalized * input.z + camRight.normalized * input.x).normalized;
    }

    // 실제 이동 처리 (속도, 애니메이션)
    void HandleMovement(Vector3 direction)
    {
        if (direction.magnitude < 0.01f)
        {
            animator.SetFloat("Speed", 0); // 정지 애니메이션
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        rb.velocity = new Vector3(direction.x * currentSpeed, rb.velocity.y, direction.z * currentSpeed);
        animator.SetFloat("Speed", isRunning ? 1.0f : 0.5f); // 달리기/걷기 애니메이션
    }

    // 이동 방향을 따라 캐릭터 회전 처리
    void HandleRotation(Vector3 direction)
    {
        if (direction.magnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15f);
    }

    // 점프 입력 및 애니메이션, 사운드 처리
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            animator.SetBool("IsJumping", true);
            if (jumpClip != null) audioSource.PlayOneShot(jumpClip);
        }
    }

    // 앞 방향키 더블탭(달리기) 입력 처리
    void HandleDoubleTap()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            isRunning = (Time.time - lastForwardTapTime) < doubleTapThreshold;
            lastForwardTapTime = Time.time;
        }
    }

    // 시작 위치로 캐릭터 리셋 (Q키)
    public void ResetPosition()
    {
        if (startPoint != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
    }

    // 충돌 처리: 땅에 닿으면 점프 상태 해제, 자동차에 닿으면 데미지
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
        else if (collision.gameObject.CompareTag("Car"))
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(1);
        }
    }

    // 발소리 애니메이션 이벤트용 (실제 사운드 재생 로직 없음)
    public void PlayFootstep()
    {
        // 빈 메서드 (애니메이션 이벤트에서 호출)
    }
}
