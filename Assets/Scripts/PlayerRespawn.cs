using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        // 시작 시 현재 위치를 시작 위치로 저장
        startPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // "Car" 태그를 가진 오브젝트와 충돌하면
        if (collision.gameObject.CompareTag("Car"))
        {
            // 플레이어 위치를 시작 위치로 되돌림(리스폰)
            transform.position = startPosition;
        }
    }
}
