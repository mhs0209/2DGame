using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public abstract void OnPickup(GameObject player);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickup(collision.gameObject);
            Destroy(gameObject); // 획득 후 오브젝트 파괴
        }
    }
}