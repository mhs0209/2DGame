using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    public Active currentActive;
    
    [Header("Swap Settings")]
    public float swapCooldown = 1f; 
    private float lastSwapTime = -99f; // 시작 시 즉시 획득 가능하도록 초기화

    // ItemObject에서 쿨타임을 확인할 수 있도록 공개 메서드 제공
    public bool CanSwap() => Time.time >= lastSwapTime + swapCooldown;

    void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.useActive) && currentActive != null)
        {
            if (currentActive.currentCharge >= currentActive.activeData.maxCharges)
            {
                currentActive.Use(gameObject);
                currentActive.currentCharge = 0;
            }
        }
    }
    
    public void EquipActive(Active newItem)
    {
        // 1. 위치 정보 미리 확보 (새 아이템이 놓여있던 자리)
        Vector3 dropPosition = newItem.transform.position;

        // 2. 기존 아이템이 있다면 '그 자리'에 버리기
        if (currentActive != null)
        {
            DropCurrentItem(dropPosition);
        }

        // 3. 새 아이템 장착 및 쿨타임 갱신
        currentActive = newItem;
        lastSwapTime = Time.time; 

        // 4. 최초 획득 처리
        if (currentActive.isFirstPickup)
        {
            currentActive.currentCharge = currentActive.activeData.maxCharges;
            currentActive.isFirstPickup = false;
            Debug.Log($"{currentActive.activeData.itemName} 최초 획득!");
        }
    }

    private void DropCurrentItem(Vector3 targetPos)
    {
        // 부모 해제 및 위치 이동
        currentActive.transform.SetParent(null);
        currentActive.transform.position = targetPos;

        // 시각적/물리적 재활성화
        if (currentActive.TryGetComponent(out SpriteRenderer sr)) sr.enabled = true;
        if (currentActive.TryGetComponent(out Collider2D col)) col.enabled = true;

        currentActive = null;
    }

    public void AddChargeAll(int amount) => currentActive?.AddCharge(amount);
}