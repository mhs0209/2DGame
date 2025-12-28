using UnityEngine;

public class Pickup : ItemObject
{
    public PickupItemData data; // SO 데이터 연결

    public override void OnPickup(GameObject player)
    {
        // PlayerManager나 Inventory 스크립트가 있다고 가정하고 수치 변경
        PlayerStat stats = player.GetComponent<PlayerStat>(); 
        if (stats == null) return;

        switch (data.pickupType)
        {
            case PickupItemData.PickupType.Health: stats.Heal(data.value); break;
            case PickupItemData.PickupType.Gold:   stats.AddGold(data.value); break;
            case PickupItemData.PickupType.Key:    stats.AddKey(data.value); break;
            case PickupItemData.PickupType.Bomb:   stats.AddBomb(data.value); break;
        }
        Debug.Log($"{data.itemName} 획득! 수치 {data.value} 증가");
    }
}