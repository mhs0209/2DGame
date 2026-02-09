using UnityEngine;
using System.Collections.Generic;

public class ActiveReroll : Active
{
    public override void Use(GameObject player)
    {
        // 현재 방 컨트롤러를 바로 가져옴
        RoomController controller = RoomController.CurrentRoom;

        if (controller != null)
        {
            ItemObject[] items = controller.GetComponentsInChildren<ItemObject>();
            foreach (var item in items)
            {
                RerollItem(item, controller);
            }
        }
    }

    private void RerollItem(ItemObject oldItem, RoomController controller)
    {
        List<ItemData> pool = null;
        int currentPrice = 0;
        int currentKey = 0;

        // 상점 아이템인 경우 가격 정보를 가져옴
        if (oldItem.TryGetComponent<ShopItem>(out var shopItem)) {
            currentPrice = shopItem.price; // 5G 또는 15G
        }
        
        // 상점 아이템인 경우 가격 정보를 가져옴
        if (oldItem.TryGetComponent<SpecialItem>(out var specialItem))
        {
            currentKey = specialItem.requiredKeys;
        }

        if (controller.roomData is ShopMap sData) {
            // 가격이 15G면 장비 풀, 5G면 픽업 풀에서 리롤
            pool = (currentPrice >= 15) ? sData.shopItemPool : sData.pickupPool;
        } else {
            pool = controller.roomData.itemDropPool;
        }

        if (pool == null || pool.Count == 0) return;

        ItemData newData = pool[Random.Range(0, pool.Count)];
    
        // 생성 후 다시 ShopItem 컴포넌트 설정 (상점이라면)
        GameObject newItem = Instantiate(newData.itemPrefab, oldItem.transform.position, Quaternion.identity, oldItem.transform.parent);
        
        if (currentPrice > 0) {
            ShopItem newShopLogic = newItem.AddComponent<ShopItem>();
            newShopLogic.Initialize(currentPrice);
        }
        if (currentKey > 0)
        {
            SpecialItem newSpecialItem = newItem.AddComponent<SpecialItem>();
            newSpecialItem.Initialize(currentKey);
        }
    
        Destroy(oldItem.gameObject);
    }
    
}