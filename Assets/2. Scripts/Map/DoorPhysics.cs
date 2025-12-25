using UnityEngine;

public class DoorPhysics : MonoBehaviour
{
    private SpriteRenderer[] srs;
    private Collider2D[] cols;
    public bool isSpecialLock = false; 

    void Awake()
    {
        // 자식에 있는 모든 렌더러와 콜라이더를 배열로 가져옴
        srs = GetComponentsInChildren<SpriteRenderer>();
        cols = GetComponentsInChildren<Collider2D>();
    }

    public void SetLock(bool lockState)
    {
        // 모든 자식의 색상을 변경
        if (srs != null)
        {
            foreach (var sr in srs) 
                sr.color = lockState ? Color.red : Color.white;
        }

        // 모든 자식의 물리 상태 변경
        if (cols != null)
        {
            foreach (var col in cols) 
                col.isTrigger = !lockState;
        }
    }
}