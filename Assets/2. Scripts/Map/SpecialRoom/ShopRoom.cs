using UnityEngine;

public class ShopRoom : ItemRoom 
{
    public GameObject[] shopSpots; 

    private void Start() 
    {
        if (controller.roomData is ShopMap data) 
        {
            for (int i = 0; i < shopSpots.Length; i++) 
            {
                // 1. 아이템 스폰
                GameObject itemPrefab = (i < 2) ? data.shopItemPool[Random.Range(0, data.shopItemPool.Length)] 
                    : data.pickupPool[Random.Range(0, data.pickupPool.Length)];
                
                GameObject spawnedItem = Instantiate(itemPrefab, shopSpots[i].transform.position + Vector3.up * 0.5f, Quaternion.identity, shopSpots[i].transform);
                
                // 2. 상점 전용 구매 스크립트 동적 부착
                ShopItem shopLogic = spawnedItem.AddComponent<ShopItem>();
                
                // 3. 가격 설정 (픽업 5, 패시브/액티브 15)
                int price = (i < 2) ? 15 : 5; 
                shopLogic.Initialize(price);
            }
        }
    }
    public override void OnRoomCleared() { }
}