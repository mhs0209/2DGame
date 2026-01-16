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
        // 1. 보정 전 원본 스탯 값들을 저장 (딕셔너리나 리스트 활용)
        // 여기서는 간단하게 연산 전의 실제 값들을 백업합니다.
        System.Collections.Generic.Dictionary<StatType, float> originalValues = new();
    
        foreach (var mod in activeData.modifiers)
        {
            float val = mod.statType switch {
                StatType.Atk => stat.atk,
                StatType.Speed => stat.speed,
                StatType.Range => stat.range,
                StatType.Delay => stat.delay,
                _ => 0f
            };
            originalValues[mod.statType] = val;

            // 즉시 스탯 증가
            stat.ApplyStatChange(mod.statType, mod.value);
        }

        if (activeData.isDecaying)
        {
            float elapsed = 0f;
            while (elapsed < activeData.duration)
            {
                float dt = Time.deltaTime;
                elapsed += dt;

                foreach (var mod in activeData.modifiers)
                {
                    float decayAmount = (mod.value / activeData.duration) * dt;
                    stat.ApplyStatChange(mod.statType, -decayAmount);
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(activeData.duration);
            // 즉시 제거 방식은 오차가 거의 없지만, 일관성을 위해 아래에서 처리
        }

        // 2. [핵심] 최종 보정 로직
        // 연산 오차로 인해 원본과 달라진 값을 계산하여 정확히 원상복구 시킵니다.
        foreach (var mod in activeData.modifiers)
        {
            float currentVal = mod.statType switch {
                StatType.Atk => stat.atk,
                StatType.Speed => stat.speed,
                StatType.Range => stat.range,
                StatType.Delay => stat.delay,
                _ => originalValues[mod.statType]
            };

            // 오차만큼 다시 더하거나 빼줌 (original - current = 차이값)
            float correction = originalValues[mod.statType] - currentVal;
            stat.ApplyStatChange(mod.statType, correction);
        }
    
        stat.OnStatChanged?.Invoke();
        Debug.Log("스탯 부스트 종료 및 소수점 오차 보정 완료");
    }
}