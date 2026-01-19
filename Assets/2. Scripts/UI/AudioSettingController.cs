using UnityEngine;
using UnityEngine.UI;

public class AudioSettingController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 리스너 연결 (값이 바뀔 때마다 GameManager에 전달)
        masterSlider.onValueChanged.AddListener((val) => {
            GameManager.Instance.ApplyAudioVolume("MasterVol", val);
            PlayerPrefs.SetFloat("MasterVol", val);
        });
        bgmSlider.onValueChanged.AddListener((val) => {
            GameManager.Instance.ApplyAudioVolume("BGMVol", val);
            PlayerPrefs.SetFloat("BGMVol", val);
        });
        sfxSlider.onValueChanged.AddListener((val) => {
            GameManager.Instance.ApplyAudioVolume("SFXVol", val);
            PlayerPrefs.SetFloat("SFXVol", val);
        });

        RefreshUI(); // 초기 UI 상태 갱신
    }

    // 저장된 값에 맞춰 슬라이더 위치를 옮겨주는 함수
    public void RefreshUI()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVol", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}