using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUICanvasDDOL : MonoBehaviour
{
    public static SettingUICanvasDDOL Instance;
    public GameObject startButton;
    
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject); // 이미 존재한다면 새로 생긴 녀석을 제거!
        }
    }
}
