using System;
using UnityEngine;

public class PlayerStat : Stat 
{
    public int gold, keys, bombs;
    
    // 최대 체력 상한선
    private const float MAX_HEALTH_LIMIT = 20f;

    // 체력 변경 시 UI에 알리기 위한 이벤트
    public Action OnHealthChanged;
    public Action OnMaxHealthChanged;
    
    
    protected override void Awake()
    {
        base.Awake();
        // 플레이어는 피격 무적 기능을 항상 사용함
        useHitInvincibility = true; 
        invincibilityMult = 0.5f; // 예: 받은 데미지 * 0.5초 무적
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHealthChanged?.Invoke(); // 피격 시 호출
    }

    protected override void Die()
    {
        Debug.Log("플레이어 사망 - 게임 오버 UI 띄우기");
    }
    
    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke(); // 회복 시 호출
    }
    
    public void AddGold(int amount) => gold += amount;
    public void AddKey(int amount) => keys += amount;
    public void AddBomb(int amount) => bombs += amount;
    
    public override void ApplyStatChange(StatType type, float value)
    {
        // 1. 부모의 기본 로직을 먼저 실행하여 수치를 변경합니다.
        base.ApplyStatChange(type, value);
    
        // 2. 변경된 타입에 따라 후속 처리를 진행합니다.
        switch (type)
        {
            case StatType.MaxHealth:
                // 최대 체력 상한선(20) 적용
                maxHealth = Mathf.Min(maxHealth, MAX_HEALTH_LIMIT);
                // 최대 체력이 늘어나면 현재 체력도 그만큼 회복시켜주는 것이 일반적입니다.
                health = Mathf.Min(health + value, maxHealth); 
            
                // UI 하트 개수를 갱신하라고 신호를 보냅니다.
                OnMaxHealthChanged?.Invoke();
                OnHealthChanged?.Invoke();
                break;
    
            case StatType.Health:
                // 현재 체력 제한 적용
                health = Mathf.Min(health, maxHealth);
            
                // 하트 색상을 갱신하라고 신호를 보냅니다.
                OnHealthChanged?.Invoke();
                break;
        }
    }
}