using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Normal Room Data", menuName = "RoomData/NormalRoomData")]
public class NormalMap : BaseRoomData
{
    public GameObject[] enemyPrefabs;
    public int minEnemies = 2;
    public int maxEnemies = 5;
    //public GameObject rewardPrefab;
}
