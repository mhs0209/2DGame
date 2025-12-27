using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Item", menuName = "Items/Active")]
public class ActiveItemData : ItemData
{
    public float cooldown; // 재사용 대기시간
    public int maxCharges; // 최대 충전량 (아이작 방식)
    public GameObject activeEffectPrefab; // 사용 시 발생할 이펙트/투사체

    private void OnEnable() => type = ItemType.Active;
}
