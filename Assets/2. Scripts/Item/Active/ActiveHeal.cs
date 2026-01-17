using UnityEngine;

public class ActiveHeal : Active
{
    public override void Use(GameObject player)
    {
        if (activeData == null) return;
        PlayerStat stat = player.GetComponent<PlayerStat>();

        // modifiers[0]의 value만큼 회복하도록 설계
        float healAmount = activeData.modifiers[0].value;
        stat.Heal(healAmount);
        
        Debug.Log($"체력을 {healAmount}만큼 회복했습니다.");
    }
}