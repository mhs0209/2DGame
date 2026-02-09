using UnityEngine;
using TMPro; // TextMeshPro 사용 권장

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private bool showMilliseconds = false; // 밀리초 표시 여부

    void Update()
    {
        if (RunDataManager.Instance == null) return;

        UpdateTimerText(RunDataManager.Instance.elapsedTime);
    }

    private void UpdateTimerText(float time)
    {
        int hours = (int)(time / 3600);
        int minutes = (int)((time % 3600) / 60);
        int seconds = (int)(time % 60);

        if (showMilliseconds)
        {
            int fraction = (int)((time * 100) % 100);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}.{3:02}", hours, minutes, seconds, fraction);
        }
        else
        {
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }
}