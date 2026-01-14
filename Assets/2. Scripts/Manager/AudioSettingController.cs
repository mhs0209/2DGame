using UnityEngine;
using UnityEngine.Audio; // 오디오 믹서 사용을 위해 필요
using UnityEngine.UI;    // UI 제어를 위해 필요

public class AudioSettingController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 초기 슬라이더 값 설정 (기본값 1)
        masterSlider.value = 1f;
        bgmSlider.value = 1f;
        sfxSlider.value = 1f;

        // 슬라이더 값이 변경될 때 실행될 리스너 연결
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        // 로그 계산: dB = 20 * log10(volume)
        // volume이 1일 때 0dB(원본 소리), 2일 때 약 +6dB, 0.0001일 때 -80dB가 됨
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }
}