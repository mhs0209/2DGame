using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootEntry 
{
    public GameObject prefab;
    [Range(0, 100)] public float weight; // 가중치 (전체 가중치 합 중 비중)
}

[CreateAssetMenu(fileName = "Normal Map", menuName = "Map/Normal")]
public class NormalMap : BaseRoomData 
{
    public LootEntry[] lootTable; // 전투 후 픽업 아이템 보상 목록
}