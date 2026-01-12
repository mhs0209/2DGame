using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawnCleaner : MonoBehaviour
{
    [SerializeField] private float checkRadius = 1.0f; // 타일을 검사할 반경
    [SerializeField] private LayerMask wallLayer;      // 인스펙터에서 'Wall' 레이어 선택 필수

    void Start()
    {
        ClearCenterObstacles();
    }

    private void ClearCenterObstacles()
    {
        // 1. 아이템 위치 주변에 있는 Wall 레이어의 콜라이더들을 찾음
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, wallLayer);

        foreach (var hit in hitColliders)
        {
            // 2. 콜라이더가 붙은 오브젝트에서 Tilemap 컴포넌트를 가져옴
            Tilemap tilemap = hit.GetComponent<Tilemap>();
            
            if (tilemap != null)
            {
                // 3. 월드 좌표를 해당 타일맵의 셀 좌표로 변환
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

                // 4. 중심점 기준 주변 타일 제거 (반경 내 타일들을 null로 설정)
                // 중앙 나무 4그루를 지우기 위해 범위를 넉넉히 잡음
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        Vector3Int targetCell = cellPosition + new Vector3Int(x, y, 0);
                        tilemap.SetTile(targetCell, null);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}