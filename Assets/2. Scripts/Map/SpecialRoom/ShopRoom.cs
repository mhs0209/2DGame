using UnityEngine;

public class ShopRoom : ItemRoom
{
    public override void SpawnItems()
    {
        // 상점은 지정된 여러 좌표(spawnPoints)에 아이템 생성
        foreach (Transform t in spawnPoints)
        {
            CreatePedestal(t.position, RoomType.Shop);
            // 상점 전용 추가 로직 (가격표 표시 등)이 여기 추가될 예정
        }
    }

    private void Start()
    {
        SpawnItems();
    }
}