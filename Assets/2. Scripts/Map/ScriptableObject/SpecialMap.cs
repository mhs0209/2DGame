using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Special Map", menuName = "Map/Special")]
public class SpecialMap : BaseRoomData 
{
    public GameObject pedestalPrefab; // 아이템 제단
    //public GameObject[] itemPool;     // 등장 가능한 아이템들
}