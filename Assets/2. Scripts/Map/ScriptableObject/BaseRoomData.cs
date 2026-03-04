using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Boss, Shop, Treasure, Special, Base }

public abstract class BaseRoomData : ScriptableObject
{
    public int roomID;
    public RoomType roomType;
    public GameObject roomPrefab;
    public Sprite minimapIcon;
    
    // 테이블을 통해 자동으로 채워짐.
    public List<ItemData> itemDropPool = new List<ItemData>(); 
}