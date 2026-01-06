using UnityEngine;
using System.Collections;

public enum DoorDirection { Top, Bottom, Left, Right }

public class RoomTransfer : MonoBehaviour
{
    public DoorDirection direction;
    private static bool isTransferring = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        DoorPhysics myDoor = GetComponent<DoorPhysics>();

        if (collision.CompareTag("Player") && !isTransferring)
        {
            TransferPlayer(collision.transform);
        }
    }

    public void TransferPlayer(Transform player)
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

        BaseRoom targetRoom = MapGenerator.Instance.GetRoomAt(targetGrid);
        if (targetRoom == null) { isTransferring = false; return; }

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
            CameraManager.Instance.ImmediateMove(new Vector3(targetGrid.x * spacing, targetGrid.y * spacing, -10));
            player.position = dest.position;
        
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            // [핵심] 들어간 방의 입성 함수 호출 (이 안에서 잠금 로직이 실행됨)
            targetRoom.OnPlayerEnter();
        }

        Invoke(nameof(ResetTransferFlag), 1.0f);
    }

    private void ResetTransferFlag() => isTransferring = false;
}