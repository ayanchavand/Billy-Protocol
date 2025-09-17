using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// Manages background ambience audio across scenes. 
/// Implements a singleton pattern to ensure only one instance exists,
/// automatically plays a default ambience clip on Awake, and persists
/// between scene loads. Designed to provide continuous atmospheric sound.
/// </summary>
public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip ambienceClip;
    public float volume = 0.5f;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Persist between scenes
        DontDestroyOnLoad(gameObject); 

        // Start ambience if not already playing
        if (!audioSource.isPlaying && ambienceClip != null)
        {
            audioSource.clip = ambienceClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
