using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct SFXClip
{
    [Tooltip("Unique key to call this SFX")]
    public string key;
    [Tooltip("The AudioClip for this SFX")]
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // Singleton
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer (with exposed MusicVol & SFXVol)")]
    [SerializeField] private AudioMixer masterMixer = null;
    private const string MUSIC_PARAM = "MusicVol";
    private const string SFX_PARAM = "SFXVol";

    [Header("BGM Clips")]
    [SerializeField] private AudioClip menuBGM = null;
    [SerializeField] private AudioClip defaultBGM = null;

    [Header("Named SFX Clips")]
    [Tooltip("Populate this with all your one-shot SFX, giving each a unique key")]
    [SerializeField] private SFXClip[] sfxClips = null;

    // runtime
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Dictionary<string, AudioClip> sfxDict;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create music AudioSource
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.outputAudioMixerGroup =
                masterMixer.FindMatchingGroups("Music")[0];

            // Create SFX AudioSource
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.outputAudioMixerGroup =
                masterMixer.FindMatchingGroups("SFX")[0];

            // Build lookup dictionary for named SFX
            sfxDict = new Dictionary<string, AudioClip>();
            foreach (var entry in sfxClips)
            {
                if (!string.IsNullOrEmpty(entry.key) && entry.clip != null)
                {
                    if (!sfxDict.ContainsKey(entry.key))
                        sfxDict.Add(entry.key, entry.clip);
                    else
                        Debug.LogWarning($"AudioManager: duplicate SFX key '{entry.key}'");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ───── BGM ─────

    public void PlayMenuMusic()
    {
        if (menuBGM == null) return;
        if (musicSource.clip == menuBGM && musicSource.isPlaying) return;
        musicSource.clip = menuBGM;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (defaultBGM == null) return;
        if (musicSource.clip == defaultBGM && musicSource.isPlaying) return;
        musicSource.clip = defaultBGM;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ───── SFX ─────

    /// <summary>One-shot SFX by passing the AudioClip directly.</summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    /// <summary>One-shot SFX by its string key (as defined in the inspector).</summary>
    public void PlaySFX(string key)
    {
        if (sfxDict != null && sfxDict.TryGetValue(key, out var clip))
        {
            PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning($"AudioManager: no SFX found with key '{key}'");
        }
    }

    // ───── Volume Control ─────

    public void SetMusicVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(MUSIC_PARAM, dB);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(SFX_PARAM, dB);
    }
}
