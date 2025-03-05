using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip blockCaughtSuccess;  // Sound when correct block is caught
    public AudioClip blockCaughtWrong;    // Sound when wrong block is caught
    public AudioClip blockMissed;         // Sound when block is missed
    public AudioClip levelComplete;       // Sound when level is completed
    public AudioClip gameOver;            // Sound when game is over

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize audio sources if not set
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = 0.5f;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 1.0f;
        }
    }

    // Play a sound effect
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Play specific game sounds
    public void PlaySuccessSound()
    {
        PlaySFX(blockCaughtSuccess);
    }

    public void PlayWrongSound()
    {
        PlaySFX(blockCaughtWrong);
    }

    public void PlayMissedSound()
    {
        PlaySFX(blockMissed);
    }

    public void PlayLevelCompleteSound()
    {
        PlaySFX(levelComplete);
    }

    public void PlayGameOverSound()
    {
        PlaySFX(gameOver);
    }
}