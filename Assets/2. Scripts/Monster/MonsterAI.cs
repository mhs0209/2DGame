using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    private MonsterStat stat;
    private Weapon weapon;
    private Transform player;
    private Rigidbody2D rb;

    [Header("AI Settings")]
    public float stopDistance = 4f; // 원거리 몹 유지 거리
    public float retreatDistance = 2f; // 너무 가까우면 도망갈 거리

    void Start()
    {
        stat = GetComponent<MonsterStat>();
        weapon = GetComponent<Weapon>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || stat.health <= 0) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (stat.monsterType)
        {
            case MonsterType.Normal:
                MoveToPlayer();
                break;
            case MonsterType.Range:
                HandleRangeAttack(distance);
                break;
            case MonsterType.Boss:
                HandleBossPattern(distance);
                break;
        }
    }

    // 1. 근접 몬스터: 무조건 플레이어 방향으로 이동
    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * stat.speed;
    }

    // 2. 원거리 몬스터: 거리 유지 및 사격
    void HandleRangeAttack(float dist)
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (dist > stopDistance) // 멀면 다가감
        {
            rb.velocity = direction * stat.speed;
        }
        else if (dist < retreatDistance) // 너무 가까우면 도망감
        {
            rb.velocity = -direction * stat.speed;
        }
        else // 적정 거리면 정지 후 공격
        {
            rb.velocity = Vector2.zero;
            weapon.Fire(direction);
        }
    }

    // 3. 보스: 거리에 따라 패턴 섞기 (예시)
    private float patternTimer;
    void HandleBossPattern(float dist)
    {
        patternTimer += Time.deltaTime;
        Vector2 direction = (player.position - transform.position).normalized;

        if (patternTimer > 2f) // 2초마다 패턴 변경 시도
        {
            // 예: 보스는 이동하면서 공격 방향으로 사격
            weapon.Fire(direction);
            if (patternTimer > 2.5f) patternTimer = 0;
        }
        MoveToPlayer(); // 보스는 기본적으로 압박하며 이동
    }
}