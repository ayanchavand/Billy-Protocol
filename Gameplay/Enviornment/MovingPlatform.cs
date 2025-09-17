using UnityEngine;

/// <summary>
/// Oscillates the platform between two positions over time.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OscillatingPlatform : MonoBehaviour
{
    [Header("Position Settings (Local Space)")]
    [Tooltip("Local position when the platform is at its start.")]
    public Vector3 startPosition = Vector3.zero;
    [Tooltip("Local position when the platform is at its end.")]
    public Vector3 endPosition = Vector3.up;

    [Header("Motion Settings")]
    [Tooltip("Duration of one complete back-and-forth cycle, in seconds.")]
    public float cycleDuration = 2f;

    private float timer = 0f;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;
        startPosition = transform.localPosition;
    }

    void Update()
    {
        if (cycleDuration <= 0f) return;

        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / cycleDuration, 1f);
        transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        var parent = transform.parent;
        Vector3 worldStart = parent ? parent.TransformPoint(startPosition) : startPosition;
        Vector3 worldEnd = parent ? parent.TransformPoint(endPosition) : endPosition;
        Gizmos.DrawLine(worldStart, worldEnd);
        Gizmos.DrawSphere(worldStart, 0.05f);
        Gizmos.DrawSphere(worldEnd, 0.05f);
    }
}
