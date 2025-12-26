using UnityEngine;

public class BossRoom : ItemRoom
{
    public override void OnRoomCleared()
    {
        
    }

    public override void SpawnItems()
    {
        // 보스 방은 보스가 죽은 위치 혹은 특정 지정 위치에 생성
        Vector3 spawnPos = spawnPoints.Length > 0 ? spawnPoints[0].position : transform.position;
        CreatePedestal(spawnPos, RoomType.Boss);
    }

    // 보스 사망 시 RoomController나 Boss 스크립트에서 이 함수를 호출하게 함
    public void OnBossDefeated()
    {
        SpawnItems();
    }
}