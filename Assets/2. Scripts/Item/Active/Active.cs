using UnityEngine;

public abstract class Active : ItemObject
{
    public ActiveItemData activeData; // SO 데이터
    public int currentCharge;

    public override void OnPickup(GameObject player)
    {
        // 1. 인벤토리에 등록 (ActiveInventory가 이 인스턴스를 참조함)
        player.GetComponent<ActiveInventory>().EquipActive(this);
    
        // 2. 부모 설정 (플레이어 자식으로 옮겨야 씬 전환 시 파괴되지 않고 추적이 쉬움)
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;

        // 3. 시각적/물리적 비활성화 (스크립트 컴포넌트는 살아있음)
        // GetComponent<Renderer>()가 아니라 구체적으로 SpriteRenderer를 꺼야 에러가 안 납니다.
        if (TryGetComponent(out SpriteRenderer sr)) sr.enabled = false;
        if (TryGetComponent(out Collider2D col)) col.enabled = false;
        
        Debug.Log("장착");
    }

    public void AddCharge(int amount)
    {
        if (activeData == null) return;
        currentCharge = Mathf.Min(currentCharge + amount, activeData.maxCharges);
    }

    public abstract void Use(GameObject player);
}