using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Drag your two sliders here")]
    [SerializeField] private Slider bgmSlider = null;
    [SerializeField] private Slider sfxSlider = null;

    void Awake()
    {
        // Make sure sliders can produce floats between 0–1
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.wholeNumbers = false;

        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.wholeNumbers = false;

        // Subscribe to their OnValueChanged events
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    void Start()
    {
        // Push the initial slider values through immediately
        OnBgmSliderChanged(bgmSlider.value);
        OnSfxSliderChanged(sfxSlider.value);
    }

    private void OnBgmSliderChanged(float value)
    {
        Debug.Log($"[Settings] BGM slider → {value}");
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxSliderChanged(float value)
    {
        Debug.Log($"[Settings] SFX slider → {value}");
        AudioManager.Instance.SetSFXVolume(value);
    }
}
