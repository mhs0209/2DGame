using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialType { Roulette, Augment, ItemGamble }
[CreateAssetMenu(fileName = "Special Room Data", menuName = "RoomData/SpecialRoomData")]
public class SpecialMap : BaseRoomData
{
    public SpecialType specialType;
    public float winChance = 0.3f; // 룰렛/아이템 획득 확률
}
