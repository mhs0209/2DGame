using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Normal Room Data", menuName = "RoomData/NormalRoomData")]
public class NormalMap : BaseRoomData {
    public GameObject[] enemyPrefabs;
    public int minEnemies = 2;
    public int maxEnemies = 5;
    public List<LootTable> lootTables; // 확률형 보상 목록
}

[System.Serializable]
public class LootTable {
    public GameObject rewardPrefab; // 생성할 프리팹 (돈, 열쇠 등)
    [Range(0, 100)] public float dropWeight; // 드랍 가중치/확률
}