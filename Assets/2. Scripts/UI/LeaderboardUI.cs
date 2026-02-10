using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] rankTexts; // 1등부터 5등까지의 텍스트 UI 배열
    public GameObject leaderboardPanel; // 기록 창 패널

    private const string SAVE_KEY = "BestClearTimes";

    // 기록 창을 열 때 호출 (버튼에 연결)
    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        DisplayBestTimes();
    }

    private void DisplayBestTimes()
    {
        // 1. 저장된 데이터 읽기
        string savedData = PlayerPrefs.GetString(SAVE_KEY, "");
        
        if (string.IsNullOrEmpty(savedData))
        {
            foreach (var t in rankTexts) t.text = "NO RECORD";
            return;
        }

        // 2. 데이터 파싱 및 정렬
        List<float> times = savedData.Split(',')
            .Select(float.Parse)
            .OrderBy(t => t) // 빠른 순
            .ToList();

        // 3. UI 텍스트에 뿌려주기
        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (i < times.Count)
            {
                float time = times[i];
                int h = (int)(time / 3600);
                int m = (int)((time % 3600) / 60);
                int s = (int)(time % 60);
                rankTexts[i].text = $"{i + 1}. {h:D2}:{m:D2}:{s:D2}";
            }
            else
            {
                rankTexts[i].text = $"{i + 1}. --:--:--";
            }
        }
    }
}