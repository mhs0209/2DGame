using UnityEngine;

public class TreasureRoom : ItemRoom
{
    public override void SpawnItems()
    {
        // 성장 방은 보통 방 중앙(0,0,0)에 아이템 1개 생성
        CreatePedestal(transform.position, RoomType.Treasure);
    }

    private void Start()
    {
        // 성장 방은 전투가 없으므로 시작하자마자 생성
        SpawnItems();
    }
}