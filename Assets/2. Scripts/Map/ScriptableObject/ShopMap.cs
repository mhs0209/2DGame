using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Room Data", menuName = "RoomData/ShopRoomData")]
public class ShopMap : BaseRoomData
{
    public GameObject itemPrefab;
    public GameObject[] pickupPrefab;
    public int[] price;
}
