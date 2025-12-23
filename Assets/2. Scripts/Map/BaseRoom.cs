using UnityEngine;

public class BaseRoom : MonoBehaviour
{
    public Vector2Int gridPos;
    public RoomType type;

    [System.Serializable]
    public struct DoorSet {
        public GameObject door; // 문 오브젝트
        public GameObject wall; // 벽 오브젝트
    }

    public DoorSet top, bottom, left, right;

    public void SetupDoors(System.Func<Vector2Int, bool> hasRoomAt)
    {
        SetDoorState(top, hasRoomAt(gridPos + Vector2Int.up));
        SetDoorState(bottom, hasRoomAt(gridPos + Vector2Int.down));
        SetDoorState(left, hasRoomAt(gridPos + Vector2Int.left));
        SetDoorState(right, hasRoomAt(gridPos + Vector2Int.right));
    }
    
    // BaseRoom.cs 에 추가
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    public virtual void OnPlayerEnter()
    {
        // 1. 현재 방 방문 표시
        MinimapManager.Instance.UpdateRoomIcon(gridPos, true);

        // 2. 인접한 방들은 '발견됨(?)' 상태로 표시
        Vector2Int[] neighbors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in neighbors)
        {
            MinimapManager.Instance.UpdateRoomIcon(gridPos + dir, false);
        }
    }

    private void SetDoorState(DoorSet doorSet, bool exists)
    {
        if (doorSet.door != null) doorSet.door.SetActive(exists);
        if (doorSet.wall != null) doorSet.wall.SetActive(!exists);
    }
}