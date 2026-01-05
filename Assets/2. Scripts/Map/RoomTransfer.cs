using UnityEngine;

public enum DoorDirection { Top, Bottom, Left, Right }

public class RoomTransfer : MonoBehaviour
{
    public DoorDirection direction;
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
        
        // 1. 목표 격자 좌표 계산
        Vector2Int currentGrid = new Vector2Int(
            Mathf.RoundToInt(player.position.x / spacing),
            Mathf.RoundToInt(player.position.y / spacing)
        );

        Vector2Int targetGrid = currentGrid;
        switch (direction)
        {
            case DoorDirection.Top: targetGrid += Vector2Int.up; break;
            case DoorDirection.Bottom: targetGrid += Vector2Int.down; break;
            case DoorDirection.Left: targetGrid += Vector2Int.left; break;
            case DoorDirection.Right: targetGrid += Vector2Int.right; break;
        }

        // 2. 목표 방 데이터 가져오기
        BaseRoom targetRoom = MapGenerator.Instance.GetRoomAt(targetGrid);
        if (targetRoom == null) 
        {
            isTransferring = false;
            return;
        }

        // 3. 지점 이동 (반대 방향 지점으로 소환)
        Transform dest = null;
        switch (direction)
        {
            case DoorDirection.Top: dest = targetRoom.bottomSpawn; break;
            case DoorDirection.Bottom: dest = targetRoom.topSpawn; break;
            case DoorDirection.Left: dest = targetRoom.rightSpawn; break;
            case DoorDirection.Right: dest = targetRoom.leftSpawn; break;
        }

        if (dest != null)
        {
            // 카메라 즉시 이동
            Vector3 cameraPos = new Vector3(targetGrid.x * spacing, targetGrid.y * spacing, -10);
            CameraManager.Instance.ImmediateMove(cameraPos);

            // 플레이어 위치 고정
            player.position = dest.position;

            // 방 진입 효과 (미니맵 등)
            targetRoom.OnPlayerEnter();
            
            // A* 경로 재계산 (필요 시)
            if (AstarPath.active != null) AstarPath.active.Scan();
        }

        // 0.5초면 연쇄 이동을 막기에 충분합니다.
        Invoke(nameof(ResetTransferFlag), 0.5f);
    }

    private void ResetTransferFlag() => isTransferring = false;
}