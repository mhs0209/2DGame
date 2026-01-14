using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [Header("Item Get Sounds")]
    public AudioClip[] itemGetClips; // 인스펙터에서 2개 등록

    [Header("Hit Sounds")]
    public AudioClip[] hitClips;     // 인스펙터에서 2개 등록

    // 아이템 획득 시 호출 (아이템 스크립트의 OnTriggerEnter 등에서 호출)
    public void PlayItemGetSound()
    {
        SoundManager.Instance.PlayRandomSFX(itemGetClips);
    }

    // 피격 시 호출 (플레이어 체력 깎이는 로직 부분에 추가)
    public void PlayHitSound()
    {
        SoundManager.Instance.PlayRandomSFX(hitClips);
    }
}