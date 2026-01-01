using UnityEngine;

public enum DoorDirection { Top, Bottom, Left, Right }

public class RoomTransfer : MonoBehaviour
{
    public DoorDirection direction;
    // 문에서 플레이어가 소환될 거리 (방 크기에 맞춰 조절)
    private float verticalEntryOffset = 6f; 
    private float horizontalEntryOffset = 2.0f; 

    private static bool isTransferring = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransferring)
        {
            TransferPlayer(collision.transform);
        }
    }

    private void TransferPlayer(Transform player)
    {
        isTransferring = true;

        float spacing = MapGenerator.Instance.roomSpacing;
        
        // 1. 현재 격자 좌표 및 목표 격자 계산
        Vector2Int currentGrid = new Vector2Int(
            Mathf.RoundToInt(player.position.x / spacing),
            Mathf.RoundToInt(player.position.y / spacing)
        );

        Vector2Int targetGrid = currentGrid;
        Vector3 spawnPosition = Vector3.zero;

        // 2. 방향에 따른 '다음 방의 문 앞' 좌표 계산
        switch (direction)
        {
            case DoorDirection.Top:
                targetGrid += Vector2Int.up;
                // 위쪽 방의 '아래쪽 문 앞' (중심에서 아래로 이동)
                spawnPosition = new Vector3(targetGrid.x * spacing, targetGrid.y * spacing - (spacing / 2 - verticalEntryOffset), 0);
                break;
            case DoorDirection.Bottom:
                targetGrid += Vector2Int.down;
                // 아래쪽 방의 '위쪽 문 앞' (중심에서 위로 이동)
                spawnPosition = new Vector3(targetGrid.x * spacing, targetGrid.y * spacing + (spacing / 2 - verticalEntryOffset), 0);
                break;
            case DoorDirection.Left:
                targetGrid += Vector2Int.left;
                // 왼쪽 방의 '오른쪽 문 앞' (중심에서 오른쪽으로 이동)
                spawnPosition = new Vector3(targetGrid.x * spacing + (spacing / 2 - horizontalEntryOffset), targetGrid.y * spacing, 0);
                break;
            case DoorDirection.Right:
                targetGrid += Vector2Int.right;
                // 오른쪽 방의 '왼쪽 문 앞' (중심에서 왼쪽으로 이동)
                spawnPosition = new Vector3(targetGrid.x * spacing - (spacing / 2 - horizontalEntryOffset), targetGrid.y * spacing, 0);
                break;
        }

        // 3. 카메라 및 플레이어 즉시 이동 (부드러운 이동 삭제)
        Vector3 targetRoomCenter = new Vector3(targetGrid.x * spacing, targetGrid.y * spacing, -10);
        CameraManager.Instance.ImmediateMove(targetRoomCenter);
        player.position = spawnPosition;
        
        AstarPath.active.Scan();
        
        // 4. [추가] 이동한 방의 미니맵 강제 업데이트
        if (MapGenerator.Instance.GetRoomAt(targetGrid) != null)
        {
            MapGenerator.Instance.GetRoomAt(targetGrid).OnPlayerEnter();
        }

        // 5. 쿨타임 (연쇄 이동 방지)
        Invoke("ResetTransferFlag", 0.2f);
    }

    private void ResetTransferFlag() => isTransferring = false;
}