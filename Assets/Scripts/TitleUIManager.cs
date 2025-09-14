using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    public GameObject menuSet;             // Menu UI 전체
    public Camera uiCamera;                // 시작 카메라
    public Camera trackingCamera;          // 캐릭터 시점 카메라

    // 게임이 시작될 때 호출되는 함수
    public void Start()
    {
        Time.timeScale = 0f; // 게임을 일시정지 상태로 만듦 (시간 흐름을 멈춤)
        trackingCamera.gameObject.SetActive(false); // 캐릭터 시점 카메라를 비활성화
        uiCamera.gameObject.SetActive(true);        // UI 카메라를 활성화하여 타이틀 화면을 보여줌
    }

    // '시작' 버튼이 클릭되면 호출되는 함수
    public void OnStartButtonClicked()
    {
        menuSet.SetActive(false);                // 메뉴 UI를 숨김
        uiCamera.gameObject.SetActive(false);    // UI 카메라를 비활성화
        trackingCamera.gameObject.SetActive(true); // 캐릭터 시점 카메라를 활성화
        Time.timeScale = 1f;                      // 게임을 정상 속도로 진행 (일시정지 해제)
    }
}
