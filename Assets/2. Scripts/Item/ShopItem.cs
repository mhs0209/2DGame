using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price;
    private ItemObject item;
    private TextMeshPro priceUI;

    public void Initialize(int p)
    {
        price = p;
        item = GetComponent<ItemObject>();
        
        // 가격 표시 UI 동적 생성 (간단한 예시)
        GameObject uiObj = new GameObject("PriceUI");
        uiObj.transform.SetParent(this.transform);
        uiObj.transform.localPosition = Vector3.up * -1.2f;
        
        priceUI = uiObj.AddComponent<TextMeshPro>();
        priceUI.alignment = TextAlignmentOptions.Center;
        priceUI.fontSize = 4;
        priceUI.text = $"{price} G";
        priceUI.sortingOrder = 2;
        item.GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStat pStat = collision.gameObject.GetComponent<PlayerStat>();
            if (pStat.gold >= price)
            {
                pStat.gold -= price;
                pStat.OnStatChanged?.Invoke();
                item.OnPickup(collision.gameObject); // 원래 아이템의 효과 실행
                Destroy(gameObject); // 구매 완료 후 제거
            }
            else
            {
                Debug.Log("골드가 부족합니다!");
            }
        }
    }
}