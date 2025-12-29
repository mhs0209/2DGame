using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Item", menuName = "Items/Active")]
public class ActiveItemData : ItemData
{
    public int maxCharges; 
    public float duration; 
    public bool isDecaying; // true면 서서히 감소, false면 지속 시간 후 한 번에 원래대로

    [Header("Stat Boost")]
    public StatModifier[] modifiers;

    private void OnEnable() => type = ItemType.Active;
}