using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]

/// <summary>
/// Represents a triggerable switch that can toggle between ON and OFF states.
/// Fires configurable <see cref="UnityEvent"/>s when the state changes. Designed for player interaction via trigger colliders.
/// </summary>
public class InteractableSwitch : MonoBehaviour
{
    [Header("State")]
    [Tooltip("Current state of the switch (true = ON).")]
    [SerializeField] bool isOn = false;

    [Header("Events")]
    [Tooltip("All UnityEvents to fire when the switch turns ON.")]
    public List<UnityEvent> onTurnedOn = new();
    [Tooltip("All UnityEvents to fire when the switch turns OFF.")]
    public List<UnityEvent> onTurnedOff = new();

    [Header("Audio")]
    public AudioClip onSound;
    public AudioClip offSound;

    private AudioSource audioSource;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Toggle();
    }

    public void Toggle()
    {
        isOn = !isOn;
        PlaySound(isOn);
        var list = isOn ? onTurnedOn : onTurnedOff;
        foreach (var e in list) e?.Invoke();
    }

    public void TurnOn()
    {
        if (!isOn)
        {
            isOn = true;
            PlaySound(true);
            foreach (var e in onTurnedOn) e?.Invoke();
        }
    }

    public void TurnOff()
    {
        if (isOn)
        {
            isOn = false;
            PlaySound(false);
            foreach (var e in onTurnedOff) e?.Invoke();
        }
    }

    private void PlaySound(bool turningOn)
    {
        if (audioSource == null) return;

        AudioClip clip = turningOn ? onSound : offSound;
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
