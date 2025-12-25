using UnityEngine;
using Pathfinding; // A* 라이브러리 네임스페이스

public class NormalMonster : MonoBehaviour
{
    private RoomController parentRoom;
    private Transform player;
    
    // A* 라이브러리 컴포넌트
    private AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;

    [Header("Monster Settings")]
    public bool isFlying = false; // 비행 여부 (벽 무시 설정용)

    public void Setup(RoomController room)
    {
        parentRoom = room;
        
        // 1. 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 2. A* 컴포넌트 설정
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();

        if (aiDestinationSetter != null && player != null)
        {
            aiDestinationSetter.target = player;
        }

        // 3. 비행/지상 설정에 따른 레이어 무시 등 처리 (옵션)
        if (isFlying)
        {
            // 비행 몬스터라면 벽 레이어를 무시하도록 설정하는 로직을 여기에 추가 가능
            // aiPath.constrainInsideGraph = false; 
        }

        // 테스트용: 3~5초 후 사망
        Invoke("Die", Random.Range(3f, 5f));
    }

    // A* Pathfinding 라이브러리가 목적지 추적을 스스로 하므로 
    // 기존 Update의 SetDestination 코드는 더 이상 필요 없습니다.

    void Die()
    {
        if (parentRoom != null)
        {
            parentRoom.OnEnemyDeath(this.gameObject);
        }
        Destroy(gameObject);
    }
}