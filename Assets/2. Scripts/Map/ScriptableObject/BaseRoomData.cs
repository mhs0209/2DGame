using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Boss, Shop, Treasure, Special, Base }

public abstract class BaseRoomData : ScriptableObject
{
    public int roomID;
    public RoomType roomType;
    public GameObject roomPrefab;
    public Sprite minimapIcon;
    
    // 이 배열(리스트)을 테이블을 통해 자동으로 채울 것입니다.
    public List<ItemData> itemDropPool = new List<ItemData>(); 
}