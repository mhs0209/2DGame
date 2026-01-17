using UnityEngine;

public class ActiveRoomDamage : Active
{
    public override void Use(GameObject player)
    {
        PlayerStat stat = player.GetComponent<PlayerStat>();
        float damage = stat.atk * stat.atkMult;

        // 현재 씬에 활성화된 모든 적을 찾음 (전투 중인 방의 적들)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");

        foreach (var enemyObj in enemies)
        {
            // Enemy 컴포넌트(또는 Stat 컴포넌트)를 가져와 데미지 처리
            MonsterStat enemy = enemyObj.GetComponent<MonsterStat>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        Debug.Log("방 전체의 적에게 데미지를 입혔습니다.");
    }
}