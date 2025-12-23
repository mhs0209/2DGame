using UnityEngine;

public enum RoomType { Normal, Boss, Shop, Treasure, Special, Base }

public abstract class BaseRoomData : ScriptableObject
{
    public int roomID;
    public RoomType roomType;
    public GameObject roomPrefab;
    public Sprite minimapIcon;
}