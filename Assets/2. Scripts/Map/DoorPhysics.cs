using System;
using Unity.VisualScripting;
using UnityEngine;

public class DoorPhysics : MonoBehaviour
{
    private SpriteRenderer[] srs;
    private Collider2D[] cols;
    public bool isSpecialLock = false;
    public RoomController myRoom;
    private bool isPlayerTouching = false; // 플레이어 접촉 상태 확인
    private GameObject player;

    void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        cols = GetComponentsInChildren<Collider2D>();
        myRoom = GetComponentInParent<RoomController>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (myRoom == null) Debug.Log("myRoom is null");
    }

    void Update()
    {
        if (!isSpecialLock) return;
        
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
        
            // 문과 플레이어 거리가 1.5 유닛 이내일 때
            if (dist < 1f)
            {
                myRoom.OnPlayerTryEntry(this);
            }
        }
    }

    public void SetLock(bool lockState)
    {
        if (srs != null)
        {
            foreach (var sr in srs) sr.color = lockState ? Color.red : Color.white;
        }
        if (cols != null)
        {
            foreach (var col in cols) col.isTrigger = !lockState;
        }
    }
}