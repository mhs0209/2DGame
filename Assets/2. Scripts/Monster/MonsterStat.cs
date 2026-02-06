using System.Collections;
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
        SetSpritesColor(Color.white);
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
        SetSpritesColor(Color.gray);
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
    
    private Coroutine hitEffectCoroutine;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (health > 0)
        {
            // 핵심: 기존에 돌고 있던 깜빡이 코루틴을 "확실히" 강제 종료
            if (hitEffectCoroutine != null) 
            {
                StopCoroutine(hitEffectCoroutine);
            }
            hitEffectCoroutine = StartCoroutine(HitFlashRoutine());
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        // 1. 빨간색으로 변경
        SetSpritesColor(Color.red);

        // 2. 대기 (시간을 0.15f 정도로 살짝 늘려보세요)
        yield return new WaitForSeconds(0.15f);

        // 3. 원래 색상으로 복구
        Color recoveryColor = isNamed ? Color.gray : Color.white;
        SetSpritesColor(recoveryColor);

        hitEffectCoroutine = null;
    }

    // 중복 코드를 줄이기 위한 헬퍼 함수
    private void SetSpritesColor(Color targetColor)
    {
        if (childrenRenderers == null) return;

        foreach (var sr in childrenRenderers)
        {
            if (sr != null)
            {
                // .color 대신 .material.color를 수정합니다.
                // 이렇게 하면 애니메이터의 감시망을 피해서 색을 바꿀 수 있습니다.
                sr.material.color = targetColor;
            }
        }
    }
    
    private void OnDestroy()
    {
        // 생성된 머티리얼 인스턴스들을 제거하여 메모리 누수를 방지합니다.
        if (childrenRenderers != null)
        {
            foreach (var sr in childrenRenderers)
            {
                if (sr != null && sr.material != null)
                {
                    Destroy(sr.material);
                }
            }
        }
    }
}