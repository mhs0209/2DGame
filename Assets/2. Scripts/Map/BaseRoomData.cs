using UnityEngine;

public enum RoomType { Normal, Start, Boss, Shop, Treasure, Special }

[CreateAssetMenu(fileName = "RoomData", menuName = "RoomData")]
public abstract class BaseRoomData : ScriptableObject
{
    public RoomType roomType;
    public GameObject roomPrefab;
    public Sprite minimapIcon;
}