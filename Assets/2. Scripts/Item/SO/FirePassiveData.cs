using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/FireModifier")]
public class FirePassiveData : ItemData
{
    public FireShape shapeChange;
    public int addProjectileCount;
    public bool setPiercing;
    private void OnEnable() => type = ItemType.Passive;
}