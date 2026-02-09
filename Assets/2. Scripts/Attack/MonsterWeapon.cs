using UnityEngine;

public class MonsterWeapon : Weapon
{

    protected override void Awake()
    {
        base.Awake();
        lastFireTime = Time.time;
    }
}