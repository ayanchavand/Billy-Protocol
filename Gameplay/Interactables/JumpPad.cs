using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]

/// <summary>
/// Launches the player upward when they enter the pad's trigger area.
/// Supports enabling/disabling the pad, adjusting launch force, 
/// playing particle effects, and triggering a sound on activation.
/// Designed for player interaction in platforming scenarios.
/// </summary>
public class JumpPad : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Enable or disable the jump pad.")]
    public bool isActive = true;

    [Header("Launch Settings")]
    [Tooltip("Strength of the launch impulse.")]
    public float launchForce = 12f;

    [Header("Effects")]
    [Tooltip("Optional particle system that plays when the pad is active.")]
    public ParticleSystem padParticles;

    [Header("Audio")]
    [Tooltip("Audio clip to play when the jump pad activates.")]
    public AudioClip jumpPadClip;

    private Collider col;
    private AudioSource audioSource;

    void Reset()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

        if (!col.isTrigger)
            Debug.LogWarning($"{name}: Collider should be set to 'Is Trigger' for the JumpPad to work.");

        // Sync particle system with initial state
        if (padParticles != null)
        {
            if (isActive) padParticles.Play();
            else padParticles.Stop();
        }

        col.isTrigger = isActive;
    }

    // When a player enters the jump pad's trigger area jump them upwards
    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        var pc = other.GetComponent<PlatformerController>();
        if (pc == null) return;

        Vector3 dir = transform.up.normalized;
        pc.Launch(dir * launchForce);

        if (jumpPadClip != null)
        {
            audioSource.PlayOneShot(jumpPadClip);
        }
    }

    public void SetActive(bool on)
    {
        Debug.Log("Jumpadstate before call: " + isActive);
        isActive = on;

        if (padParticles != null)
        {
            if (isActive)
            {
                col.isTrigger = true;
                padParticles.Play();
            }
            else if (padParticles.isPlaying)
            {
                padParticles.Stop();
                col.isTrigger = false;
            }
        }

        Debug.Log("Jumpadstate after call: " + isActive);
    }

    public void SetJumpPadForce(float force)
    {
        launchForce = force;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isActive ? Color.cyan : Color.gray;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * launchForce);
        Gizmos.DrawSphere(transform.position + transform.up * launchForce, 0.2f);
    }
}
