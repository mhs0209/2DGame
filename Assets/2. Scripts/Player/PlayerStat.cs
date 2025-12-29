using UnityEngine;

public class PlayerStat : Stat 
{
    public int gold, keys, bombs;
    
    protected override void Awake()
    {
        base.Awake();
        // 플레이어는 피격 무적 기능을 항상 사용함
        useHitInvincibility = true; 
        invincibilityMult = 0.5f; // 예: 받은 데미지 * 0.5초 무적
    }

    protected override void Die()
    {
        Debug.Log("플레이어 사망 - 게임 오버 UI 띄우기");
    }

    public void Heal(float amount) => health = Mathf.Min(health + amount, maxHealth);
    public void AddGold(int amount) => gold += amount;
    public void AddKey(int amount) => keys += amount;
    public void AddBomb(int amount) => bombs += amount;
}