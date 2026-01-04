using System;
using UnityEngine;

public class PlayerStat : Stat 
{
    public int gold, keys, bombs;
    
    // 최대 체력 상한선
    private const float MAX_HEALTH_LIMIT = 20f;

    // 체력 변경 시 UI에 알리기 위한 이벤트
    public Action OnStatChanged;
    public Action OnHealthChanged;
    public Action OnMaxHealthChanged;
    
    
    protected override void Awake()
    {
        base.Awake();
        // 플레이어는 피격 무적 기능을 항상 사용함
        useHitInvincibility = true; 
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHealthChanged?.Invoke(); // 피격 시 호출
    }

    protected override void Die()
    {
        GameManager.Instance.OnPlayerDeath();
    }
    
    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke(); // 회복 시 호출
    }
    
    // 재화 추가 및 이벤트 호출
    public void AddGold(int amount) { gold += amount; OnStatChanged?.Invoke(); }
    public void AddKey(int amount) { keys += amount; OnStatChanged?.Invoke(); }
    public void AddBomb(int amount) { bombs += amount; OnStatChanged?.Invoke(); }
    
    // 폭탄 사용 가능 여부 확인 및 차감
    public bool UseBomb()
    {
        if (bombs > 0)
        {
            bombs--;
            OnStatChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public override void ApplyStatChange(StatType type, float value)
    {
        // 부모의 기본 로직을 먼저 실행하여 수치를 변경합니다.
        base.ApplyStatChange(type, value);
        // 수치 변경 후 UI 갱신 이벤트 호출
        OnStatChanged?.Invoke();
    
        // 변경된 타입에 따라 후속 처리를 진행합니다.
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