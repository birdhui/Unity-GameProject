using UnityEngine;

// 게임의 배경음악(BGM)이 씬 전환에도 음악이 끊기지 않도록 유지
public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private AudioSource audioSource;     // BGM 재생용 오디오 소스

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 오브젝트가 파괴되지 않음
            audioSource = GetComponent<AudioSource>(); // 오디오 소스 컴포넌트 참조
            if (!audioSource.isPlaying)
                audioSource.Play(); // 씬 시작 시 BGM 자동 재생
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }
}
