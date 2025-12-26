using UnityEngine;

public class BossRoom : ItemRoom
{
    [Header("Stage Transition")]
    public GameObject stagePortalPrefab; // 다음 스테이지 이동 포탈 프리팹
    public string stagePortalName;

    public override void OnRoomCleared()
    {
        if (controller.roomData is BossMap data)
        {
            // 1. 보스 보상 아이템 생성 (중앙 혹은 지정 위치)
            if (data.rewardPool.Length > 0)
            {
                GameObject reward = data.rewardPool[Random.Range(0, data.rewardPool.Length)];
                Instantiate(reward, transform.position + Vector3.up, Quaternion.identity);
            }

            // 2. 다음 스테이지 이동 포탈 생성
            SpawnNextStagePortal();
        }
    }

    private void SpawnNextStagePortal()
    {
        if (stagePortalPrefab != null)
        {
            // 플레이어가 보상을 먹기 편하게 약간 떨어진 위치에 포탈 생성
            Instantiate(stagePortalPrefab, transform.position + Vector3.down * 2f, Quaternion.identity);
            stagePortalPrefab.GetComponent<StagePortal>().NextStage(stagePortalName);
            Debug.Log("다음 스테이지 포탈이 열렸습니다.");
        }
    }
}