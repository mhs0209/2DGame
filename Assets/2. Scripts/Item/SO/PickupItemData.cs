using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pickup Item", menuName = "Items/Pickup")]
public class PickupItemData : ItemData
{
    public enum PickupType { Health, Gold, Key, Bomb }
    public PickupType pickupType;
    public int value; // 회복량 또는 획득 수량
    private void OnEnable() => type = ItemType.Pickup;
}
