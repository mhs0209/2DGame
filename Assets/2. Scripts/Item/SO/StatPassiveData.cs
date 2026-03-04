using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/StatModifier")]
public class StatPassiveData : ItemData
{
    public StatModifier[] modifiers;
    private void OnEnable() => type = ItemType.Passive;
}