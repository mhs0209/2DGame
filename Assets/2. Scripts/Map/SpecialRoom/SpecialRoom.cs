using UnityEngine;

public class SpecialRoom : ItemRoom
{
    public override void SpawnItems()
    {
        // 특수 방만의 로직 (예: 조건부 생성 등)
        CreatePedestal(transform.position, RoomType.Special);
    }

    private void Start()
    {
        SpawnItems();
    }
}