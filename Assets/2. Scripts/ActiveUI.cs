using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ActiveUI : MonoBehaviour
{
    public ActiveInventory inventory;
    [FormerlySerializedAs("itemIcon")] public Image itemDisplayImage;
    public Slider chargeSlider;

    void Update()
    {
        if (inventory.currentActive == null)
        {
            itemDisplayImage.enabled = false;
            return;
        }

        itemDisplayImage.enabled = true;

        // 1. 실제 월드에 존재하는 아이템의 SpriteRenderer를 가져옵니다.
        SpriteRenderer itemSR = inventory.currentActive.GetComponent<SpriteRenderer>();

        if (itemSR != null)
        {
            // 데이터의 아이콘 대신, 현재 렌더러의 스프라이트와 색상을 그대로 UI에 복사
            itemDisplayImage.sprite = itemSR.sprite;
            itemDisplayImage.color = itemSR.color; 
        }

        // 충전도 업데이트
        float max = inventory.currentActive.activeData.maxCharges;
        float current = inventory.currentActive.currentCharge;
        
        chargeSlider.maxValue = max;
        chargeSlider.value = current;
        
        // 게이지 꽉 찼을 때 색상 변경 등 연출 추가 가능
        chargeSlider.fillRect.GetComponent<Image>().color = (current >= max) ? Color.yellow : Color.white;
    }
}