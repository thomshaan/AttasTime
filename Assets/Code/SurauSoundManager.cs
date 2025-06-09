using UnityEngine;

public class SurauSoundManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource surauAudioSource;   // AudioSource for the Surau sound
    public AudioClip surauSoundClip;       // The audio clip to play

    [Header("Timing")]
    public int firstTimeHour = 12;   // 12 PM (noon)
    public int secondTimeHour = 18;  // 6 PM (afternoon)
    public int soundDuration = 27;   // Duration to play the sound (in seconds)

    private bool isSoundPlaying = false;

    void Start()
    {
        // Ensure AudioSource is assigned
        if (surauAudioSource == null || surauSoundClip == null)
        {
            Debug.LogError("SurauAudioSource or SurauSoundClip is not assigned!");
            return;
        }

        // Start with no sound playing
        surauAudioSource.Stop();
    }

    void Update()
    {
        // Get current in-game time from LightingManager
        int currentHour = Mathf.FloorToInt(LightingManager.Instance.TimeOfDay);

        // Check for time to trigger Surau sound
        if ((currentHour == firstTimeHour || currentHour == secondTimeHour) && !isSoundPlaying)
        {
            PlaySurauSound();
        }
        else if ((currentHour != firstTimeHour && currentHour != secondTimeHour) && isSoundPlaying)
        {
            StopSurauSound();
        }
    }

    // Method to play Surau sound for 27 seconds
    void PlaySurauSound()
    {
        if (!surauAudioSource.isPlaying)
        {
            surauAudioSource.clip = surauSoundClip;
            surauAudioSource.loop = false;
            surauAudioSource.Play();
            isSoundPlaying = true;

            // Stop after 27 seconds
            Invoke("StopSurauSound", soundDuration);
        }
    }

    // Method to stop Surau sound
    void StopSurauSound()
    {
        if (surauAudioSource.isPlaying)
        {
            surauAudioSource.Stop();
            isSoundPlaying = false;
        }
    }
}
