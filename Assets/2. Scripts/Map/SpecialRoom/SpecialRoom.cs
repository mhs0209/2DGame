using UnityEngine;

public class SpecialRoom : ItemRoom
{
    // 1번 에러 해결: 부모의 OnRoomCleared를 반드시 구현(override)해야 함
    public override void OnRoomCleared()
    {
        if (controller.roomData is SpecialMap data)
        {
            // 특수 방 기믹(룰렛 등) 생성
            if (data.gimmickPrefab != null)
            {
                Instantiate(data.gimmickPrefab, transform.position, Quaternion.identity, transform);
                Debug.Log($"{data.type} 기믹이 생성되었습니다.");
            }
        }
    }

    private void Start()
    {
        // 만약 특수 방이 전투 없이 바로 기믹을 보여줘야 한다면 Start에서 호출
        OnRoomCleared();
    }
}