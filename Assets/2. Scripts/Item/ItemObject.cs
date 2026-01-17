using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public abstract void OnPickup(GameObject player);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- 액티브 아이템 습득 쿨타임 체크 ---
            if (this is Active)
            {
                var inven = collision.GetComponent<ActiveInventory>();
                if (inven != null && !inven.CanSwap()) return; // 쿨타임 중이면 무시
            }
            // ----------------------------------

            collision.GetComponent<PlayerSoundController>()?.PlayItemGetSound();
            OnPickup(collision.gameObject);

            if ((this is Active) == false)
            {
                Destroy(gameObject); 
            }
        }
    }
}