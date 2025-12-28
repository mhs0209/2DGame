using UnityEngine;

public enum StatType { Health, MaxHealth, Atk, AtkMult, Delay, Range, Speed }
public enum FireShape { Base, Multi, Radial, Shotgun }

[System.Serializable]
public struct StatModifier
{
    public StatType statType;
    public float value;
}

public abstract class Stat : MonoBehaviour
{
    [Header("Basic Stats")]
    public float health;
    public float maxHealth;
    public float atk;
    public float atkMult = 1.0f;
    public float delay;
    public float range;
    public float speed;

    [Header("Projectile Info")]
    public FireShape fireShape = FireShape.Base;
    public int projectileCount = 1; 
    public bool isPiercing = false; // 관통 여부

    public virtual void ApplyStatChange(StatType type, float value)
    {
        switch (type)
        {
            case StatType.Health: health += value; break;
            case StatType.MaxHealth: maxHealth += value; break;
            case StatType.Atk: atk += value; break;
            case StatType.AtkMult: atkMult += value; break;
            case StatType.Delay: delay += value; break;
            case StatType.Range: range += value; break;
            case StatType.Speed: speed += value; break;
        }
    }
}