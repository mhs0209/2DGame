using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    void Awake() => Instance = this;

    // 부드러운 이동 대신 즉시 위치를 고정하는 메서드
    public void ImmediateMove(Vector3 targetPos)
    {
        // Z축 값은 유지 (-10)
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }
}