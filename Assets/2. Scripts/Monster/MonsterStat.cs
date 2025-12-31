using UnityEngine;
public enum MonsterType { Normal, Range, Named, Boss }

public class MonsterStat : Stat
{
    public MonsterType monsterType;
    public bool isNamed;
    public RoomController myRoom;

    // 기본 스탯 저장용 (풀링 복구용)
    private float originAtk;
    private float originMaxHealth;
    private Vector3 originScale;
    private float originSpeed;

    protected override void Awake()
    {
        base.Awake();
        // 최초 초기 상태 저장
        originAtk = atk;
        originMaxHealth = maxHealth;
        originScale = transform.localScale;
        originSpeed = speed;
    }

    private void OnEnable()
    {
        ResetStat();
        // 10% 확률로 네임드 몬스터화 (원하는 확률로 조정 가능)
        if (Random.value < 0.1f) ApplyNamedBonus();
    }

    private void ResetStat()
    {
        isNamed = false;
        atk = originAtk;
        maxHealth = originMaxHealth;
        health = maxHealth;
        transform.localScale = originScale;
        isInvincible = false;
    }

    private void ApplyNamedBonus()
    {
        isNamed = true;
        atk = 2;
        maxHealth *= 1.5f;
        health = maxHealth;
        transform.localScale *= 1.2f;
        speed *= 1.5f;
        // 시각적 차이를 위해 색상을 변경할 수도 있습니다.
        if(spriteRenderer) spriteRenderer.color = new Color(1f, 0.6f, 0.6f); 
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