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

        if (controller.roomData is ShopMap sData) 
        {
            // 상점은 현재 아이템이 픽업인지 장비인지 판별하여 풀 결정 (가격 등으로 판별 가능)
            // 여기서는 단순화하여 전체 풀을 합치거나 sData.shopItemPool 사용
            pool = sData.shopItemPool; 
        }
        else {
            // 보물, 보스, 노말, 특수방은 모두 itemDropPool을 공통으로 사용
            pool = controller.roomData.itemDropPool;
        }

        if (pool == null || pool.Count == 0) return;

        // 가중치 리롤을 원하시면 위에서 만든 가중치 함수를 쓰시고, 아니면 일반 랜덤
        ItemData newData = pool[Random.Range(0, pool.Count)];
        Instantiate(newData.itemPrefab, oldItem.transform.position, Quaternion.identity, oldItem.transform.parent);
        Destroy(oldItem.gameObject);
    }
    
}