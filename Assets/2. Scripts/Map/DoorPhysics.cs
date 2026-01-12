using System;
using Unity.VisualScripting;
using UnityEngine;

public class DoorPhysics : MonoBehaviour
{
    private SpriteRenderer[] srs;
    private Collider2D[] cols;
    public bool isSpecialLock = false;
    public RoomController myRoom;
    private bool isPlayerTouching; // 플레이어 접촉 상태 확인
    private GameObject player;

    void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        cols = GetComponentsInChildren<Collider2D>();
        myRoom = GetComponentInParent<RoomController>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (myRoom == null) Debug.Log("myRoom is null");
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isSpecialLock == false) return;
        
        if (other.gameObject.tag == "Player")
        {
            myRoom.OnPlayerTryEntry(this);
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