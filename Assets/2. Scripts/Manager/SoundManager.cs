using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 오디오 클립 배열 중 하나를 랜덤하게 재생하는 함수
    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        sfxSource.PlayOneShot(clips[randomIndex], volume);
    }
}