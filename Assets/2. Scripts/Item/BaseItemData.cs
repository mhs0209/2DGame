using UnityEngine;

public enum ItemCategory { Passive, Active, Consumable }

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemData")]
public abstract class BaseItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemCategory category;
    [TextArea] public string description;
    public Sprite icon;

    // 아이템 획득 시 실행될 공통 로직 (가상 메서드)
    public virtual void OnEquip() { Debug.Log($"{itemName} 장착됨"); }
}