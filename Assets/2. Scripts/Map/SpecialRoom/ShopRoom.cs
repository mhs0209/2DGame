using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : ItemRoom 
{
    public GameObject[] shopSpots; 
    
    private void Start() 
    {
        if (controller.roomData is ShopMap data) 
        {
            // 중복 방지를 위한 복사본
            List<ItemData> equipmentList = new List<ItemData>(data.shopItemPool);
            List<ItemData> pickupList = new List<ItemData>(data.pickupPool);

            for (int i = 0; i < shopSpots.Length; i++) 
            {
                if (shopSpots[i] == null) continue;

                // 0, 1번 자리는 장비(15G) / 2, 3번 자리는 픽업(5G)
                bool isEquipment = (i < 2);
                List<ItemData> targetPool = isEquipment ? equipmentList : pickupList;

                if (targetPool.Count == 0) continue;

                // 랜덤 선택 및 중복 제거
                int randIdx = Random.Range(0, targetPool.Count);
                ItemData selected = targetPool[randIdx];
                targetPool.RemoveAt(randIdx); 

                // 생성 및 가격 설정
                GameObject spawned = Instantiate(selected.itemPrefab, 
                    shopSpots[i].transform.position + Vector3.up * 0.5f, 
                    Quaternion.identity, shopSpots[i].transform);
                
                ShopItem shopLogic = spawned.AddComponent<ShopItem>();
                shopLogic.Initialize(isEquipment ? 15 : 5);
            }
        }
    }
    
    public override void OnRoomCleared() { }
}