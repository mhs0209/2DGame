using UnityEngine;
using Pathfinding; // A* 라이브러리 필요

public enum AIState { Idle, Chase, Attack }

public class MonsterAI : MonoBehaviour
{
    private MonsterStat stat;
    private MonsterWeapon weapon;
    private Transform player;
    private AIPath aiPath; // A* 컴포넌트
    private Rigidbody2D rb;

    public AIState currentState = AIState.Idle;
    private AIState lastState = AIState.Idle; // 상태 변화 감지용
    public bool seeRight;

    [Header("AI Settings")]
    public float detectRange = 10f;
    public float attackRange = 5f;

    [Header("Boss/Pattern Settings")]
    private float patternTimer;      // 에러 해결: 변수 선언
    private int bossPatternIndex = 0;

    // --- 사운드 관련 변수 추가 ---
    [Header("Sound Settings")]
    private AudioSource audioSource;
    public AudioClip attackClip; // 뿅뿅 사운드
    public AudioClip chaseClip;  // 추격 사운드 (공통)
    private bool isChasingSoundPlaying = false;
    
    void Awake()
    {
        stat = GetComponent<MonsterStat>();
        weapon = GetComponent<MonsterWeapon>();
        aiPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        // AudioSource 자동 할당 (없으면 추가)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 3D 사운드 설정 (플레이어와 멀어지면 작게 들림)
        audioSource.spatialBlend = 1.0f; 
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (aiPath != null && stat != null) aiPath.maxSpeed = stat.speed;
    }

    void Update()
    {
        if (player == null || stat.health <= 0)
        {
            StopChaseSound(); // 죽거나 플레이어 없으면 소리 중지
            if(aiPath != null) aiPath.destination = transform.position;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (stat.monsterType == MonsterType.Boss) HandleBossPattern(dist);
        else
        {
            UpdateState(dist);
            HandleStateAction();
        }

        FlipSprite();
        lastState = currentState; // 이전 상태 기록
    }

    void UpdateState(float dist)
    {
        float attackRange = stat.range;
        if (dist > detectRange) currentState = AIState.Idle;
        else if (dist <= attackRange) currentState = AIState.Attack;
        else currentState = AIState.Chase;
    }

    void StartChaseSound()
    {
        if (chaseClip == null) return;
        audioSource.clip = chaseClip;
        audioSource.loop = true; // 추격은 계속 소리가 나야 함
        audioSource.Play();
        isChasingSoundPlaying = true;
    }

    void StopChaseSound()
    {
        if (isChasingSoundPlaying)
        {
            audioSource.Stop();
            isChasingSoundPlaying = false;
        }
    }

    void HandleStateAction()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;

        switch (currentState)
        {
            case AIState.Idle:
                StopMovement();
                break;
            case AIState.Chase:
                StartChaseSound();
                MoveToTarget(player.position);
                break;
            case AIState.Attack:
                if (stat.monsterType == MonsterType.Range)
                {
                    StopMovement();
                    if (weapon != null && weapon.CanFire()) // 무기 쿨타임 확인
                    {
                        StopChaseSound();
                        audioSource.PlayOneShot(attackClip); // (한 번만 재생)
                        weapon.Fire(dir);
                        StartChaseSound();
                    }
                }
                else
                {
                    MoveToTarget(player.position);
                }
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
            StartChaseSound();
        }
        else // 패턴 2: 정지 후 사격
        {
            StopMovement();
            if (weapon != null && weapon.CanFire()) // 무기 쿨타임 확인
            {
                StopChaseSound();
                audioSource.PlayOneShot(attackClip); // (한 번만 재생)
                weapon.Fire(dir);
                StartChaseSound();
            }
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

        float baseScaleX = Mathf.Abs(transform.localScale.x);
        bool isPlayerLeft = player.position.x < transform.position.x;

        // seeRight가 true이고 플레이어가 왼쪽이면 -1, 아니면 1
        // seeRight가 false이고 플레이어가 왼쪽이면 1, 아니면 -1
        float direction = (seeRight == isPlayerLeft) ? -1f : 1f;

        transform.localScale = new Vector3(baseScaleX * direction, transform.localScale.y, transform.localScale.z);
    }
}