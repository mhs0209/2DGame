using UnityEngine;
using Pathfinding; // A* 라이브러리 필요

public enum AIState { Idle, Chase, Attack, Retreat }

public class MonsterAI : MonoBehaviour
{
    private MonsterStat stat;
    private MonsterWeapon weapon;
    private Transform player;
    private AIPath aiPath; // A* 컴포넌트
    private Rigidbody2D rb;

    public AIState currentState = AIState.Idle;

    [Header("AI Settings")]
    public float detectRange = 10f;
    public float attackRange = 5f;
    public float retreatRange = 2f;

    [Header("Boss/Pattern Settings")]
    private float patternTimer;      // 에러 해결: 변수 선언
    private int bossPatternIndex = 0;

    void Awake()
    {
        stat = GetComponent<MonsterStat>();
        weapon = GetComponent<MonsterWeapon>();
        aiPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (aiPath != null && stat != null)
        {
            aiPath.maxSpeed = stat.speed;
        }
    }

    void Update()
    {
        if (player == null || stat.health <= 0)
        {
            if(aiPath != null) aiPath.destination = transform.position;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // 1. 상태 결정 로직
        if (stat.monsterType == MonsterType.Boss)
        {
            HandleBossPattern(dist); // 보스는 별도 패턴 로직으로 관리
        }
        else
        {
            UpdateState(dist);
            HandleStateAction();
        }
        
        FlipSprite();
    }

    // 일반 몬스터 상태 전환
    void UpdateState(float dist)
    {
        float attackRange = stat.range;
        
        if (dist > detectRange) currentState = AIState.Idle;
        else if (dist <= attackRange && dist > retreatRange) currentState = AIState.Attack;
        else if (dist <= retreatRange && stat.monsterType == MonsterType.Range) currentState = AIState.Retreat;
        else currentState = AIState.Chase;
    }

    void HandleStateAction()
    {
        if (player == null) return;
    
        // 플레이어와의 방향 벡터
        Vector2 dir = (player.position - transform.position).normalized;

        switch (currentState)
        {
            case AIState.Idle:
                StopMovement();
                break;

            case AIState.Chase:
                // 목적지를 플레이어 위치로 실시간 갱신
                MoveToTarget(player.position);
                break;

            case AIState.Attack:
                if (stat.monsterType == MonsterType.Range)
                {
                    StopMovement(); // 원거리는 서서 사격
                    GetComponent<MonsterWeapon>()?.Fire(dir);
                }
                else
                {
                    // 노말 몬스터는 공격 상태에서도 플레이어에게 끝까지 붙어야 함
                    MoveToTarget(player.position);
                }
                break;

            case AIState.Retreat:
                // 도망 로직: 플레이어 반대 방향으로 2만큼 떨어진 '좌표'를 목적지로 설정
                // 단순히 위로 가지 않도록 Vector2 계산 확인
                Vector2 retreatTarget = (Vector2)transform.position - (dir * 2f);
                MoveToTarget(retreatTarget);
            
                // 도망치면서 발사
                GetComponent<MonsterWeapon>()?.Fire(dir);
                break;
        }
    }

    // --- 보스 패턴 로직 (에러 해결: MoveToPlayer, HandleRangeAttack 등 통합) ---
    void HandleBossPattern(float dist)
    {
        patternTimer += Time.deltaTime;
        Vector2 dir = (player.position - transform.position).normalized;

        // 3초마다 패턴 변경
        if (patternTimer > 3f)
        {
            bossPatternIndex = (bossPatternIndex + 1) % 2; 
            patternTimer = 0;
        }

        if (bossPatternIndex == 0) // 패턴 1: 추격 및 근접 압박
        {
            MoveToTarget(player.position);
        }
        else // 패턴 2: 정지 후 강력한 사격
        {
            StopMovement();
            weapon.Fire(dir);
        }
    }

    // --- 헬퍼 함수들 (코드 중복 방지 및 에러 해결) ---
    
    void MoveToTarget(Vector2 targetPos)
    {
        if (aiPath != null)
        {
            aiPath.canMove = true;
            // AIPath가 인식할 수 있도록 목적지를 전달
            aiPath.destination = new Vector3(targetPos.x, targetPos.y, 0); 
        }
    }

    void StopMovement()
    {
        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.destination = transform.position;
        }
        if (rb != null) rb.velocity = Vector2.zero;
    }

    void FlipSprite()
    {
        if (player == null) return;

        // 플레이어가 왼쪽에 있으면 왼쪽을, 오른쪽에 있으면 오른쪽을 보게 함
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // 왼쪽
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // 오른쪽
        }
    }
}