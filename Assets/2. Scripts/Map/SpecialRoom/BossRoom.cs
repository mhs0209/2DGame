using UnityEngine;

public class BossRoom : ItemRoom
{
    [Header("Stage Transition")]
    public GameObject stagePortalPrefab; // 다음 스테이지 이동 포탈 프리팹
    public string stagePortalName;

    public override void OnRoomCleared()
    {
        GameObject portal = null;
        
        if (controller.roomData is BossMap data) {
            // 보상 아이템 생성
            if (data.rewardPool.Length > 0)
                Instantiate(data.rewardPool[Random.Range(0, data.rewardPool.Length)], transform.position + Vector3.up, Quaternion.identity);
            
            // 포탈 생성
            if (stagePortalPrefab != null)
                portal = Instantiate(stagePortalPrefab, transform.position + Vector3.down * 1.5f, Quaternion.identity);
        }
        
        if (portal != null)
        {
            portal.GetComponent<StagePortal>().NextStage(stagePortalName);
        }
    }
}