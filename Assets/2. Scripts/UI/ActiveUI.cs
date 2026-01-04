using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ActiveUI : MonoBehaviour
{
    public ActiveInventory inventory;
    [FormerlySerializedAs("itemIcon")] public Image itemDisplayImage;
    public Slider chargeSlider;
    private Active lastActive; // 이전 아이템 기억용

    void Update()
    {
        if (inventory.currentActive == null)
        {
            itemDisplayImage.enabled = false;
            return;
        }

        itemDisplayImage.enabled = true;

        // 아이템이 바뀌었을 때만 정보를 가져옴 (매 프레임 색상 덮어쓰기 방지)
        if (inventory.currentActive != lastActive)
        {
            lastActive = inventory.currentActive;
            // 1. 실제 월드에 존재하는 아이템의 SpriteRenderer를 가져옵니다.
            SpriteRenderer itemSR = inventory.currentActive.GetComponent<SpriteRenderer>();
            if (itemSR != null)
            {
                itemDisplayImage.sprite = itemSR.sprite;
                itemDisplayImage.color = itemSR.color; // 처음 장착할 때의 색상만 복사
            }
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