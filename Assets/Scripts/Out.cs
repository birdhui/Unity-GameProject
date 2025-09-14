using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어가 특정 영역에 닿으면 현재 씬을 다시 시작하는 클래스
public class Out : MonoBehaviour
{
    // 다른 오브젝트가 이 트리거에 들어왔을 때 실행됨
    void OnTriggerEnter(Collider col)
    {
        // 만약 닿은 오브젝트의 태그가 "Player"라면
        if (col.gameObject.tag == "Player")
        {
            // 현재 실행 중인 씬을 다시 불러와서(리셋) 게임을 처음 상태로 돌림
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }
    }
}
