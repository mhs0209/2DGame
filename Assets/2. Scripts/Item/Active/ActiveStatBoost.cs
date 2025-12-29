using UnityEngine;
using System.Collections;

public class ActiveStatBoost : Active
{
    public override void Use(GameObject player)
    {
        if (activeData == null) return;
        StartCoroutine(StatBoostRoutine(player.GetComponent<PlayerStat>()));
    }

    private IEnumerator StatBoostRoutine(PlayerStat stat)
    {
        // 1. 즉시 스탯 증가
        foreach (var mod in activeData.modifiers)
        {
            stat.ApplyStatChange(mod.statType, mod.value);
        }

        if (activeData.isDecaying)
        {
            // 2-A. 서서히 감소하는 로직 (Decay)
            float elapsed = 0f;
            while (elapsed < activeData.duration)
            {
                float dt = Time.deltaTime;
                elapsed += dt;

                foreach (var mod in activeData.modifiers)
                {
                    // 매 프레임 (전체 증가량 / 지속시간)만큼 빼줌
                    float decayAmount = (mod.value / activeData.duration) * dt;
                    stat.ApplyStatChange(mod.statType, -decayAmount);
                }
                yield return null;
            }
        }
        else
        {
            // 2-B. 지속시간 동안 유지 후 한 번에 복구
            yield return new WaitForSeconds(activeData.duration);

            foreach (var mod in activeData.modifiers)
            {
                stat.ApplyStatChange(mod.statType, -mod.value);
            }
        }
    }
}