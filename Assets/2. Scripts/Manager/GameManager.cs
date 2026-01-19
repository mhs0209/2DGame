using FreeworkGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    
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
            LoadAllKeys(); // 게임 시작 시 저장된 키 불러오기
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleRestartKey();
        if(Input.GetKeyDown(settingsKey)) ToggleSettings();
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
    }

    // --- 키 저장 및 로드 로직 ---

    public void UpdateKey(string actionName, KeyCode newKey)
    {
        switch (actionName)
        {
            case "MoveUp": moveUp = newKey; break;
            case "MoveDown": moveDown = newKey; break;
            case "MoveLeft": moveLeft = newKey; break;
            case "MoveRight": moveRight = newKey; break;
            case "AttackUp": attackUp = newKey; break;
            case "AttackDown": attackDown = newKey; break;
            case "AttackLeft": attackLeft = newKey; break;
            case "AttackRight": attackRight = newKey; break;
            case "UseActive": useActive = newKey; break;
            case "UsePickup": usePickup = newKey; break;
            case "ViewMinimap": viewMinimap = newKey; break;
            case "Restart": restartKey = newKey; break;
            case "Settings": settingsKey = newKey; break;
        }
        // 문자열로 저장
        PlayerPrefs.SetString(actionName, newKey.ToString());
        PlayerPrefs.Save();
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

    // --- 기존 게임 상태 로직 ---

    public void OnPlayerDeath()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
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