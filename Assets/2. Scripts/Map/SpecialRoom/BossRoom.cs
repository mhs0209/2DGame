using UnityEngine;

public class BossRoom : ItemRoom
{
    [Header("Stage Transition")]
    public GameObject stagePortalPrefab; // 다음 스테이지 이동 포탈 프리팹

    public override void OnRoomCleared()
    {
        if (controller.roomData is BossMap data) {
            // 보상 아이템 생성
            if (data.itemDropPool.Count > 0)
                Instantiate(data.itemDropPool[Random.Range(0, data.itemDropPool.Count)], transform.position + Vector3.up, Quaternion.identity);
            
            // 포탈 생성
            if (stagePortalPrefab != null)
                Instantiate(stagePortalPrefab, transform.position + Vector3.down * 1.5f, Quaternion.identity);
        }
    }
}