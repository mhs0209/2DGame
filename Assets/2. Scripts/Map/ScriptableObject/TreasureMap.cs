using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Treasure Map", menuName = "Map/Treasure")]
public class TreasureMap : BaseRoomData 
{
    public GameObject pedestalPrefab; // 아이템 제단
    //public GameObject[] itemPool;     // 등장 가능한 아이템들
}
