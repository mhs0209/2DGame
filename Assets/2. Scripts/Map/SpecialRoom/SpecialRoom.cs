using UnityEngine;

public class SpecialRoom : ItemRoom
{
    private void Start()
    { // 성장방은 입장 시 바로 생성
        if (controller.roomData is TreasureMap data) {
            SpawnPedestal(transform.position, data.itemPool);
        }
    }
    public override void OnRoomCleared() { /* 이미 클리어 상태 */ }

    private void SpawnPedestal(Vector3 pos, GameObject[] pool) {
        if (pool.Length == 0) return;
        TreasureMap data = controller.roomData as TreasureMap;
        GameObject pedestal = Instantiate(data.pedestalPrefab, pos, Quaternion.identity, transform);
        // 제단에 랜덤 아이템 올리는 로직 (임시)
        if (data.itemPool.Length > 0) {
            GameObject item = data.itemPool[Random.Range(0, data.itemPool.Length)];
            Instantiate(item, pedestal.transform.position + Vector3.up * 0.5f, Quaternion.identity, pedestal.transform);
            Debug.Log($"성장방 아이템 생성: {item.name}");  
        }
    }
}