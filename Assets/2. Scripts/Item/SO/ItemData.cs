using UnityEngine;

public enum ItemType { Pickup, Passive, Active }

public abstract class ItemData : ScriptableObject
{
    [Header("Base Info")]
    public int itemID;
    public string itemName;
    public ItemType type;
    public GameObject itemPrefab; // 필드에 드랍될 때의 외형
}