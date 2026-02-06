using System.Collections.Generic;
using UnityEngine;

public class NormalRoom : ItemRoom 
{
    public override void OnRoomCleared() {
        if (controller.roomData is BaseRoomData data) {
            SpawnLoot(data.itemDropPool);
        }
    }

    private void SpawnLoot(List<ItemData> pool) {
        if (pool == null || pool.Count == 0) return;

        // [중요] 테이블의 DropWeight를 사용하는 가중치 랜덤 로직
        float totalWeight = 0;
        foreach (var item in pool) {
            var tableData = TableDataManager.Instance.GetItemTableData(item.itemID);
            if (tableData != null) totalWeight += tableData.DropWeight;
        }
        
        float roll = Random.Range(0, totalWeight);
        float current = 0;

        foreach (var item in pool) {
            var tableData = TableDataManager.Instance.GetItemTableData(item.itemID);
            current += tableData.DropWeight;

            if (roll <= current) {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                break; // 1개만 소환하고 종료
            }
        }
    }
}