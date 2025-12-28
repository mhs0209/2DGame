using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePassiveItem : Passive
{
    public FirePassiveData fireData;
    public override void ApplyEffect(PlayerStat stat)
    {
        // 시너지 로직: 샷건은 다른 모든 멀티 발사를 덮어씀
        if (fireData.shapeChange == FireShape.Shotgun || stat.fireShape == FireShape.Shotgun)
        {
            stat.fireShape = FireShape.Shotgun;
            stat.projectileCount = 10; // 샷건 고정값 예시
            stat.range *= 0.5f;        // 샷건 사거리 감소
        }
        else
        {
            stat.fireShape = fireData.shapeChange;
            stat.projectileCount += fireData.addProjectileCount;
        }
        
        if (fireData.setPiercing) stat.isPiercing = true;
    }
}