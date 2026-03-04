using UnityEngine;

public class BossRoom : ItemRoom
{
    public override void OnRoomCleared()
    {
        if (controller.roomData is BossMap data) {
            // 보상 아이템 생성
            if (data.itemDropPool.Count > 0)
                Instantiate(data.itemDropPool[Random.Range(0, data.itemDropPool.Count)].itemPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
            
            // 포탈 생성
            if (data.stagePortalPrefab != null)
                Instantiate(data.stagePortalPrefab, transform.position + Vector3.down * 1.5f, Quaternion.identity);
        }
    }
}