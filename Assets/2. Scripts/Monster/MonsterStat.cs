using UnityEngine;
public enum MonsterType { Normal, Range, Named, Boss }

public class MonsterStat : Stat
{
    public MonsterType monsterType;
    public bool isNamed;

    private void Awake()
    {
        if (monsterType == MonsterType.Named || isNamed)
        {
            ApplyNamedBonus();
        }
    }

    private void ApplyNamedBonus()
    {
        atk *= 1.5f;
        maxHealth *= 2f;
        health = maxHealth;
        transform.localScale *= 1.2f; // 네임드는 조금 더 크게
    }
}