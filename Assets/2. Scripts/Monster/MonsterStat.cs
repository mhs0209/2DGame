using UnityEngine;
public enum MonsterType { Normal, Range, Named, Boss }

public class MonsterStat : Stat
{
    public MonsterType monsterType;
    public bool isNamed;

    protected override void Awake()
    {
        if (monsterType == MonsterType.Named || isNamed)
        {
            ApplyNamedBonus();
        }
    }
    
    protected override void Die()
    {
        Debug.Log("몬스터 사망 - 아이템 드랍 및 풀링 반환");
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    private void ApplyNamedBonus()
    {
        atk *= 1.5f;
        maxHealth *= 2f;
        health = maxHealth;
        transform.localScale *= 1.2f; // 네임드는 조금 더 크게
    }

    private void ResetNamedBonus()
    {
        
    }
}