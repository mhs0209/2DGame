using UnityEngine;

public class MonsterWeapon : Weapon
{
    private Transform player;
    public float detectRange = 10f;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || GetComponent<MonsterStat>().monsterType == MonsterType.Normal) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Fire(dir);
        }
    }
}