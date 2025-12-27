using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Item", menuName = "Items/Passive")]
public class PassiveItemData : ItemData
{
    [Header("Stat Boosts")]
    public float damageMod;
    public float speedMod;
    public float attackSpeedMod;
    public int maxHealthMod;

    private void OnEnable() => type = ItemType.Passive;
}