using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : ItemRoom
{
    private void Start()
    { // 성장방은 입장 시 바로 생성
        if (controller.roomData is SpecialMap data) {
            SpawnPedestal(transform.position, data.itemDropPool);
        }
    }
    public override void OnRoomCleared() { /* 이미 클리어 상태 */ }

    private void SpawnPedestal(Vector3 pos, List<ItemData> pool) {
        if (pool.Count == 0) return;
        SpecialMap data = controller.roomData as SpecialMap;
        GameObject pedestal = Instantiate(data.pedestalPrefab, pos, Quaternion.identity, transform);
        // 랜덤 아이템 생성
        if (data.itemDropPool.Count > 0) {
            GameObject itemPrefab = data.itemDropPool[Random.Range(0, data.itemDropPool.Count)].itemPrefab;
            GameObject spawnedItem = Instantiate(itemPrefab, pedestal.transform.position + Vector3.up * 0.5f, Quaternion.identity, pedestal.transform);
            
            // [핵심] 특수방 전용 구매 스크립트 부착 및 초기화 (열쇠 1개 소모)
            SpecialItem specialLogic = spawnedItem.AddComponent<SpecialItem>();
            specialLogic.Initialize(1); 
        }
    }
}