using UnityEngine;
public enum MonsterType { Normal, Range, Named, Boss }

public class MonsterStat : Stat
{
    public MonsterType monsterType;
    public bool isNamed;
    public RoomController myRoom;

    //[Header("Scaling Settings")]
    private float growthRate = 1.5f; // 스테이지별 스탯 증가율

    private float originAtk;
    private float originMaxHealth;
    private float originSpeed;

    protected override void Awake()
    {
        base.Awake();
        originAtk = atk;
        originMaxHealth = maxHealth;
        originSpeed = speed;
    }

    private void OnEnable()
    {
        ResetStat();           // 1. 원본 데이터로 초기화
        ApplyStageScaling();   // 2. 스테이지에 따른 배율 적용
        
        // 3. 10% 확률로 네임드 보너스 (스테이지 스케일링이 끝난 후 추가 보너스)
        if (Random.value < 0.1f) ApplyNamedBonus();
    }

    private void ResetStat()
    {
        isNamed = false;
        atk = originAtk;
        maxHealth = originMaxHealth;
        speed = originSpeed;
        health = maxHealth;
        isInvincible = false;
        if(childrenRenderers[0]) childrenRenderers[0].color = new Color(1,1,1, childrenRenderers[0].color.a);
    }

    private void ApplyStageScaling()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // "Stage01"에서 뒤의 숫자 두 자리를 가져옴
        if (sceneName.Length >= 2 && int.TryParse(sceneName.Substring(sceneName.Length - 2), out int stageNum))
        {
            // 체력 복리 계산: Base * (Growth ^ (Stage-1))
            float hpMultiplier = Mathf.Pow(growthRate, stageNum - 1);
            maxHealth *= hpMultiplier;
            health = maxHealth;

            // 공격력 결정: 1~4스테이지는 1, 5스테이지부터는 2 (네임드는 아래에서 별도 처리)
            atk = (stageNum >= 5) ? 2 : 1;
        }
    }

    private void ApplyNamedBonus()
    {
        isNamed = true;
        atk = 2; // 네임드는 스테이지 상관없이 무조건 2
        maxHealth *= 1.5f;
        health = maxHealth;
        speed *= 1.2f;
        if(childrenRenderers[0]) childrenRenderers[0].color = new Color(1f, 0f, 0f,childrenRenderers[0].color.a); // 네임드는 색깔 표시
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 닿아 있는 동안 계속 시도하지만, 
        // 플레이어의 TakeDamage 내부에서 'isInvincible'로 걸러줌
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerStat playerStat))
            {
                playerStat.TakeDamage(atk); 
            }
        }
    }

    protected override void Die()
    {
        // 방 매니저에게 죽었다고 알림
        if (myRoom != null) myRoom.OnEnemyDeath(this.gameObject);

        // [변경] 오브젝트 풀링 대신 즉시 파괴
        Destroy(gameObject);
    }
}