using UnityEngine;

public abstract class ItemRoom : MonoBehaviour
{
    protected RoomController controller;

    protected virtual void Awake()
    {
        controller = GetComponent<RoomController>();
    }

    // RoomController의 EndBattle에서 호출됨
    public abstract void OnRoomCleared();
}