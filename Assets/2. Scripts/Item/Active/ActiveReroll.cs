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
        GameObject[] pool = null;

        // RoomController의 roomData와 baseRoom.type을 활용하여 풀 결정
        if (controller.roomData is TreasureMap tData) pool = tData.itemPool;
        else if (controller.roomData is ShopMap sData) pool = sData.shopItemPool;
        else if (controller.roomData is BossMap bData) pool = bData.rewardPool;
        
        // 특수방 처리 (SpecialRoom은 TreasureMap 데이터를 공유한다고 하셨으므로)
        if (controller.baseRoom.type == RoomType.Special && pool == null)
        {
            if (controller.roomData is TreasureMap specData) pool = specData.itemPool;
        }

        if (pool == null || pool.Length == 0) return;

        // 새 아이템 생성 및 기존 아이템 파괴
        GameObject newItemPrefab = pool[Random.Range(0, pool.Length)];
        Instantiate(newItemPrefab, oldItem.transform.position, Quaternion.identity, oldItem.transform.parent);
        Destroy(oldItem.gameObject);
    }
    
}