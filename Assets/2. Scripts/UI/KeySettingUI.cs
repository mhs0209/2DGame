using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하지 않는다면 그냥 Text로 바꾸세요

public class KeySettingUI : MonoBehaviour
{
    [Header("Waiting UI")]
    public GameObject waitingPanel; // "아무 키나 누르세요" 안내창

    [System.Serializable]
    public struct KeyMapUI
    {
        public string actionName;      // GameManager의 switch-case와 일치해야 함
        public TextMeshProUGUI keyText; // 현재 설정된 키를 표시할 텍스트
    }

    public List<KeyMapUI> keyMapList = new List<KeyMapUI>();

    private bool isRebinding = false;

    private void OnEnable()
    {
        UpdateAllKeyTexts();
    }

    // 모든 UI 텍스트를 현재 GameManager의 변수값으로 갱신
    public void UpdateAllKeyTexts()
    {
        foreach (var item in keyMapList)
        {
            item.keyText.text = GetKeyCodeByActionName(item.actionName).ToString();
        }
    }

    // 버튼에서 호출할 함수 (예: ChangeKey("MoveUp"))
    public void StartRebinding(string actionName)
    {
        if (!isRebinding)
        {
            StartCoroutine(RebindRoutine(actionName));
        }
    }

    private IEnumerator RebindRoutine(string actionName)
    {
        isRebinding = true;
        if (waitingPanel != null) waitingPanel.SetActive(true);

        yield return null; // 클릭 시 발생하는 입력을 방지

        bool keySet = false;
        while (!keySet)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        // ESC는 취소용으로 사용 (원한다면 변경 가능)
                        if (k != KeyCode.Escape)
                        {
                            GameManager.Instance.UpdateKey(actionName, k);
                        }
                        keySet = true;
                        break;
                    }
                }
            }
            yield return null;
        }

        if (waitingPanel != null) waitingPanel.SetActive(false);
        UpdateAllKeyTexts();
        isRebinding = false;
    }

    private KeyCode GetKeyCodeByActionName(string actionName)
    {
        switch (actionName)
        {
            case "MoveUp": return GameManager.Instance.moveUp;
            case "MoveDown": return GameManager.Instance.moveDown;
            case "MoveLeft": return GameManager.Instance.moveLeft;
            case "MoveRight": return GameManager.Instance.moveRight;
            case "AttackUp": return GameManager.Instance.attackUp;
            case "AttackDown": return GameManager.Instance.attackDown;
            case "AttackLeft": return GameManager.Instance.attackLeft;
            case "AttackRight": return GameManager.Instance.attackRight;
            case "UseActive": return GameManager.Instance.useActive;
            case "UsePickup": return GameManager.Instance.usePickup;
            case "ViewMinimap": return GameManager.Instance.viewMinimap;
            case "Restart": return GameManager.Instance.restartKey;
            case "Settings": return GameManager.Instance.settingsKey;
            default: return KeyCode.None;
        }
    }
}