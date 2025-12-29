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
        SpriteRenderer sr = stat.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = Color.yellow; 

        yield return new WaitForSeconds(activeData.duration); // SO의 지속시간 사용

        sr.color = originalColor;
        stat.isInvincible = false;
    }
}