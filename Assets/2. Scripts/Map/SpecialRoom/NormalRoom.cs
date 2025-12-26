using UnityEngine;

public class NormalRoom : ItemRoom 
{
    public override void OnRoomCleared() {
        if (controller.roomData is NormalMap data) {
            SpawnLoot(data.lootTable);
        }
    }

    private void SpawnLoot(LootEntry[] table) {
        if (table == null || table.Length == 0) return;
        float totalWeight = 0;
        foreach (var entry in table) totalWeight += entry.weight;
        
        float roll = Random.Range(0, totalWeight);
        float current = 0;
        foreach (var entry in table) {
            current += entry.weight;
            if (roll <= current) {
                if (entry.prefab != null) Instantiate(entry.prefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}