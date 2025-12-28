using UnityEngine;

public class PlayerStat : Stat 
{
    public int gold, keys, bombs;

    public void Heal(float amount) => health = Mathf.Min(health + amount, maxHealth);
    public void AddGold(int amount) => gold += amount;
    public void AddKey(int amount) => keys += amount;
    public void AddBomb(int amount) => bombs += amount;
}