using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : ItemRoom 
{
    public GameObject[] shopSpots; 
    
    private void Start() 
    {
        if (controller.roomData is ShopMap data) 
        {
            // 방어 코드: 인스펙터에서 shopSpots가 안 채워져 있는지 확인
            if (shopSpots == null || shopSpots.Length == 0) {
                Debug.LogError("ShopRoom: shopSpots 배열이 비어있습니다!");
                return;
            }
    
            for (int i = 0; i < shopSpots.Length; i++) 
            {
                if (shopSpots[i] == null) continue;
    
                bool isEquipment = (i < 2);
                List<ItemData> pool = isEquipment ? data.shopItemPool : data.pickupPool;
    
                // 풀이 비어있거나, 뽑힌 데이터가 Null인 경우를 철저히 방어
                if (pool == null || pool.Count == 0) continue;
    
                ItemData selectedData = pool[Random.Range(0, pool.Count)];
            
                // 여기가 에러 지점일 확률이 높음: selectedData나 itemPrefab 확인
                if (selectedData == null || selectedData.itemPrefab == null) {
                    Debug.LogWarning($"ShopRoom: {i}번 자리 아이템 데이터 혹은 프리팹이 부족합니다.");
                    continue;
                }
    
                GameObject spawnedItem = Instantiate(selectedData.itemPrefab, 
                    shopSpots[i].transform.position + Vector3.up * 0.5f, 
                    Quaternion.identity, shopSpots[i].transform);
            
                ShopItem shopLogic = spawnedItem.AddComponent<ShopItem>();
                shopLogic.Initialize(isEquipment ? 15 : 5);
            }
        }
    }
    
    public override void OnRoomCleared() { }
}