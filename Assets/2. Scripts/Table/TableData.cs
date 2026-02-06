using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableData : MonoBehaviour
{
    
}

// 아이템 테이블 데이터
public class ItemTableData {
    public int ID;
    public string Name;
    public ItemType ItemType;
    public bool InTreasure, InBoss, InShop, InSpecial, InNormal;
    public float DropWeight;
}

// 맵 테이블 데이터
public class MapTableData {
    public int ID;
    public string PrefabName;
    public RoomType RoomType;
    public int MinStage, MaxStage;
}