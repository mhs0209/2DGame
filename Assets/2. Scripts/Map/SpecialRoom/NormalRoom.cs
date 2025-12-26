using UnityEngine;

public class NormalRoom : ItemRoom {
    
    public override void OnRoomCleared() {
        NormalMap data = roomController.roomData as NormalMap;
        if (data == null || data.lootTables == null) return;

        // 확률 로직 (0~100 사이 랜덤)
        float randomValue = Random.Range(0f, 100f);
        float currentWeight = 0;

        foreach (var loot in data.lootTables) {
            currentWeight += loot.dropWeight;
            if (randomValue <= currentWeight) {
                if (loot.rewardPrefab != null) {
                    Instantiate(loot.rewardPrefab, transform.position, Quaternion.identity);
                }
                break; // 하나만 드랍하고 종료
            }
        }
        Debug.Log("노말 방 전투 보상 생성 시도 완료");
    }
}