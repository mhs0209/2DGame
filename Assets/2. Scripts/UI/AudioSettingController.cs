using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 1. 저장된 값 불러오기 (기본값 1f)
        float m = PlayerPrefs.GetFloat("MasterVol", 1f);
        float b = PlayerPrefs.GetFloat("BGMVol", 1f);
        float s = PlayerPrefs.GetFloat("SFXVol", 1f);

        // 2. UI 및 믹서 초기화
        masterSlider.value = m;
        bgmSlider.value = b;
        sfxSlider.value = s;

        // 초기 로드시 믹서에 적용 (오디오 믹서 파라미터가 노출되어 있어야 함)
        SetMasterVolume(m);
        SetBGMVolume(b);
        SetSFXVolume(s);

        // 리스너 연결
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MasterVol", volume); // 저장
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("BGMVol", volume); // 저장
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVol", volume); // 저장
    }

    private void OnDisable()
    {
        PlayerPrefs.Save(); // 설정창을 닫을 때 확실히 저장
    }
}