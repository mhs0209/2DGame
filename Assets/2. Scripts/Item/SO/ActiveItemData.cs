using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Item", menuName = "Items/Active")]
public class ActiveItemData : ItemData
{
    public int maxCharges; // 최대 충전량 (아이작 방식)
    public GameObject activeEffectPrefab; // 사용 시 발생할 이펙트/투사체
    public float duration; // 지속 시간

    private void OnEnable() => type = ItemType.Active;
}
