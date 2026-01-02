using System.Collections;
using UnityEngine;

public class ActiveInvincible : Active
{
    public override void Use(GameObject player)
    {
        if (activeData == null) return;
        StartCoroutine(ActiveInvincibleRoutine(player.GetComponent<PlayerStat>()));
    }

    private IEnumerator ActiveInvincibleRoutine(PlayerStat stat)
    {
        stat.isInvincible = true;
        
        // 액티브 사용 시에는 특별한 색상으로 표시 (예: 황금색)
        SpriteRenderer[] renderers = stat.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            if (sr == null) continue;
            sr.color = Color.yellow; 
        }

        yield return new WaitForSeconds(activeData.duration); // SO의 지속시간 사용

        foreach (var sr in renderers)
        {
            if (sr == null) continue;
            sr.color = Color.white; 
        }
        stat.isInvincible = false;
    }
}