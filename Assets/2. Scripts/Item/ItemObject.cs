using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public abstract void OnPickup(GameObject player);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickup(collision.gameObject);
            if ((this is Active) == false)
            {
                Destroy(gameObject); // 액티브가 아닐 때만 파괴
            }
        }
    }
}