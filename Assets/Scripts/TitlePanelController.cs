using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePanelController : MonoBehaviour
{
    public CanvasGroup chatUIGroup; // ChatUI의 CanvasGroup
    public GameObject titlePanel;

    void Start()
    {
        // 게임 시작 시 ChatUI를 완전히 숨기고, 상호작용 및 이벤트를 차단함
        if (chatUIGroup != null)
        {
            chatUIGroup.alpha = 0f;            // ChatUI를 완전히 투명하게 설정
            chatUIGroup.interactable = false;  // 사용자와의 상호작용 비활성화
            chatUIGroup.blocksRaycasts = false;// 마우스 클릭 등 모든 이벤트 차단
        }
    }

    // '시작' 버튼 클릭 시 호출되는 함수
    public void OnStartButtonClick()
    {
        if (titlePanel != null)
            titlePanel.SetActive(false); // 타이틀 패널을 비활성화하여 숨김

        // ChatUI를 보이게 하고, 상호작용 및 이벤트를 허용함
        if (chatUIGroup != null)
        {
            chatUIGroup.alpha = 1f;            // ChatUI를 완전히 불투명하게 설정
            chatUIGroup.interactable = true;   // 사용자와의 상호작용 활성화
            chatUIGroup.blocksRaycasts = true; // 마우스 클릭 등 모든 이벤트 허용
        }
    }
}
