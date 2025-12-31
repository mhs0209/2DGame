using UnityEngine;

public class MonsterWeapon : Weapon
{
    private MonsterStat stat;

    protected override void Awake()
    {
        base.Awake();
        stat = GetComponent<MonsterStat>();
    }
}