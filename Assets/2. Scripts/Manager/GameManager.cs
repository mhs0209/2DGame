using FreeworkGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    
    [Header("Audio")]
    public AudioMixer audioMixer;
    
    private float restartHoldTime = 0f;

    [Header("Key Bindings")]
    public KeyCode moveUp = KeyCode.UpArrow;
    public KeyCode moveDown = KeyCode.DownArrow;
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;

    public KeyCode attackUp = KeyCode.W;
    public KeyCode attackDown = KeyCode.S;
    public KeyCode attackLeft = KeyCode.A;
    public KeyCode attackRight = KeyCode.D;

    public KeyCode useActive = KeyCode.Space;
    public KeyCode usePickup = KeyCode.E;
    public KeyCode viewMinimap = KeyCode.Tab;
    public KeyCode restartKey = KeyCode.R;
    public KeyCode settingsKey = KeyCode.Escape;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 핵심: 모든 설정 로드 및 적용
        LoadAllSettings(); 
    }

    private void Update()
    {
        HandleRestartKey();
        if(Input.GetKeyDown(settingsKey)) ToggleSettings();
    }

    // --- 모든 설정 로드 및 적용 ---
    public void LoadAllSettings()
    {
        // 1. 키세팅 로드
        LoadAllKeys();

        // 2. 오디오 세팅 로드 및 즉시 적용 (설정창이 꺼져있어도 실행됨)
        ApplyAudioVolume("MasterVol", PlayerPrefs.GetFloat("MasterVol", 1f));
        ApplyAudioVolume("BGMVol", PlayerPrefs.GetFloat("BGMVol", 1f));
        ApplyAudioVolume("SFXVol", PlayerPrefs.GetFloat("SFXVol", 1f));
    }

    public void ApplyAudioVolume(string parameterName, float volume)
    {
        if (audioMixer != null)
        {
            // volume(0~1)을 데시벨(-80~0)로 변환
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat(parameterName, dB);
        }
    }

    // --- 초기화(Reset) 기능 ---
    public void ResetAllSettings()
    {
        // 저장 데이터 완전 삭제
        PlayerPrefs.DeleteAll();

        // 믹서와 변수들을 다시 기본값으로 로드
        LoadAllSettings();

        // 현재 설정창이 켜져 있다면 UI 갱신 (슬라이더 및 텍스트)
        if (settingsPanel.activeSelf)
        {
            settingsPanel.GetComponentInChildren<AudioSettingController>()?.RefreshUI();
            settingsPanel.GetComponentInChildren<KeySettingUI>()?.UpdateAllKeyTexts();
        }
        
        Debug.Log("모든 설정이 초기화되었습니다.");
    }

    // --- 기존 키 로드 로직 (UpdateKey 포함) ---
    public void UpdateKey(string actionName, KeyCode newKey)
    {
        PlayerPrefs.SetString(actionName, newKey.ToString());
        PlayerPrefs.Save();
        LoadAllKeys(); // 변수 업데이트
    }

    private void LoadAllKeys()
    {
        moveUp = LoadSingleKey("MoveUp", KeyCode.UpArrow);
        moveDown = LoadSingleKey("MoveDown", KeyCode.DownArrow);
        moveLeft = LoadSingleKey("MoveLeft", KeyCode.LeftArrow);
        moveRight = LoadSingleKey("MoveRight", KeyCode.RightArrow);
        attackUp = LoadSingleKey("AttackUp", KeyCode.W);
        attackDown = LoadSingleKey("AttackDown", KeyCode.S);
        attackLeft = LoadSingleKey("AttackLeft", KeyCode.A);
        attackRight = LoadSingleKey("AttackRight", KeyCode.D);
        useActive = LoadSingleKey("UseActive", KeyCode.Space);
        usePickup = LoadSingleKey("UsePickup", KeyCode.E);
        viewMinimap = LoadSingleKey("ViewMinimap", KeyCode.Tab);
        restartKey = LoadSingleKey("Restart", KeyCode.R);
        settingsKey = LoadSingleKey("Settings", KeyCode.Escape);
    }

    private KeyCode LoadSingleKey(string actionName, KeyCode defaultValue)
    {
        string keyStr = PlayerPrefs.GetString(actionName, defaultValue.ToString());
        return (KeyCode)Enum.Parse(typeof(KeyCode), keyStr);
    }

    private void HandleRestartKey()
    {
        if (Input.GetKey(restartKey))
        {
            restartHoldTime += Time.deltaTime;
            if (restartHoldTime >= 3.0f)
            {
                RestartGame();
            }
        }
        else
        {
            restartHoldTime = 0f;
        }
    }
    
    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        Time.timeScale = settingsPanel.activeSelf ? 0f : 1f;
        if (RunDataManager.Instance != null)
        {
            if (settingsPanel.activeSelf)
            {
                RunDataManager.Instance.StopTimer();
            }
            else
            {
                RunDataManager.Instance.StartTimer();
            }
        }
    }

    // --- 기존 게임 상태 로직 ---

    public void OnPlayerDeath()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        // 런데이터 초기화
        if (RunDataManager.Instance != null)
        {
            RunDataManager.Instance.ResetData();
            // 초기화 후 다시 시작할 때 타이머 작동 시작
            RunDataManager.Instance.StartTimer();
        }
        if (AniManager.Instance != null) Destroy(AniManager.Instance.gameObject);
        if (InGameUICanvasDDOL.Instance != null) Destroy(InGameUICanvasDDOL.Instance.gameObject);
        SceneManager.LoadScene("Stage01");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        if (AniManager.Instance != null) Destroy(AniManager.Instance.gameObject);
        if (InGameUICanvasDDOL.Instance != null) Destroy(InGameUICanvasDDOL.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}