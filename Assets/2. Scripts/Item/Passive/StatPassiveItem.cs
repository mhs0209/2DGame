using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPassiveItem : Passive
{
    public StatPassiveData statData;
    public override void ApplyEffect(PlayerStat stat)
    {
        // [추가] 획득 기록
        RunDataManager.Instance.RecordPassive(statData);
        foreach (var mod in statData.modifiers)
        {
            stat.ApplyStatChange(mod.statType, mod.value);
        }
    }
}