using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModel : MonoBehaviour { }

[System.Serializable]
public class ItemRow {
    public int id;
    public string name;
    public string itemtype; // ItemType
    public bool intreasure; // InTreasure
    public bool inboss;     // InBoss
    public bool inshop;     // InShop
    public bool inspecial;  // InSpecial
    public bool innormal;   // InNormal
    public int dropweight;  // DropWeight
}

[System.Serializable]
public class MapRow {
    public int id;
    public string prefabname; // PrefabName (이 부분이 Name과 다를 수 있음)
    public string roomtype;   // RoomType
    public int minstage;      // MinStage
    public int maxstage;      // MaxStage
}