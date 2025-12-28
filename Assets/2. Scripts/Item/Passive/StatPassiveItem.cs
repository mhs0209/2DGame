using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPassiveItem : Passive
{
    public StatPassiveData statData;
    public override void ApplyEffect(PlayerStat stat)
    {
        foreach (var mod in statData.modifiers)
        {
            stat.ApplyStatChange(mod.statType, mod.value);
        }
    }
}