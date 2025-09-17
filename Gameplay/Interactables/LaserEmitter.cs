// LaserEmitter.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxDistance = 20f;
    public LayerMask obstacleMask = ~0;   // default: hit everything

    [Header("Power")]
    [Tooltip("Is the emitter currently powered and emitting a laser beam?")]
    public bool isPowered = true;

    [Header("Events")]
    public UnityEvent onLaserOn;   // fired when beam first hits a receiver
    public UnityEvent onLaserOff;  // fired when beam stops hitting a receiver

    private LineRenderer lineRenderer;
    private bool laserActive = false;     // true while the beam is lighting a receiver
    private LaserReceiver currentReceiver;

    // ????????????????????????????????????????????????????????????????????????????
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // If the emitter is switched OFF, make sure everything is clean and bail.
        if (!isPowered)
        {
            if (laserActive)
                DeactivateReceiver();

            lineRenderer.enabled = false;
            return;
        }

        // Emitter is ON ? do normal laser behaviour
        lineRenderer.enabled = true;

        Vector3 start = transform.position;
        Vector3 dir = transform.forward;
        Vector3 end = start + dir * maxDistance;

        if (Physics.Raycast(start, dir, out RaycastHit hit, maxDistance, obstacleMask))
        {
            end = hit.point;

            LaserReceiver receiver = hit.collider.GetComponent<LaserReceiver>();
            if (receiver != null)
            {
                if (!laserActive || receiver != currentReceiver)
                    ActivateReceiver(receiver);
            }
            else if (laserActive)
            {
                DeactivateReceiver();
            }
        }
        else if (laserActive)
        {
            DeactivateReceiver();
        }

        // Draw the beam
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    // ????????????????????????????????????????????????????????????????????????????
    private void ActivateReceiver(LaserReceiver receiver)
    {
        currentReceiver = receiver;
        laserActive = true;

        onLaserOn?.Invoke();
        receiver.Activate();
    }

    private void DeactivateReceiver()
    {
        onLaserOff?.Invoke();
        if (currentReceiver != null)
            currentReceiver.Deactivate();

        currentReceiver = null;
        laserActive = false;
    }

    // ????????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Power the emitter ON or OFF externally (e.g. from a switch).
    /// </summary>
    public void SetPowered(bool value)
    {
        // If we’re turning off, ensure any active receiver is shut down immediately.
        if (!value && laserActive)
            DeactivateReceiver();

        isPowered = value;
        lineRenderer.enabled = value;
    }

    /// <summary>
    /// Convenience toggle that can be wired to a button / UnityEvent.
    /// </summary>
    public void TogglePower() => SetPowered(!isPowered);
}
