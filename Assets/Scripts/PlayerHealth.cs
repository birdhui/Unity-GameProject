using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;

    void Start()
    {
        currentLives = maxLives;    // 시작 시 생명을 최대치로 설정
        HeartManager.instance.UpdateHearts(currentLives); // 초기 하트 UI 갱신
    }

    // 데미지를 입었을 때 호출되는 함수
    public void TakeDamage(int damage)
    {
        currentLives = Mathf.Max(currentLives - damage, 0); // 생명 감소(최소 0까지)
        HeartManager.instance.UpdateHearts(currentLives);   // 하트 UI 갱신
    }
}
