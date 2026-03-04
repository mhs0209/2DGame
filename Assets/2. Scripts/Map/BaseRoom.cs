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


    private float safetyOffset = 1f; 
    [HideInInspector] public Transform topSpawn, bottomSpawn, leftSpawn, rightSpawn;

    private void Awake()
    {
        // 씬 시작 시 혹은 동적 생성 시 스폰 포인트 자동 생성
        GenerateSpawnPoints();
    }

    // Awake나 SetupDoors 이후 호출
    public void GenerateSpawnPoints()
    {
        // 1. 기존 포인트 삭제 (재생성 시 대비)
        foreach (string name in new[] { "TopSpawn", "BottomSpawn", "LeftSpawn", "RightSpawn" })
        {
            Transform t = transform.Find(name);
            if (t != null) DestroyImmediate(t.gameObject);
        }

        // 2. 각 문의 위치를 기준으로 스폰 포인트 생성
        if (top.door != null) topSpawn = CreatePoint("TopSpawn", top.door.transform.localPosition + Vector3.down * safetyOffset);
        if (bottom.door != null) bottomSpawn = CreatePoint("BottomSpawn", bottom.door.transform.localPosition + Vector3.up * safetyOffset);
        if (left.door != null) leftSpawn = CreatePoint("LeftSpawn", left.door.transform.localPosition + Vector3.right * safetyOffset);
        if (right.door != null) rightSpawn = CreatePoint("RightSpawn", right.door.transform.localPosition + Vector3.left * safetyOffset);
    }

    private Transform CreatePoint(string name, Vector3 localPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    // MapGenerator에서 방을 다 만든 후 반드시 호출해줘야 함
    public void SetupDoors(System.Func<Vector2Int, bool> hasRoomAt)
    {
        SetDoorState(top, hasRoomAt(gridPos + Vector2Int.up));
        SetDoorState(bottom, hasRoomAt(gridPos + Vector2Int.down));
        SetDoorState(left, hasRoomAt(gridPos + Vector2Int.left));
        SetDoorState(right, hasRoomAt(gridPos + Vector2Int.right));
        
        // 문이 활성화된 후 그 위치를 기반으로 포인트 생성
        GenerateSpawnPoints();
    }
    
    private Vector2Int[] neighbors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public virtual void OnPlayerEnter()
    {
        // 1. 미니맵 업데이트
        if (MinimapManager.Instance != null)
        {
            MinimapManager.Instance.UpdateRoomIcon(gridPos, true);
            foreach (var dir in neighbors)
            {
                MinimapManager.Instance.UpdateRoomIcon(gridPos + dir, false);
            }
        }
        // 2. 방 로직 실행
        var controller = GetComponent<RoomController>();
        if (controller != null) controller.ActivateRoomLogic();
    }

    private void SetDoorState(DoorSet doorSet, bool exists)
    {
        if (doorSet.door != null) doorSet.door.SetActive(exists);
        if (doorSet.wall != null) doorSet.wall.SetActive(!exists);
    }

    // 디버깅용: 에디터에서 스폰 지점 확인
    private void OnDrawGizmosSelected()
    {
        if (topSpawn == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(topSpawn.position, 0.3f);
        Gizmos.DrawSphere(bottomSpawn.position, 0.3f);
        Gizmos.DrawSphere(leftSpawn.position, 0.3f);
        Gizmos.DrawSphere(rightSpawn.position, 0.3f);
    }
}