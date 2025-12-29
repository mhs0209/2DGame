using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Map", menuName = "Map/Shop")]
public class ShopMap : BaseRoomData 
{
    //public GameObject pedestalPrefab;
    public GameObject[] shopItemPool;
    public GameObject[] pickupPool;   // 소모품(하트, 열쇠 등)
    //public Vector2Int priceRange = new Vector2Int(5, 15);
}