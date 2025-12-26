using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialRoomType { Roulette, Sacrifice, Library }
[CreateAssetMenu(fileName = "Special Map", menuName = "Map/Special")]
public class SpecialMap : BaseRoomData 
{
    public SpecialRoomType type;
    public GameObject gimmickPrefab; // 룰렛기계 등
    public float specialChance = 0.5f;
}